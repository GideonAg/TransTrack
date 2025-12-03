# TransTrack Processors

Small console processors that parse, validate and upload shipment files (North: CSV, South: TXT) and write processing output.

---

## Prerequisites
- .NET 8 SDK
- Visual Studio (recommended) or `dotnet` CLI
- Optional: Cloudinary account for real uploads

---

## Quick setup

1. Clone the repo and open the solution in Visual Studio or use the CLI:
   - `git clone <repo-url>`
   - `dotnet build`

2. In Visual Studio, pick which processor to run:
   - Right-click project and select __Set as Startup Project__ for `NorthProcessor` or `SouthProcessor`.

3. Configure file folders (optional)
   - Default folders are:
     - `C:\TransTrack\North\Incoming`, `C:\TransTrack\North\Processed`, `C:\TransTrack\North\Errors`
     - `C:\TransTrack\South\Incoming`, `C:\TransTrack\South\Processed`, `C:\TransTrack\South\Errors`
   - Change values by editing the `appSettings` keys in `NorthProcessor\App.config` or `SouthProcessor\App.config` (see below) or by setting environment variables in your run configuration.

4. Run the processor (F5 or `dotnet run` from project folder).

---

## Cloudinary credentials

The processors obtain Cloudinary credentials from `appSettings` keys:
- `CloudinaryCloudName`
- `CloudinaryApiKey`
- `CloudinaryApiSecret`

You have two recommended options to configure them:

A) Edit `NorthProcessor\App.config` / `SouthProcessor\App.config` (not recommended for real secrets)
B) Use secure local settings (recommended)
- Use Visual Studio __Project Properties__ > __Debug__ > __Environment variables__ to set:
  - `CloudinaryCloudName`, `CloudinaryApiKey`, `CloudinaryApiSecret`
- Or use __Manage User Secrets__ for each project and add the same keys there.
- Alternatively, set OS-level environment variables and configure your run profile to pick them up.

Important: Do not commit real credentials to source control. Remove any real keys from `App.config` before committing.

---

## Architecture overview

Projects:
- `NorthProcessor` — processes CSV files (warehouse North).
- `SouthProcessor` — processes TXT files (warehouse South).

Core folders / responsibilities:
- `FileHandling` — file I/O utilities:
  - `FileMover`, `FileReader`, `CsvParser`, `TxtParser`, `OutputWriter`, `FileHelper`.
- `Validation` — validation logic:
  - `NorthValidator`, `SouthValidator`, `ValidatorFactory`, `IShipmentValidator`.
- `Services` — external services:
  - `CloudinaryUploader` implements `IFileUploader` and a `MockFileUploader` is available via `UploaderFactory`.
- `Logging` — logging abstraction and implementation:
  - `ILogger`, `FileLogger` (writes to `C:\TransTrack\Logs\system.log` by default).
- `Models` — DTOs and configuration:
  - `ShipmentRecord`, `ProcessingSummary`, `ProcessorConfiguration`, `ValidationResult`.

Processing flow (high level):
1. Read configuration (`App.config` or environment).
2. Ensure directories exist (`OutputWriter.EnsureDirectoryExists`).
3. Collect files using `FileMover.GetFiles`.
4. Parse files (`CsvParser` / `TxtParser`) into `ShipmentRecord`.
5. Validate records via `ValidatorFactory`.
6. Upload the original source file using `IFileUploader` (Cloudinary or Mock).
7. Produce a processed output (`OutputWriter.WriteProcessedFile`) and move source file to `Processed` (or `Errors`) folder.
8. Log events using `FileLogger`.

---

## Notes
- Default log path is `C:\TransTrack\Logs\system.log`. Ensure the process has write permission or change path by creating `FileLogger` with a different path.
- The code currently reads configuration via `ConfigurationManager.AppSettings` — if you switch to environment variables/user secrets, ensure they are exposed to the app domain or update the code to read `Environment.GetEnvironmentVariable(...)`.
- For testing without Cloudinary, the app will automatically use the mock uploader when credentials are missing.
- Input format expectations:
  - North CSV: 5 columns — `ShipmentId,Origin,Destination,Date,Weight`
  - South TXT: pipe-delimited with 5 fields — `ShipmentId|Region|Destination|Date|LoadType`

---
