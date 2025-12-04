# TransTrack Processors

Small console processors that parse, validate and upload shipment files for TransTrack Ltd.

Projects
- `NorthProcessor` — processes CSV files (warehouse North).
- `SouthProcessor` — processes pipe-delimited TXT files (warehouse South).

Target: .NET Framework 4.6.1 — open with Visual Studio.

---

## Prerequisites

- Visual Studio (recommended) or MSBuild-capable IDE
- .NET Framework 4.6.1 runtime / developer pack installed
- (Optional) Cloudinary account for real uploads

---

## Quick setup

1. Clone the repo:
   - `git clone https://github.com/GideonAg/TransTrack`

2. Open the solution in Visual Studio.

3. Select which processor to run:
   - Right-click a project in Solution Explorer and choose __Set as Startup Project__.

4. Build:
   - Use __Build > Build Solution__ or `msbuild` / Visual Studio Build.

5. Run:
   - Press F5 or Start Without Debugging.

---

## Configuration — Cloudinary credentials

Processors read credentials from `App.config` `appSettings` keys:
- `CloudinaryCloudName` : drrpq05zz
- `CloudinaryApiKey` : 143116378342162
- `CloudinaryApiSecret` : 2-gEeh9OTEei4S0UZt3gznxX3NM


Behavior
- If credentials are missing or empty the application automatically uses the built-in mock uploader (`UploaderFactory.CreateMockUploader()`), so the processors can run without Cloudinary.

---	

## Default folders

Default directories (override via `App.config` or environment):
- North:
  - Input: `C:\TransTrack\North\Incoming`
  - Processed: `C:\TransTrack\North\Processed`
  - Errors: `C:\TransTrack\North\Errors`
- South:
  - Input: `C:\TransTrack\South\Incoming`
  - Processed: `C:\TransTrack\South\Processed`
  - Errors: `C:\TransTrack\South\Errors`

Default log file: `C:\TransTrack\Logs\system.log` (change by constructing `FileLogger` with a different path).

---

## Architecture overview

High-level flow (per processor)
1. Load configuration (from `App.config` or environment).
2. Ensure directories exist.
3. Collect input files (`FileMover.GetFiles`).
4. Parse files:
   - North: `CsvParser` (CSV 5 columns).
   - South: `TxtParser` (pipe-delimited 5 fields).
5. Validate records (`NorthValidator` / `SouthValidator` via `ValidatorFactory`).
6. Upload the original source file using `IFileUploader`:
   - `CloudinaryUploader` (real) or `MockFileUploader`.
7. Write processed output (`OutputWriter.WriteProcessedFile`) and move files to `Processed` or `Errors`.
8. Log events via `FileLogger` (`ILogger`).

Core folders
- `FileHandling` — parsing, reading, moving, writing.
- `Validation` — validators and factory.
- `Services` — uploader implementations and factory.
- `Logging` — logger interfaces and file logger.
- `Models` — DTOs and configuration types.

Key types
- `IFileUploader`, `CloudinaryUploader`, `UploaderFactory`
- `ILogger`, `FileLogger`
- `CsvParser`, `TxtParser`, `FileMover`, `OutputWriter`
- `ShipmentRecord`, `ProcessingSummary`, `ValidationResult`, `ProcessorConfiguration`

---

## Input format expectations

- North CSV per line: `ShipmentId,Origin,Destination,Date,Weight`
- South TXT per line: `ShipmentId|Region|Destination|Date|LoadType`

Validation rules live in `NorthValidator` and `SouthValidator`.
