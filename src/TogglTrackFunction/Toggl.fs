[<AutoOpen>]
module TogglTrackFunction.Toggl

open Hopac
open Hopac.Infixes
open HttpFs
open Thoth.Json.Net
open FsToolkit.ErrorHandling

type ContentType = Client.ContentType

let inline (^) f x = f x 

let appJson = 
    ContentType.create("application", "json")
    |> Client.RequestHeader.ContentType

type TogglCredentials with 

    member this.Authenticate = 
        match this with
            | TogglCredentials.Full credentials -> credentials.Username, credentials.Password
            | TogglCredentials.Token token -> token, "api_token"
        ||> Client.Request.basicAuthentication 

    member this.GetTimeEntries =
        Client.Request.createUrl Client.HttpMethod.Get "https://api.track.toggl.com/api/v9/me/time_entries"
        |> Client.Request.setHeader appJson
        |> this.Authenticate
        |> Client.getResponse
        |> Job.bind ^ Client.Response.readBodyAsString
        |> Job.map ^ Thoth.Json.Net.Decode.fromString(
            Decode.Auto.generateDecoder<Toggl.Api.TimeEntries.GetTimeEntries.Response.Main>(
                extra = Extra.withInt64 Extra.empty
            )
        )

    member this.GetLastTimeEntry = jobResult {
        match! this.GetTimeEntries with
        | last :: _ -> return last
        | [] -> return! Error "List of time entries is empty."
    }

    member this.UpdateTimeEntry (workspaceId : int) (timeEntryId : int64) (updateTimeEntry : Toggl.Api.TimeEntries.UpdateTimeEntry.Request.MainItem) = jobResult {
        let! response =
            $"https://api.track.toggl.com/api/v9/workspaces/{workspaceId}/time_entries/{timeEntryId}"
            |> Client.Request.createUrl Client.HttpMethod.Put 
            |> Client.Request.setHeader appJson
            |> Client.Request.bodyString (
                Thoth.Json.Net.Encode.Auto.toString(
                    updateTimeEntry
                    , extra = Extra.withInt64 Extra.empty
                )
            )
            |> this.Authenticate
            |> Client.getResponse
        let! body = Client.Response.readBodyAsString response
        if response.statusCode >= 300 then
            do! Error body
    }

type Toggl.Api.TimeEntries.GetTimeEntries.Response.MainItem with 
    member this.AsUpdate () : Toggl.Api.TimeEntries.UpdateTimeEntry.Request.MainItem = { 
        billable = Some this.billable
        created_with = None
        description = this.description
        duration = this.duration
        project_id = this.project_id
        start = this.start
        start_date = None
        stop = this.stop
        tag_action = ""
        tag_ids = Option.defaultValue [] this.tag_ids
        tags = Option.defaultValue [] this.tags
        task_id = this.task_id
        user_id = this.user_id
        workspace_id = this.workspace_id
    }
