namespace DrawWars.Aws
{
    public interface IAwsManager
    {
        string S3_UploadFile(string fileName, byte[] file);
    }
}
