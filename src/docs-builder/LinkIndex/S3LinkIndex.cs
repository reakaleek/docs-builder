// Licensed to Elasticsearch B.V under one or more agreements.
// Elasticsearch B.V licenses this file to you under the Apache 2.0 License.
// See the LICENSE file in the project root for more information
using Amazon.S3;
using Amazon.S3.Transfer;
using Microsoft.Extensions.Logging;

namespace Documentation.Builder.LinkIndex;

public class S3LinkIndex : ILinkIndex
{
	private readonly string _bucketName;
	private readonly ILogger<S3LinkIndex> _logger;

	public S3LinkIndex(string bucketName, ILoggerFactory loggerFactory)
	{
		_bucketName = bucketName;
		_logger = loggerFactory.CreateLogger<S3LinkIndex>();
	}

	public async Task UploadFileAsync(string filePath, bool shouldUpload)
	{
		if (!shouldUpload)
		{
			_logger.LogInformation("Not uploading link index: skip flag is set");
			return;
		}

		var githubRef = Environment.GetEnvironmentVariable("GITHUB_REF");
		if (string.IsNullOrEmpty(githubRef) || githubRef != "refs/heads/main")
		{
			throw new InvalidOperationException($"Cannot upload link index: GITHUB_REF '{githubRef}' is not main branch");
		}

		var s3DestinationPath = DeriveDestinationPath();

		_logger.LogInformation("Uploading link index {FilePath} to S3://{Bucket}/{DestinationPath}", filePath, _bucketName, s3DestinationPath);
		using var client = new AmazonS3Client();
		var fileTransferUtility = new TransferUtility(client);
		try
		{
			await fileTransferUtility.UploadAsync(filePath, _bucketName, s3DestinationPath);
			_logger.LogInformation("Successfully uploaded link reference {FilePath} to S3://{Bucket}/{DestinationPath}",
				filePath, _bucketName, s3DestinationPath);
		}
		catch (Exception e)
		{
			_logger.LogError(e, "Failed to upload link index {FilePath} to S3://{Bucket}/{DestinationPath}",
				filePath, _bucketName, s3DestinationPath);
			throw;
		}
	}

	private static string DeriveDestinationPath()
	{
		var repositoryName = Environment.GetEnvironmentVariable("GITHUB_REPOSITORY")?.Split('/').Last();
		if (string.IsNullOrEmpty(repositoryName))
			throw new InvalidOperationException("Cannot upload link index: GITHUB_REPOSITORY environment variable is not set or invalid");
		return $"{repositoryName}.json";
	}
}
