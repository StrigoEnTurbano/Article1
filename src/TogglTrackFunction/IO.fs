namespace TogglTrackFunction

type FullCredentials = {
    Username : string
    Password : string
}

type TogglCredentials = 
    | Full of FullCredentials
    | Token of string

type Command = 
    | Version
    | GetLastTimeEntryV1 of FullCredentials
    | GetLastTimeEntryV2 of TogglCredentials
    | ExtendUpToDateV1 of FullCredentials
    | ExtendUpToDateV2 of TogglCredentials

type VersionResponse = {
    Version : string
}

type TimeEntry = {
    Description : string option
    Duration : int
    Start : System.DateTime
    Id : int64
    ProjectId : int option
}

type LastTimeEntryResponse = TimeEntry       

type ExtendUpToDateResponse = TimeEntry