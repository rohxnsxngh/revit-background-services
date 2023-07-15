@echo off

rem the path below will give you all the RBP commands
rem %LOCALAPPDATA%\RevitBatchProcessor\BatchRvt.exe --help

setlocal enabledelayedexpansion

set maxIterations=100
set counter=0
rem Loop till the repo path is found
:loopRepoSearch
cd..
if "%CD%"=="%CD:\revit-background-services\=%" (
    echo Reached the "revit-background-services" directory!
    goto endRepoSearch
)

set /a counter+=1
if !counter! == %maxIterations% (
    echo Could not find the "revit-background-services" directory within the limit of %maxIterations% iterations.
    goto endRepoSearch
    exit /b
)

goto loopRepoSearch

:endRepoSearch

rem Check if an argument is provided
if "%~1"=="" (
   echo No argument provided. Please specify a file path.
   exit /b
)

rem Define the paths for the required files
rem set "settingsFile=BatchRvt.Settings.json"
rem set "revitFileList=RevitFileList.txt"
rem set "loggingFile=loggingBatchProcessor"
rem set "SyncDatabase=SyncDatabase.py"

REM Set the repository path to one directory above the current location
set "repoPath=%CD%"
set "batchPath=%repoPath:~0%\RevitBatchProcessing\"
set "pathtoDLL=%repoPath:~0%\RevitAPIModelParsing\revit_api_parse_model\bin\Debug\net48\revit_api_parse_model.dll"

echo %batchPath% 

set "revitModelName=%~1"

set "settingsFile=%batchPath%%revitModelName%\BatchRvt.Settings.json"
set "revitFileList=%batchPath%%revitModelName%\%revitModelName%.txt"
set "loggingFile=%batchPath%%revitModelName%\loggingBatchProcessor"
set "SyncDatabase=%batchPath%SyncDatabase.py"

rem Check if the revit_api_parse_model.dll exist
if not exist "%pathtoDLL%" (
   echo Settings file "%pathtoDLL%" doesn't exist.
   exit /b
)

rem if the revit_api_parse_model.dll exists copy to the batchPath directory
if exist "%pathtoDLL%" (
   echo Settings file "%pathtoDLL%" exists.
   copy "%pathtoDLL%" "%batchPath%"
)


Check if the settings file exists
if not exist "%settingsFile%" (
   echo Settings file "%settingsFile%" doesn't exist.
   echo. > "%settingsFile%"
   echo Warning: Settings file "%settingsFile%" was not found and has been created.
)

rem Check if the revitFileList exists
if not exist "%revitFileList%" (
   echo revitFileList file "%revitFileList%" doesn't exist.
   exit /b
)

rem Check if the loggingFile exists
if not exist "%loggingFile%" (
   echo loggingFile folder "%loggingFile%" doesn't exist. Creating one...
   mkdir "%loggingFile%"
   echo Warning: loggingFile folder "%loggingFile%" was not found and has been created.
)

rem Check if the TaskFile exists
if not exist "%SyncDatabase%" (
   echo SyncDatabase.py file "%SyncDatabase%" doesn't exist.
   exit /b
)

rem Running Batch File --settings_file %settingsFile% 
rem set start_time=%time%
%LOCALAPPDATA%\RevitBatchProcessor\BatchRvt.exe --settings_file %settingsFile% --task_script %SyncDatabase% --file_list %revitFileList% --log_folder %loggingFile% --per_file_timeout 120 --revit_version 2023 --worksets close_all
