namespace FileHandling
{
    public class FileMover
    {
        public static void MoveFile(string sourceFilePath, string destinationFolder)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"Source file not found: {sourceFilePath}");
            }

            OutputWriter.EnsureDirectoryExists(destinationFolder);

            string fileName = Path.GetFileName(sourceFilePath);
            string destinationPath = Path.Combine(destinationFolder, fileName);

            if (File.Exists(destinationPath))
            {
                string timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                string extension = Path.GetExtension(fileName);
                fileName = $"{fileNameWithoutExt}_{timestamp}{extension}";
                destinationPath = Path.Combine(destinationFolder, fileName);
            }

            File.Move(sourceFilePath, destinationPath);
        }

        public static void CopyFile(string sourceFilePath, string destinationFolder)
        {
            if (!File.Exists(sourceFilePath))
            {
                throw new FileNotFoundException($"Source file not found: {sourceFilePath}");
            }

            OutputWriter.EnsureDirectoryExists(destinationFolder);

            string fileName = Path.GetFileName(sourceFilePath);
            string destinationPath = Path.Combine(destinationFolder, fileName);

            File.Copy(sourceFilePath, destinationPath, overwrite: true);
        }

        public static string[] GetFiles(string directoryPath, string searchPattern = "*.*")
        {
            if (!Directory.Exists(directoryPath))
            {
                OutputWriter.EnsureDirectoryExists(directoryPath);
                return new string[0];
            }

            return Directory.GetFiles(directoryPath, searchPattern);
        }

        public static void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

}
