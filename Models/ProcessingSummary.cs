using System;

namespace Models
{
    public class ProcessingSummary
    {
        public string FileName { get; set; }
        public int TotalRecords { get; set; }
        public int ValidRecords { get; set; }
        public int InvalidRecords { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string FileUrl { get; set; }
        public DateTime ProcessedDate { get; set; }

        public ProcessingSummary()
        {
            FileName = string.Empty;
            ErrorMessage = string.Empty;
            FileUrl = string.Empty;
            ProcessedDate = DateTime.Now;
        }

        public ProcessingSummary(string fileName)
        {
            FileName = fileName;
            ErrorMessage = string.Empty;
            FileUrl = string.Empty;
            ProcessedDate = DateTime.Now;
        }
    }
}
