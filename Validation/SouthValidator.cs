using Models;

namespace Validation
{
    public class SouthValidator : IShipmentValidator
    {
        private static readonly HashSet<string> ValidRegions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "North", "South", "East", "West"
        };

        private static readonly HashSet<string> ValidLoadTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Fragile", "Bulk", "Liquid"
        };

        public ValidationResult ValidateFile(List<ShipmentRecord> records, string fileName)
        {
            var result = new ValidationResult(true) { FileName = fileName };

            if (records == null || records.Count == 0)
            {
                result.AddError("File contains no records");
                return result;
            }

            for (int i = 0; i < records.Count; i++)
            {
                var recordResult = ValidateRecord(records[i], i + 1);
                if (!recordResult.IsValid)
                {
                    foreach (var error in recordResult.ErrorMessages)
                    {
                        result.AddError(error);
                    }
                }
            }

            return result;
        }

        public ValidationResult ValidateRecord(ShipmentRecord record, int recordNumber)
        {
            var result = new ValidationResult(true);

            if (string.IsNullOrWhiteSpace(record.ShipmentId))
            {
                result.AddError($"Record {recordNumber}: ShipmentId is empty");
            }
            else if (!record.ShipmentId.StartsWith("S-", StringComparison.OrdinalIgnoreCase))
            {
                result.AddError($"Record {recordNumber}: ShipmentId '{record.ShipmentId}' must start with 'S-'");
            }

            if (string.IsNullOrWhiteSpace(record.Region))
            {
                result.AddError($"Record {recordNumber}: Region is empty");
            }
            else if (!ValidRegions.Contains(record.Region))
            {
                result.AddError($"Record {recordNumber}: Region '{record.Region}' must be one of: North, South, East, West");
            }

            if (string.IsNullOrWhiteSpace(record.Destination))
            {
                result.AddError($"Record {recordNumber}: Destination is empty");
            }

            if (record.ShipmentDate == DateTime.MinValue)
            {
                result.AddError($"Record {recordNumber}: ShipmentDate is invalid or could not be parsed");
            }
            else if (IsWeekend(record.ShipmentDate))
            {
                result.AddError($"Record {recordNumber}: ShipmentDate '{record.ShipmentDate:yyyy-MM-dd}' cannot be on a weekend ({record.ShipmentDate.DayOfWeek})");
            }

            if (string.IsNullOrWhiteSpace(record.LoadType))
            {
                result.AddError($"Record {recordNumber}: LoadType is empty");
            }
            else if (!ValidLoadTypes.Contains(record.LoadType))
            {
                result.AddError($"Record {recordNumber}: LoadType '{record.LoadType}' must be one of: Fragile, Bulk, Liquid");
            }

            return result;
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }
    }
}
