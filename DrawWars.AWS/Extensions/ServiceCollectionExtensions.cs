using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DrawWars.Aws.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void UseAwsManager(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection("AwsSettings");
            var config = section.Get<AwsSettings>();

            AwsManager.AccessId = config.AwsAccessID;
            AwsManager.Bucket = config.S3Bucket;
            AwsManager.Password = config.AwsPrivateKey;
            AwsManager.CloudFront = config.CloudFrontUrl;
        }
    }
}
