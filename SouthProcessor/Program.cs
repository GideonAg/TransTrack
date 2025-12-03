using FileHandling;
using Logging;
using Models;
using Services;
using System.Configuration;
using TransTrack.Logging;
using Validation;
using static Services.CloudinaryUploader;

namespace SouthProcessor
{
    class Program
    {
        private static ILogger _logger;
        private static IFileUploader _uploader;
        private static ProcessorConfiguration _config;

        static void Main(string[] args)
        {
            try
            {
                InitializeComponents();

                _logger.LogInfo("South Processor started");

                LoadConfiguration();

                EnsureDirectoriesExist();

                ProcessFiles();

                _logger.LogInfo("South Processor completed successfully");
                Console.WriteLine("\n✓ Processing completed successfully!");
            }
            catch (Exception ex)
            {
                string errorMsg = $"Fatal error in South Processor: {ex.Message}";
                Console.WriteLine($"\n✗ ERROR: {errorMsg}");

                if (_logger != null)
                {
                    _logger.LogError(errorMsg, ex);
                }

                Environment.Exit(1);
            }
            Console.ReadKey();
        }

        private static void InitializeComponents()
        {
            _logger = new FileLogger();
            Console.WriteLine("✓ Logger initialized");

            string cloudName = ConfigurationManager.AppSettings["CloudinaryCloudName"]!;
            string apiKey = ConfigurationManager.AppSettings["CloudinaryApiKey"]!;
            string apiSecret = ConfigurationManager.AppSettings["CloudinaryApiSecret"]!;

            if (string.IsNullOrWhiteSpace(cloudName) ||
                string.IsNullOrWhiteSpace(apiKey) ||
                string.IsNullOrWhiteSpace(apiSecret))
            {
                _logger.LogWarning("Cloudinary credentials not configured. Using mock uploader.");
                Console.WriteLine("⚠ Warning: Using mock uploader (Cloudinary not configured)");
                _uploader = UploaderFactory.CreateMockUploader();
            }
            else
            {
                _uploader = UploaderFactory.CreateCloudinaryUploader(cloudName, apiKey, apiSecret);
                Console.WriteLine("✓ Cloudinary uploader initialized");
            }
        }

        private static void LoadConfiguration()
        {
            _config = new ProcessorConfiguration
            {
                InputFolder = ConfigurationManager.AppSettings["InputFolder"] ?? @"C:\TransTrack\South\Incoming",
                ProcessedFolder = ConfigurationManager.AppSettings["ProcessedFolder"] ?? @"C:\TransTrack\South\Processed",
                ErrorsFolder = ConfigurationManager.AppSettings["ErrorsFolder"] ?? @"C:\TransTrack\South\Errors",
                CloudinaryCloudName = ConfigurationManager.AppSettings["CloudinaryCloudName"]!,
                CloudinaryApiKey = ConfigurationManager.AppSettings["CloudinaryApiKey"]!,
                CloudinaryApiSecret = ConfigurationManager.AppSettings["CloudinaryApiSecret"]!
            };

            _logger.LogInfo($"Configuration loaded - Input: {_config.InputFolder}");
            Console.WriteLine($"✓ Configuration loaded");
            Console.WriteLine($"  Input Folder: {_config.InputFolder}");
            Console.WriteLine($"  Processed Folder: {_config.ProcessedFolder}");
            Console.WriteLine($"  Errors Folder: {_config.ErrorsFolder}\n");
        }

        private static void EnsureDirectoriesExist()
        {
            OutputWriter.EnsureDirectoryExists(_config.InputFolder);
            OutputWriter.EnsureDirectoryExists(_config.ProcessedFolder);
            OutputWriter.EnsureDirectoryExists(_config.ErrorsFolder);

            _logger.LogInfo("All directories verified/created");
        }

        private static void ProcessFiles()
        {
            string[] txtFiles = FileMover.GetFiles(_config.InputFolder, "*.txt");

            if (txtFiles.Length == 0)
            {
                Console.WriteLine("No TXT files found in input folder.");
                _logger.LogInfo("No TXT files to process");
                return;
            }

            Console.WriteLine($"Found {txtFiles.Length} TXT file(s) to process\n");
            _logger.LogInfo($"Found {txtFiles.Length} TXT files to process");

            int successCount = 0;
            int errorCount = 0;

            foreach (string filePath in txtFiles)
            {
                string fileName = FileHelper.GetFileName(filePath);
                Console.WriteLine($"Processing: {fileName}");
                _logger.LogInfo($"Processing file: {fileName}");

                try
                {
                    bool success = ProcessSingleFile(filePath);

                    if (success)
                    {
                        successCount++;
                        Console.WriteLine($"  ✓ Success\n");
                    }
                    else
                    {
                        errorCount++;
                        Console.WriteLine($"  ✗ Failed (moved to Errors folder)\n");
                    }
                }
                catch (Exception ex)
                {
                    errorCount++;
                    _logger.LogError($"Error processing file {fileName}", ex);
                    Console.WriteLine($"  ✗ Error: {ex.Message}\n");

                    try
                    {
                        FileMover.MoveFile(filePath, _config.ErrorsFolder);
                        WriteErrorFile(fileName, $"Unexpected error: {ex.Message}");
                    }
                    catch (Exception moveEx)
                    {
                        _logger.LogError($"Failed to move error file {fileName}", moveEx);
                    }
                }
            }

            Console.WriteLine($"Processing Summary:");
            Console.WriteLine($"  Total Files: {txtFiles.Length}");
            Console.WriteLine($"  Successful: {successCount}");
            Console.WriteLine($"  Failed: {errorCount}");

            _logger.LogInfo($"Processing complete - Success: {successCount}, Failed: {errorCount}");
        }

        private static bool ProcessSingleFile(string filePath)
        {
            string fileName = FileHelper.GetFileName(filePath);

            try
            {
                _logger.LogInfo($"Parsing TXT file: {fileName}");
                List<ShipmentRecord> records = TxtParser.ParseTxtFile(filePath);
                Console.WriteLine($"  Parsed {records.Count} record(s)");

                _logger.LogInfo($"Validating records for: {fileName}");
                var validator = ValidatorFactory.GetSouthValidator();
                ValidationResult validationResult = validator.ValidateFile(records, fileName);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning($"Validation failed for {fileName}: {validationResult.GetErrorSummary()}");
                    Console.WriteLine($"  Validation failed:");
                    foreach (var error in validationResult.ErrorMessages)
                    {
                        Console.WriteLine($"    - {error}");
                    }

                    FileMover.MoveFile(filePath, _config.ErrorsFolder);
                    WriteErrorFile(fileName, validationResult.GetErrorSummary());
                    return false;
                }

                Console.WriteLine($"  ✓ Validation passed");

                _logger.LogInfo($"Uploading {fileName} to Cloudinary");
                Console.WriteLine($"  Uploading to Cloudinary...");
                string fileUrl = _uploader.UploadFile(filePath);
                Console.WriteLine($"  ✓ Upload complete");
                _logger.LogInfo($"File uploaded successfully: {fileUrl}");

                var summary = new ProcessingSummary(fileName)
                {
                    TotalRecords = records.Count,
                    ValidRecords = records.Count,
                    Success = true,
                    FileUrl = fileUrl,
                    ProcessedDate = DateTime.Now
                };

                string outputFileName = $"{FileHelper.GetFileNameWithoutExtension(fileName)}_processed.txt";
                string outputPath = FileHelper.CombinePaths(_config.ProcessedFolder, outputFileName);
                OutputWriter.WriteProcessedFile(outputPath, summary);
                Console.WriteLine($"  ✓ Output file created");

                FileMover.MoveFile(filePath, _config.ProcessedFolder);
                _logger.LogInfo($"File processed successfully: {fileName}");

                return true;
            }
            catch (InvalidDataException ex)
            {
                _logger.LogError($"Data format error in {fileName}: {ex.Message}");
                Console.WriteLine($"  Data format error: {ex.Message}");

                FileMover.MoveFile(filePath, _config.ErrorsFolder);
                WriteErrorFile(fileName, $"Data format error: {ex.Message}");
                return false;
            }
        }

        private static void WriteErrorFile(string originalFileName, string reason)
        {
            try
            {
                string errorFileName = $"{FileHelper.GetFileNameWithoutExtension(originalFileName)}.error";
                string errorFilePath = FileHelper.CombinePaths(_config.ErrorsFolder, errorFileName);
                OutputWriter.WriteErrorFile(errorFilePath, originalFileName, reason);
                _logger.LogInfo($"Error file created: {errorFileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to write error file for {originalFileName}", ex);
            }
        }
    }
}