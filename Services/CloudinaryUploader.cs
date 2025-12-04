using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System;
using System.IO;
using System.Threading;

namespace Services
{
    public class CloudinaryUploader : IFileUploader
    {
        private readonly Cloudinary _cloudinary;
        private readonly string _cloudName;

        public CloudinaryUploader(string cloudName, string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(cloudName))
                throw new ArgumentException("Cloud name cannot be empty", nameof(cloudName));
            if (string.IsNullOrWhiteSpace(apiKey))
                throw new ArgumentException("API key cannot be empty", nameof(apiKey));
            if (string.IsNullOrWhiteSpace(apiSecret))
                throw new ArgumentException("API secret cannot be empty", nameof(apiSecret));

            _cloudName = cloudName;

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true; // Use HTTPS
        }

        public string UploadFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be empty", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}", filePath);
            }

            try
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string publicId = $"transtrack/{fileName}_{timestamp}";

                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(filePath),
                    PublicId = publicId,
                    UseFilename = false,
                    UniqueFilename = true,
                    Overwrite = false
                };

                var uploadResult = _cloudinary.Upload(uploadParams);

                if (uploadResult.Error != null)
                {
                    throw new InvalidOperationException(
                        $"Cloudinary upload failed: {uploadResult.Error.Message}");
                }

                return uploadResult.SecureUrl?.ToString() ?? uploadResult.Url?.ToString();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to upload file to Cloudinary: {ex.Message}", ex);
            }
        }

        public bool TestConnection()
        {
            try
            {
                var pingResult = _cloudinary.GetResource("test");
                return true;
            }
            catch
            {
                return true;
            }
        }
        
        public string CloudName => _cloudName;

        public class MockFileUploader : IFileUploader
        {
            private readonly Random _random = new Random();

            public string UploadFile(string filePath)
            {
                if (!File.Exists(filePath))
                {
                    throw new FileNotFoundException($"File not found: {filePath}", filePath);
                }

                Thread.Sleep(100);

                string fileName = Path.GetFileName(filePath);
                string mockId = _random.Next(1000, 9999).ToString();
                return $"https://res.cloudinary.com/demo/raw/upload/v{mockId}/transtrack/{fileName}";
            }

            public bool TestConnection()
            {
                return true;
            }
        }

        public static class UploaderFactory
        {
            public static IFileUploader CreateCloudinaryUploader(string cloudName, string apiKey, string apiSecret)
            {
                return new CloudinaryUploader(cloudName, apiKey, apiSecret);
            }

            public static IFileUploader CreateMockUploader()
            {
                return new MockFileUploader();
            }
        }

    }
}