using Amazon.S3.Transfer;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3.Model;
using data_aparta_.DTOs;

namespace data_aparta_.Repos.Utils
{
    public  class S3Uploader
    {
        private readonly IAmazonS3 _s3Client;
        private const string BucketName = "apartaplus-bucket";

        public S3Uploader(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        public async Task<FileUploadResponse> UploadFileAsync(FileUploadInput input)
        {
            if (input.File == null)
                throw new ArgumentException("File is required");

            // Definir las carpetas según el tipo
            string folder = input.Type switch
            {
                "contract" => "contracts",
                "cover" => "properties/covers",
                "property-image" => $"properties/{input.PropertyId}/images",
                _ => throw new ArgumentException("Invalid file type")
            };

            string extension = System.IO.Path.GetExtension(input.File.Name);
            string fileName = $"{Guid.NewGuid()}{extension}";
            string key = $"{folder}/{fileName}";

            using var stream = input.File.OpenReadStream();
            var putRequest = new PutObjectRequest
            {
                BucketName = BucketName,
                Key = key,
                InputStream = stream,
                ContentType = input.File.ContentType
            };

            await _s3Client.PutObjectAsync(putRequest);

            string url = $"https://{BucketName}.s3.amazonaws.com/{key}";

            return new FileUploadResponse
            {
                Url = url,
                Key = key
            };
        }
    }
}
