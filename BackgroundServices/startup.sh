#!/bin/bash

# Get the path of the directory where the script resides
SCRIPT_DIR=$(dirname "$0")

# Navigate to the repository directory
REPO_DIR="$SCRIPT_DIR/revit-background-services"
cd "$REPO_DIR/RevitAPIModelParsing"

# Build and .dll and revit extraction
dotnet build


cd "$REPO_DIR/RevitAPIModelParsing"

# Build and run your service
dotnet build
dotnet run