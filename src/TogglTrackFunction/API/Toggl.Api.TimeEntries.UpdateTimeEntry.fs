///PUT TimeEntries.
///https://api.track.toggl.com/api/v9/workspaces/{workspace_id}/time_entries/{time_entry_id}
///Updates a workspace time entry.
module Toggl.Api.TimeEntries.UpdateTimeEntry

module Request =
    type MainItem =
        {
            ///Whether the time entry is marked as billable, optional, default false
            ///GEN: Option because description contains "optional".
            billable: bool option
            ///Must be provided when creating a time entry and should identify the service/application used to create it
            created_with: string option
            ///Time entry description, optional
            ///GEN: Option because description contains "optional".
            description: string option
            ///Time entry duration. For running entries should be negative, preferable -1
            duration: int
            ///Project ID, optional
            ///GEN: Option because description contains "optional".
            project_id: int option
            ///Start time in UTC, required for creation. Format: 2006-01-02T15:04:05Z
            ///GEN: DateTime because name = "start".
            start: System.DateTime
            ///If provided during creation, the date part will take precedence over the date part of "start". Format: 2006-11-07
            start_date: string option
            ///Stop time in UTC, can be omitted if it's still running or created with "duration". If "stop" and "duration" are provided, values must be consistent (start + duration == stop)
            ///GEN: DateTime because name = "stop".
            ///GEN: Option because description contains "can be omitted".
            stop: System.DateTime option
            ///Can be "add" or "delete". Used when updating an existing time entry
            tag_action: string
            ///IDs of tags to add/remove
            tag_ids: int list
            ///Names of tags to add/remove. If name does not exist as tag, one will be created automatically
            tags: string list
            ///Task ID, optional
            ///GEN: Option because description contains "optional".
            task_id: int option
            ///Time Entry creator ID, if omitted will use the requester user ID
            user_id: int
            ///Workspace ID, required
            workspace_id: int
        }

    type Main = MainItem

///A workspace TimeEntry.
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

    type Main = MainItem