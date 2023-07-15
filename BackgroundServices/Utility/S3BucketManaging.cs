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
    private static IAmazonS3? _s3Client;
    private const string BucketName = "materialflow-sg";
    private const string folderName = "revit_background_services_error_logging";

    public static async Task GetBucketInformation(string fileName, Stream fileStream, string modelName)
    {
        // Load environment variables from .env file
        DotNetEnv.Env.Load();

        var FileName = fileName;
        var FileStream = fileStream;
        var ModelName = modelName;

        string? accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
        if (accessKey == null)
        {
            Console.WriteLine("Error: AWS_ACCESS_KEY_ID environment variable is not set.");
            Console.WriteLine("Please ensure that the access key is properly configured.");
            // You can choose to terminate the application or take any other necessary action here.
            Environment.Exit(1); // Terminate the application with an error code
        }
        string? secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
        if (secretKey == null)
        {
            Console.WriteLine("Error: AWS_SECRET_ACCESS_KEY environment variable is not set.");
            Console.WriteLine("Please ensure that the access key is properly configured.");
            // You can choose to terminate the application or take any other necessary action here.
            Environment.Exit(1); // Terminate the application with an error code
        }

        AmazonS3Config config = new AmazonS3Config
        {
            ServiceURL = "https://s3.a.iad08.tcs.tesla.com",
            ForcePathStyle = true,
            DisableLogging = true,
            DisableHostPrefixInjection = true,
            UseHttp = true,
            UseDualstackEndpoint = false,
        };

        _s3Client = new AmazonS3Client(
            accessKey,
            secretKey,
            config
        );

        var response = await GetBuckets(_s3Client);
        DisplayBucketList(response.Buckets);


        var folderExists = await CheckAndAddSubfolderAsync(_s3Client, BucketName, folderName, ModelName);

        Console.WriteLine(folderExists);

        if (folderExists)
        {
            await UploadFileToS3FolderAsync(_s3Client, BucketName, folderName, modelName, FileName, FileStream);
        }

    }

    private static async Task TestAmazonS3BucketConnection(AmazonS3Client s3Client)
    {
        try
        {
            ListBucketsResponse response = await s3Client.ListBucketsAsync();
            foreach (S3Bucket bucket in response.Buckets)
            {
                Console.WriteLine($"Bucket Name: {bucket.BucketName}, Creation Date: {bucket.CreationDate}");
            }
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"An error occurred while testing connection: {ex.Message}");
        }
    }

    public static async Task<ListBucketsResponse> GetBuckets(IAmazonS3 client)
    {
        return await client.ListBucketsAsync();
    }

    public static void DisplayBucketList(List<S3Bucket> bucketList)
    {
        bucketList.ForEach(b => Console.WriteLine($"Bucket name: {b.BucketName}, created on: {b.CreationDate}"));
    }

    /// <summary>
    /// This method uses a paginator to retrieve the list of objects in an
    /// an Amazon S3 bucket.
    /// </summary>
    /// <param name="client">An Amazon S3 client object.</param>
    /// <param name="bucketName">The name of the S3 bucket whose objects
    /// you want to list.</param>
    public static async Task ListingObjectsAsync(IAmazonS3 client, string bucketName)
    {
        var listObjectsV2Paginator = client.Paginators.ListObjectsV2(new ListObjectsV2Request
        {
            BucketName = bucketName,
        });

        await foreach (var response in listObjectsV2Paginator.Responses)
        {
            Console.WriteLine($"HttpStatusCode: {response.HttpStatusCode}");
            Console.WriteLine($"Number of Keys: {response.KeyCount}");
            foreach (var entry in response.S3Objects)
            {
                Console.WriteLine($"Key = {entry.Key} Size = {entry.Size}");
            }
        }
    }

    /// <summary>
    /// Shows how to list the objects in an Amazon S3 bucket.
    /// </summary>
    /// <param name="client">An initialized Amazon S3 client object.</param>
    /// <param name="bucketName">The name of the bucket for which to list
    /// the contents.</param>
    /// <returns>A boolean value indicating the success or failure of the
    /// copy operation.</returns>
    public static async Task<bool> ListBucketContentsAsync(IAmazonS3 client, string BucketName)
    {
        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = BucketName,
                MaxKeys = 5,
            };

            ListObjectsV2Response response;

            do
            {
                response = await client.ListObjectsV2Async(request);

                response.S3Objects
                    .ForEach(obj => Console.WriteLine($"{obj.Key,-35}{obj.LastModified.ToShortDateString(),10}{obj.Size,10}"));

                // If the response is truncated, set the request ContinuationToken
                // from the NextContinuationToken property of the response.
                request.ContinuationToken = response.NextContinuationToken;
            }
            while (response.IsTruncated);

            return true;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error encountered on server. Message:'{ex.Message}' getting list of objects.");
            return false;
        }
    }


    /// <summary>
    /// Checks for parent folder, if it does not exist creates parent folder
    /// </summary>
    /// <param name="client">An initialized Amazon S3 client object.</param>
    /// <param name="bucketName">The name of the bucket for which to list
    /// the contents.</param>
    /// <param name="folderName"> The name of the parent folder. </param>
    /// <returns>A boolean value indicating the success or failure of the
    /// copy operation.</returns>
    public static async Task<bool> CheckAndAddFolderAsync(IAmazonS3 client, string bucketName, string folderName)
    {
        try
        {
            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = folderName + "/",
                MaxKeys = 1
            };

            var response = await client.ListObjectsV2Async(request);

            bool folderExists = response.KeyCount > 0;

            if (!folderExists)
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = folderName + "/"
                };

                var putResponse = await client.PutObjectAsync(putRequest);

                if (putResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
                {
                    Console.WriteLine($"Folder '{folderName}' created successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to create folder '{folderName}'.");
                    return false;
                }
            }

            foreach (var obj in response.S3Objects)
            {
                if (obj.Key == folderName + "/") // Check if the object is a folder
                {
                    Console.WriteLine($"{obj.Key} (Folder)");
                }
                else
                {
                    Console.WriteLine($"{obj.Key,-35}{obj.LastModified.ToShortDateString(),10}{obj.Size,10}");
                }
            }

            return true;
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error encountered on server. Message: '{ex.Message}' while getting list of objects.");
            return false;
        }
    }

    /// <summary>
    /// Checks for sub folder based on model name, if it does not exist creates parent folder
    /// </summary>
    /// <param name="client">An initialized Amazon S3 client object.</param>
    /// <param name="bucketName">The name of the bucket for which to list
    /// the contents.</param>
    /// <param name="folderName"> The name of the parent folder. </param>
    /// <param name="subfolderName"> The name of the subfolder that retrieved based on the model name. </param>
    /// <returns>A boolean value indicating the success or failure of the
    /// copy operation.</returns>
    public static async Task<bool> CheckAndAddSubfolderAsync(IAmazonS3 client, string bucketName, string parentFolderName, string subfolderName)
    {
        try
        {
            string folderName = string.IsNullOrEmpty(parentFolderName) ? subfolderName : $"{parentFolderName}/{subfolderName}";

            bool parentFolderExists = await CheckAndAddFolderAsync(client, bucketName, parentFolderName);

            var request = new ListObjectsV2Request
            {
                BucketName = bucketName,
                Prefix = folderName + "/",
                MaxKeys = 1
            };

            var response = await client.ListObjectsV2Async(request);

            bool folderExists = response.KeyCount > 0;

            if (folderExists)
            {
                Console.WriteLine($"Subfolder '{subfolderName}' already exists under '{parentFolderName}'.");
                return true;
            }

            var putRequest = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = folderName + "/"
            };

            var putResponse = await client.PutObjectAsync(putRequest);

            if (putResponse.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"Subfolder '{subfolderName}' created successfully under '{parentFolderName}'.");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to create subfolder '{subfolderName}' under '{parentFolderName}'.");
                return false;
            }
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error encountered on server. Message: '{ex.Message}' while getting list of objects.");
            return false;
        }
    }


    /// <summary>
    /// Uploads chosen file from filstream to chosen subfolder under parent folder in s3 bucket
    /// </summary>
    /// <param name="client">An initialized Amazon S3 client object.</param>
    /// <param name="bucketName">The name of the bucket for which to list
    /// the contents.</param>
    /// <param name="folderName"> The name of the parent folder. </param>
    /// <param name="subfolderName"> The name of the subfolder that retrieved based on the model name. </param>
    /// <param name="fileName"> The name of the file. </param>
    /// <param name="fileStream"> The stream from the file. </param>
    /// <returns>A boolean value indicating the success or failure of the
    /// copy operation.</returns>
    public static async Task<bool> UploadFileToS3FolderAsync(IAmazonS3 client, string bucketName, string folderName, string subfolderName, string fileName, Stream fileStream)
    {
        try
        {
            string objectKey;
            if (string.IsNullOrEmpty(subfolderName))
            {
                objectKey = string.IsNullOrEmpty(folderName) ? fileName : $"{folderName}/{fileName}";
            }
            else
            {
                objectKey = string.IsNullOrEmpty(folderName) ? $"{subfolderName}/{fileName}" : $"{folderName}/{subfolderName}/{fileName}";
            }
            Console.WriteLine(objectKey);

            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = objectKey,
                InputStream = fileStream
            };

            Console.WriteLine($"Uploading the file '{fileName}' to '{bucketName}/{folderName}/{subfolderName}'...");

            var response = await client.PutObjectAsync(request);

            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                Console.WriteLine($"File '{fileName}' uploaded to '{bucketName}/{folderName}/{subfolderName}' successfully.");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to upload file '{fileName}' to '{bucketName}/{folderName}/{subfolderName}'.");
                return false;
            }
        }
        catch (AmazonS3Exception ex)
        {
            Console.WriteLine($"Error encountered on server. Message: '{ex.Message}' while uploading the file.");
            return false;
        }
    }



}