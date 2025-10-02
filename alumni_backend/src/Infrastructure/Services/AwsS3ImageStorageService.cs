using Amazon.S3;
using Amazon.S3.Model;
using Application.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class AwsS3ImageStorageService : IImageStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AwsS3ImageStorageService> _logger;
    private readonly string _bucketName;
    private readonly string _baseUrl;

    private readonly string[] _allowedImageTypes = {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/bmp"
    };

    public AwsS3ImageStorageService(
        IAmazonS3 s3Client,
        IConfiguration configuration,
        ILogger<AwsS3ImageStorageService> logger)
    {
        _s3Client = s3Client;
        _configuration = configuration;
        _logger = logger;
        _bucketName = _configuration["AWS:S3:BucketName"] ?? throw new InvalidOperationException("AWS S3 BucketName not configured");
        
        var region = _configuration["AWS:S3:Region"] ?? "us-west-2";
        _baseUrl = $"https://{_bucketName}.s3.{region}.amazonaws.com";
    }

    public async Task<string> UploadImageAsync(Stream imageStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!IsValidImageType(contentType))
            {
                throw new ArgumentException($"Invalid image type: {contentType}");
            }

            var uniqueFileName = GenerateUniqueFileName(fileName);
            var key = $"images/{uniqueFileName}";

            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = key,
                InputStream = imageStream,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead,
                Metadata =
                {
                    ["uploaded-by"] = "alumni-api",
                    ["uploaded-at"] = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")
                }
            };

            var response = await _s3Client.PutObjectAsync(request, cancellationToken);
            
            if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
            {
                var imageUrl = $"{_baseUrl}/{key}";
                _logger.LogInformation("Successfully uploaded image to S3: {ImageUrl}", imageUrl);
                return imageUrl;
            }
            
            throw new InvalidOperationException($"Failed to upload image to S3. HTTP Status: {response.HttpStatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading image to S3: {FileName}", fileName);
            throw;
        }
    }

    public async Task<string> UploadImageAsync(byte[] imageBytes, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(imageBytes);
        return await UploadImageAsync(stream, fileName, contentType, cancellationToken);
    }

    public async Task<bool> DeleteImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith(_baseUrl))
            {
                _logger.LogWarning("Invalid image URL for deletion: {ImageUrl}", imageUrl);
                return false;
            }

            var key = imageUrl.Replace($"{_baseUrl}/", "");
            
            var request = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            var response = await _s3Client.DeleteObjectAsync(request, cancellationToken);
            
            if (response.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
            {
                _logger.LogInformation("Successfully deleted image from S3: {ImageUrl}", imageUrl);
                return true;
            }
            
            _logger.LogWarning("Failed to delete image from S3: {ImageUrl}. HTTP Status: {StatusCode}", imageUrl, response.HttpStatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting image from S3: {ImageUrl}", imageUrl);
            return false;
        }
    }

    public Task<string> GeneratePresignedUploadUrlAsync(string fileName, string contentType, int expirationMinutes = 15, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!IsValidImageType(contentType))
            {
                throw new ArgumentException($"Invalid image type: {contentType}");
            }

            var uniqueFileName = GenerateUniqueFileName(fileName);
            var key = $"images/{uniqueFileName}";

            var request = new GetPreSignedUrlRequest
            {
                BucketName = _bucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
                ContentType = contentType
            };

            var presignedUrl = _s3Client.GetPreSignedURL(request);
            _logger.LogInformation("Generated presigned URL for image upload: {Key}", key);
            
            return Task.FromResult(presignedUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating presigned URL for image: {FileName}", fileName);
            throw;
        }
    }

    public bool IsValidImageType(string contentType)
    {
        return !string.IsNullOrEmpty(contentType) && 
               _allowedImageTypes.Contains(contentType.ToLowerInvariant());
    }

    public string GenerateUniqueFileName(string originalFileName, string prefix = "")
    {
        var extension = Path.GetExtension(originalFileName);
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var guid = Guid.NewGuid().ToString("N")[..8];
        
        if (!string.IsNullOrEmpty(prefix))
        {
            return $"{prefix}_{timestamp}_{guid}{extension}";
        }
        
        return $"{timestamp}_{guid}{extension}";
    }
}