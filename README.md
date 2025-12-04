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
