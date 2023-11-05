///GET TimeEntries.
///https://api.track.toggl.com/api/v9/me/time_entries
///Lists latest time entries.
module Toggl.Api.TimeEntries.GetTimeEntries

module Response =
    type MainItem =
        {
            ///When was last updated
            ///GEN: DateTime because name = "at".
            at: System.DateTime
            ///Whether the time entry is marked as billable
            billable: bool
            ///Time Entry description, null if not provided at creation/update
            description: string option
            ///Time entry duration. For running entries should be negative, preferable -1
            duration: int
            ///Time Entry ID
            id: int64
            ///Project ID. Can be null if project was not provided or project was later deleted
            project_id: int option
            ///When was deleted, null if not deleted
            ///GEN: DateTime because name "server_deleted_at" ends with "_at".
            server_deleted_at: System.DateTime option
            ///Start time in UTC
            ///GEN: DateTime because name = "start".
            start: System.DateTime
            ///Stop time in UTC, can be null if it's still running or created with "duration" and "duronly" fields
            ///GEN: DateTime because name = "stop".
            ///GEN: Option because description contains "null if".
            stop: System.DateTime option
            ///Tag IDs, null if tags were not provided or were later deleted
            ///GEN: Option because description contains "null if".
            tag_ids: int list option
            ///Tag names, null if tags were not provided or were later deleted
            ///GEN: Option because description contains "null if".
            tags: string list option
            ///Task ID. Can be null if task was not provided or project was later deleted
            task_id: int option
            ///Time Entry creator ID
            user_id: int
            ///Workspace ID
            workspace_id: int
        }

    type Main = MainItem list