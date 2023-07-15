using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using BackgroundServices.Utility;
using System.Diagnostics;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace BackgroundServices.Migrations;

public class AWSS3MigrationService
{
    public async Task UploadObjectToS3Bucket(string fileName, Stream fileStream, string modelName)
    {
        try
        {
            await S3BucketManager.GetBucketInformation(fileName, fileStream, modelName);
            Console.WriteLine(" UPLOADED OBJECTS ");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while uploading object to S3: {ex.Message}");
            throw;
        }
    }

    public static Stream GetFileStream(string filePath)
    {
        // Open the file in read mode and return the file stream
        return File.OpenRead(filePath);
    }

}

