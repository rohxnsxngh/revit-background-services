@echo off

REM Set the repository path to one directory above the current location
set "repoPath=%CD%"
set "revitAPIModelParsingPath=%repoPath:~0%\RevitAPIModelParsing\"
set "backgroundServicesPath=%repoPath:~0%\BackgroundServices\"

cd %revitAPIModelParsingPath%

REM build .dll in the RevitAPIModelParsing directory
dotnet build

cd %backgroundServicesPath%

# Build and run the service
dotnet build
dotnet run