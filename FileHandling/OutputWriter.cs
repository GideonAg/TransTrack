using Models;
using System.Text;

namespace FileHandling
{
    public class OutputWriter
    {
        public static void WriteProcessedFile(string outputPath, ProcessingSummary summary)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"ProcessedFile: {summary.FileName}");
            sb.AppendLine($"TotalRecords: {summary.TotalRecords}");
            sb.AppendLine($"FileUrl:");
            sb.AppendLine(summary.FileUrl);
            sb.AppendLine($"DateProcessed: {summary.ProcessedDate:yyyy-MM-dd HH:mm:ss}");

            File.WriteAllText(outputPath, sb.ToString());
        }

        public static void WriteErrorFile(string outputPath, string fileName, string reason)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"FileName: {fileName}");
            sb.AppendLine($"Reason: {reason}");
            sb.AppendLine($"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

            File.WriteAllText(outputPath, sb.ToString());
        }

        public static void WriteTextFile(string filePath, string content)
        {
            File.WriteAllText(filePath, content);
        }

        public static void EnsureDirectoryExists(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }
    }
}
