using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using System;
using System.IO;

namespace DrawWars.Aws
{
    public class AwsManager : IAwsManager
    {
        private RegionEndpoint _region;

        public static string AccessId { get; set; }
        public static string Password { get; set; }

        public static string Bucket { get; set; }
        public static string CloudFront { get; set; }
        
        public AwsManager()
        {
            _region = RegionEndpoint.EUWest1;
        }

        public string S3_UploadFile(string fileName, byte[] file)
        {
            try
            {
                using (var s3Client = new AmazonS3Client(AccessId, Password, _region))
                using (var tu = new TransferUtility(s3Client))
                using (var stream = new MemoryStream(file))
                {
                    tu.Upload(stream, Bucket, fileName);
                    return $"http://{CloudFront}/{fileName}";
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
    }
}

