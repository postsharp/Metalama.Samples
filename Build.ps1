(& dotnet nuget locals http-cache -c) | Out-Null
& dotnet run --project "$PSScriptRoot\eng\src\BuildMetalamaSamples.csproj" -- $args
exit $LASTEXITCODE

