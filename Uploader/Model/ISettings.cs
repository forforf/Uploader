namespace Uploader
{
    public interface ISettings
    {
        string WatchPath { get; set; }
        string S3Path { get; set; }
    }
}
