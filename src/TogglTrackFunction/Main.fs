namespace TogglTrackFunction

open Hopac
open FsToolkit.ErrorHandling

[<AutoOpen>]
module Utils = 
    let inline (^) f x = f x 
            
type Request = {
    httpMethod : string
    body : string
}

type Response = {
    StatusCode : int
    Body : string 
}

module Handle = 
    open Thoth.Json.Net

    type Response with
        static member ok content = {
            StatusCode = 200
            Body = Encode.Auto.toString( 
                content
                , extra = Extra.withInt64 Extra.empty
            )
        }

    let (|FromFull|) (credentials : FullCredentials) = Full credentials

    let fromCommand cmd = jobResult {
        match cmd with 
        | Command.Version ->
            let version = 
                System.Reflection.Assembly.GetCallingAssembly()
                    .GetCustomAttributes(typeof<System.Reflection.AssemblyInformationalVersionAttribute>, false)
                |> Array.exactlyOne
                |> unbox<System.Reflection.AssemblyInformationalVersionAttribute>
            //let body = 
            //    Encode.Auto.toString {
            //        VersionResponse.Version = version.InformationalVersion
            //    }
            //return {
            //    StatusCode = 200
            //    Body = body
            //}
            return Response.ok {
                VersionResponse.Version = version.InformationalVersion
            }

        | Command.GetLastTimeEntryV1 (FromFull credentials) 
        | Command.GetLastTimeEntryV2 credentials ->
            let! last = credentials.GetLastTimeEntry
            return Response.ok {
                LastTimeEntryResponse.Description = last.description
                Duration = last.duration
                Start =  last.start
                ProjectId = last.project_id
                Id = last.id
            }

        | Command.ExtendUpToDateV1 (FromFull credentials) 
        | Command.ExtendUpToDateV2 credentials ->
            let! last = credentials.GetLastTimeEntry
            if last.duration < 0 then 
                do! Error "Time Entry is not finished"
            let updateTimeEntry = 
                let utcNow = System.DateTime.UtcNow 
                { last.AsUpdate() with
                    duration = int (utcNow.Subtract last.start).TotalSeconds
                    stop = Some utcNow
                }
            do! credentials.UpdateTimeEntry last.workspace_id last.id updateTimeEntry 
            return Response.ok {
                ExtendUpToDateResponse.Description = last.description
                Duration = last.duration
                Start =  last.start
                ProjectId = last.project_id
                Id = last.id
            }     
    }

    let fromRequestBody request = jobResult {
        let! command = Decode.Auto.fromString ( 
            request
            , extra = Extra.withInt64 Extra.empty
        )
        return! fromCommand command
    }

type Handler () =
                
    member this.FunctionHandler (request : Request) = run ^ job {
        match! Handle.fromRequestBody request.body with
        | Error err -> 
            return {
                StatusCode = 400
                Body = err
            }
        | Ok response ->
            return response
    }