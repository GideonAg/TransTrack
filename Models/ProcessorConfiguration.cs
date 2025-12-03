namespace Models
{
    public class ProcessorConfiguration
    {
        public string InputFolder { get; set; }
        public string ProcessedFolder { get; set; }
        public string ErrorsFolder { get; set; }
        public string CloudinaryCloudName { get; set; }
        public string CloudinaryApiKey { get; set; }
        public string CloudinaryApiSecret { get; set; }

        public ProcessorConfiguration()
        {
            InputFolder = string.Empty;
            ProcessedFolder = string.Empty;
            ErrorsFolder = string.Empty;
            CloudinaryCloudName = string.Empty;
            CloudinaryApiKey = string.Empty;
            CloudinaryApiSecret = string.Empty;
        }
    }
}
