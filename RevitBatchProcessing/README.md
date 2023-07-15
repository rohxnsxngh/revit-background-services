# Revit Batch Processing

This repository contains a batch file used for batch processing of Revit models in the background services. The batch file is called by background services and runs as a service for individual models.

## table of contents

## Table of Contents
- [Introduction](#introduction)
- [Usage](#usage)

## Introduction

The **Revit Batch Processing** batch file is designed to run in the background services and perform batch processing tasks on Revit models. It is specifically created to be executed by background services and operates as a service for individual models.

## Usage

To use this batch file, follow these steps:

1. Ensure that you have the required dependencies installed:
    - Revit Batch Processor: 
        - **%LOCALAPPDATA%\RevitBatchProcessor\BatchRvt.exe**

2. Place the batch file in the desired directory.

3. Modify the batch file if necessary:
    - Set the **maxIterations** variable to define the maximum number of iterations for the repository search loop.
    - Set the **repoPath** variable to the path of the repository where the batch file is located.
    - Customize the paths for the required files, such as **settingsFile**, **revitFileList**, **loggingFile**, and **SyncDatabase**.

4. Run the batch file by executing it.

