# Background Services

This project demonstrates the implementation of background services using Hangfire in a .NET application.

## Prerequisites

- .NET SDK (version X.X.X)
- SQLite database (for local server storage)

## Installation

1. Clone the repository:

```shell
git clone https://github.com/your-username/repository.git
```

2. Navigate to the BackgroundServices directory:

```
cd BackgroundServices
```

3. Restore the dependencies and build the project:

```
dotnet restore
dotnet build
```

4. Set up Hangfire authentication:

- Set the environment variables HANGFIRE_USERNAME and HANGFIRE_PASSWORD with your desired username and password for Hangfire authentication.

5. Run the application:

```
dotnet run
```

## Adding Services

To add a recurring service, follow these steps:

1. Create a new class that implements the IServiceManagement interface. This class will contain the logic for your recurring service.

```csharp
public class ServiceManagement : IServiceManagement
{
    // Implement the required methods and logic for your recurring service
}
```

2. Open the **Program.cs** file and navigate to the **ConfigureServices** method. Add the registration for your service:

```csharp
services.AddTransient<IServiceManagement, ServiceManagement>();
```

3. Open the **Program.cs** file and navigate to the Configure method. Locate the block of code starting with **RecurringJob.AddOrUpdate**.

- You can modify the existing recurring jobs or add new ones according to your requirements. Each recurring job is defined by a unique identifier, a method call, a cron expression, and optional configuration options.
- Modify the method calls to use the methods and parameters specific to your **ServiceManagement** class.

## AWS Error Logging

You can enable, disable, or modify error logging methods using the **S3BucketManager** class:

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using DotNetEnv;

public class S3BucketManager
{
    // ... (The rest of the code goes here)
}
```

To use the error logging functionality:

1. Create an instance of the **S3BucketManager** class.

2. Call the **GetBucketInformation** method, passing the required parameters: **fileName**, **fileStream**, and **modelName**.
3. The method will handle the error logging process and upload the error logs to the specified S3 bucket.

Make sure to set up the necessary environment variables for AWS access (**AWS_ACCESS_KEY_ID** and **AWS_SECRET_ACCESS_KEY**) in order to use the Amazon S3 service.
