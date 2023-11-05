
let src =  System.IO.Path.Combine(__SOURCE_DIRECTORY__, "..")
let published = System.IO.Path.Combine(src, """bin\Release\net6.0\publish""")
let zip = System.IO.Path.ChangeExtension(published, "zip")

System.Diagnostics.ProcessStartInfo(
    "dotnet"
    , $"""publish --configuration Release --version-suffix {System.DateTime.UtcNow.ToString "yyyyMMdd-HHmmss"}"""
    , WorkingDirectory = src
)
|> System.Diagnostics.Process.Start
|> fun p -> p.WaitForExit()


if System.IO.File.Exists zip then
    System.IO.File.Delete zip

System.IO.Compression.ZipFile.CreateFromDirectory(published, zip)
