using BackgroundServices.Services;
using BackgroundServices.Models;
using Hangfire.MemoryStorage;
using Hangfire;
using Hangfire.Storage;
using Hangfire.Storage.SQLite;
using Hangfire.JobsLogger;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using DotNetEnv;

internal class Program
{
    private static IConfiguration? Configuration { get; set; }

    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        Configuration = builder.Configuration;

        ConfigureServices(builder.Services);

        builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
        builder.Services.AddAWSService<IAmazonS3>();

        var app = builder.Build();

        Configure(app, app.Environment);

        app.Run();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.AddHangfire(config =>
        {
            config.UseSimpleAssemblyNameTypeSerializer();
            config.UseRecommendedSerializerSettings();
            config.UseMemoryStorage(new MemoryStorageOptions { FetchNextJobTimeout = TimeSpan.FromHours(12) });
            config.UseSQLiteStorage(Configuration?.GetConnectionString("DefaultConnection"), new SQLiteStorageOptions
            {
                AutoVacuumSelected = SQLiteStorageOptions.AutoVacuum.FULL,
                JobExpirationCheckInterval = TimeSpan.FromMinutes(10),
                InvisibilityTimeout = TimeSpan.FromHours(3),
            });
            config.UseJobsLogger();
        });

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder.WithOrigins("http://localhost:3000")
                    .AllowAnyHeader()
                    .AllowAnyMethod();

                builder.WithOrigins("https://fdm-devaustin.tesla.com/")
                    .AllowAnyHeader()
                    .AllowAnyMethod();

                builder.WithOrigins("https://fdm-dev.tesla.com/")
                    .AllowAnyHeader()
                    .AllowAnyMethod();

            });
        });

        services.AddHangfireServer();
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

        services.AddTransient<IServiceManagement, ServiceManagement>();
    }

    private static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Load environment variables from .env file
        DotNetEnv.Env.Load();

        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseRouting();

        app.UseCors();

        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.UseHangfireDashboard("/hangfire", new DashboardOptions
        {
            DashboardTitle = "Background Services Dashboard",
            Authorization = new[]
            {
                new HangfireCustomBasicAuthenticationFilter
                {
                    Pass = Environment.GetEnvironmentVariable("HANGFIRE_PASSWORD"),
                    User = Environment.GetEnvironmentVariable("HANGFIRE_USERNAME")

                }
            }
        });

        var options = new RecurringJobOptions
        {
            TimeZone = TimeZoneInfo.Local
        };

        // Create a cancellation token source
        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Syncs All Revit Model Elements
        RecurringJob.AddOrUpdate<IServiceManagement>("sync-database",
            x => x.ServiceDatabase(GetRevitModelNames(), GetUserName(), cancellationToken),
            cronExpression: "0 0 * * *",
            options);

        // Basic Factory Test File Development Test
        RecurringJob.AddOrUpdate<IServiceManagement>("sync-database-dev-test",
            x => x.ServiceDatabase(GetRevitTestModel(), GetUserName(), cancellationToken),
            cronExpression: "0 0 1 1 *",
            options);

        // Syncs Master Revit File and checks Revit Batch Processing folder structure to match models
        RecurringJob.AddOrUpdate<IServiceManagement>("sync-revit-master-files-with-revit-batch-processsing-file-structure",
            x => x.SyncRevitMasterFilesWithRevitBatchProcesssingStructure(cancellationToken),
            cronExpression: "0 0 1 */3 *",
            options);

        // Delete Logging Files every 3 months
        RecurringJob.AddOrUpdate<IServiceManagement>("delete-logging-files",
            x => x.DeleteLocalLoggingFile(cancellationToken),
            cronExpression: "0 0 1 */3 *",
            options);

        // Delete Logging Files every 3 months
        RecurringJob.AddOrUpdate<IServiceManagement>("hangfire-sqlite-pipeline-service",
            x => x.HangfireServiceSQLiteTracking(cancellationToken),
            cronExpression: "0 0 1 */3 *",
            options);
    }

    private static List<string> GetRevitModelNames()
    {
        return new List<string>
        {
            "AUS_00_ALL_00_QUARK_FLD_TSLA",
            "BASIC_FACTORY_TEST_FILE",
            "AUS_B1_BIW_00_FLD_TSLA",
            "AUS_B1_COAT_00_FLD_TSLA",
            "AUS_B1_CP_00_FLD_TSLA",
            "AUS_B1_GA_00_FLD_TSLA",
            "AUS_B1_PL1_00_FLD_TSLA",
            "AUS_B1_SBP_00_FLD_TSLA",
            "AUS_B2_DU1_00_FLD_TSLA",
            "AUS_B2_HB1_00_FLD_TSLA",
            "AUS_B2_PT1_00_FLD_TSLA",
            "AUS_B2_ST2_00_FLD_TSLA",
            "AUS_B3_BIW_00_FLD_TSLA",
            "AUS_B3_CA_00_FLD_TSLA",
            "AUS_B3_ST_00_FLD_TSLA",
            "GF01_CELL_EXP_FLD_TSLA",
            "GF01_SEMI_EXP_FLD_TSLA",
            // "AUS_SX_SX1_00_FLD_TSLA",
            // "NV91_ALL_00_FLD_TSLA",
            // "TSLA_STRC_GRID_V23",
            // "Giga_Texas_GRID"
            // Add more models here if needed
            // need to eventually pass this list from revit_master
        };
    }

    private static List<string> GetRevitTestModel()
    {
        return new List<string>
        {
            "BASIC_FACTORY_TEST_FILE",
        };
    }

    private static string GetUserName()
    {
        // Logic to retrieve the username
        return "";
    }
}
