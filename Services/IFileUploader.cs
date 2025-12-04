namespace Services
{
    public interface IFileUploader
    {
        string UploadFile(string filePath);
        bool TestConnection();
    }
}
