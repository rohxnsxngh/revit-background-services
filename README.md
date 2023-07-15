# Revit and GigaGraph Background Services

This project is a .NET Core Web API that implements a cron job for running various Revit and GigaGraph services using Hangfire for cron scheduling, server, and dashboards.

## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Configuration](#configuration)
- [Usage](#usage)
- [BIM360 Access](#BIM360-Access)
- [Contributing](#contributing)
- [Additional Developer Information](#additionaldeveloperinformation)

## Overview

The Revit and GigaGraph Services Cron Job is designed to automate and schedule tasks related to Revit and GigaGraph services. It provides a convenient way to execute tasks at specified intervals using cron expressions.

This project utilizes the following technologies:

- .NET Core: A cross-platform framework for building web applications.
- Hangfire: A library for managing background jobs and scheduling tasks.
- Hangfire Dashboard: A user interface for monitoring and managing Hangfire jobs.
- AWS Buckets: Storage for error logging and additional information.
- Revit 2023: Install Revit 2023
- BIM360: Models are hosted on BIM360 so BIM360 access is necessary to run this application
- Revit Batch Processor 2023: Revit Batch Processor v1.9.0 (https://github.com/bvn-architecture/RevitBatchProcessor)

## Prerequisites

Before getting started, ensure that you have the following installed on your system:

- .NET Core SDK: [Download and install .NET Core SDK](https://dotnet.microsoft.com/download)
- Visual Studio Code or Visual Studio: [Download and install Visual Studio Code](https://code.visualstudio.com) or [Download and install Visual Studio](https://visualstudio.microsoft.com/downloads/)

## Installation

1. Clone this repository to your local machine:

```
git clone https://github.com/your-username/revit-background-services.git
```

2. Open the project in your preferred IDE (Visual Studio Code or Visual Studio).

3. Build the project to restore dependencies. Build both BackgroundServices and RevitAPIModelParsing:

```bash
dotnet build
```

4. Verify that the project builds successfully.

5. Run the project from the BackgroundServices directory

```bash
dotnet run
```

## Configuration

The project requires some configuration to connect to the necessary services and set up the cron job schedules. Follow the steps below to configure the project:

1. Place a .env file in the BackgroundServices directory. this information can be acquired from Christian (csasam@tesla.com) or Forest (flam@tesla.com).

2. Modify the connection strings, API keys, and other configuration settings to match your environment.

```

# AWS S3 Bucket Information and Hangfire Dashboard Information

AWS_ACCESS_KEY_ID=############################
AWS_SECRET_ACCESS_KEY=########################
HANGFIRE_USERNAME=############################
HANGFIRE_PASSWORD=############################

```

3. Save the changes to the `.env` file.

## Usage

To start the cron job and access the Hangfire dashboard, follow these steps:

1. Open a terminal or command prompt and navigate to the project directory.

2. Run the following command in the BackgroundServices directory to start the cron job server:

```
dotnet build
dotnet run
```

3. Once the server is running, you can access the Hangfire dashboard by opening your web browser and navigating to:

```
http://localhost:<PORT>/hangfire
```

Replace `<PORT>` with the port number configured in your application.

4. Login to the Hangfire dashboard using the username and password specified in the `appsettings.json` file.

5. From the Hangfire dashboard, you can monitor and manage the scheduled tasks.

6. Customize the cron job schedules and task logic according to your requirements by modifying the code in the project files.

## BIM360-Access

The BIM360-Access section provides detailed information on how to gain access to the BIM360 cloud, which houses various models and data related to the project. This section outlines the steps and contacts necessary to obtain the required access permissions.

### Onboarding Quiz

Before requesting access to the BIM360 cloud, it is essential to complete the BIM onboarding quiz located on Confluence. This quiz serves as an initial assessment to ensure that individuals have a basic understanding of BIM principles and workflows. Please make sure to complete the quiz before proceeding with the access request.

### GFTX Access

For onboarding, evaluation, and addition to the GFTX project, Nick Kalinski (nkalinski@tesla.com) from the GFTX team is the designated point of contact. If you require access to the GFTX project within the BIM360 cloud, please reach out to Nick Kalinski for assistance. He will guide you through the necessary steps and provide the required access permissions.

### GFNV/Basic Factory Access

To gain access to GFNV and Basic Factory within the BIM360 cloud, Ravi Wood (ravwood@tesla.com) is the primary contact person. While Ravi Wood may not be directly responsible for granting access, he can help facilitate communication and connect you with the appropriate person or team. Reach out to Ravi Wood for assistance in obtaining access to GFNV and Basic Factory.

### Quark Access

Quark is a secure project within the BIM360 cloud, and access to it requires addition by a Project Administrator. To request access to Quark, please get in touch with Christine Kandigian (chkandigian@tesla.com), the designated Project Admin. Christine will handle the access request process for Quark and ensure that the appropriate permissions are granted based on your role and responsibilities.

## Contributing

- Rohan Singh (rohasingh@tesla.com)
- Forest Lam (flam@tesla.com)

<a id="additionaldeveloperinformation"></a>
## Additional Developer Information

- Background Services: [documentation](BackgroundServices/README.md)
- Revit API Extraction Model and Parsing: [documentation](RevitAPIModelParsing/README.md)
- Revit Batch Processing Structure: [documentation](RevitBatchProcessing/README.md)
