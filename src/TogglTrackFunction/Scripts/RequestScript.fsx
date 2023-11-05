#i "nuget: https://api.nuget.org/v3/index.json"

#r "nuget: Hopac, 0.5.0"
#r "nuget: Http.fs"
#r "nuget: Thoth.Json.net"
#r "nuget: Microsoft.Extensions.Configuration.UserSecrets"
#load "..\IO.fs"


open HttpFs
open Hopac
open Hopac.Infixes
open Thoth.Json.Net

let inline (^) f x = f x 

module UserSecrets =
    open Microsoft.Extensions.Configuration

    let configuration = 
        ConfigurationBuilder()
            .AddUserSecrets("7f6df90b-7a2a-4ef5-ba03-186fdb5eb6d3")
            .Build()
    let url = 
        configuration.["Toggl:YCFunctionUrl"]
    let username = 
        configuration.["Toggl:Username"]
    let password =
        configuration.["Toggl:Password"]
    let token =
        configuration.["Toggl:Token"]
    let credentials =
        if System.String.IsNullOrWhiteSpace token 
        then
            TogglTrackFunction.Full {
                Username = username
                Password = password
            }
        else 
            TogglTrackFunction.Token token

type ContentType = Client.ContentType
let appJson = 
    ContentType.create ("application", "json")
    |> Client.RequestHeader.ContentType

let sendCommandRaw (command : TogglTrackFunction.Command) =
    Client.Request.createUrl Client.HttpMethod.Post UserSecrets.url
    |> Client.Request.setHeader appJson
    |> Client.Request.bodyString ^ Encode.Auto.toString command
    |> Client.getResponse
    |> Job.bind Client.Response.readBodyAsString

let sendCommand<'response> command = job {
    let! response = sendCommandRaw command
    return Decode.Auto.fromString<'response>(
        response
        , extra = Extra.withInt64 Extra.empty
    )
}
    
let getVersion =
    sendCommand<TogglTrackFunction.VersionResponse> TogglTrackFunction.Command.Version

let extendUpToDateV1 =
    TogglTrackFunction.Command.ExtendUpToDateV1 {
        Username = UserSecrets.username
        Password = UserSecrets.password
    }
    |> sendCommand<TogglTrackFunction.ExtendUpToDateResponse>

let extendUpToDateV2 =
    TogglTrackFunction.Command.ExtendUpToDateV2 UserSecrets.credentials
    |> sendCommand<TogglTrackFunction.ExtendUpToDateResponse>
    
run getVersion

run extendUpToDateV1

run extendUpToDateV2