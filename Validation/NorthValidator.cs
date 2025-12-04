using Models;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Validation
{
    public class NorthValidator : IShipmentValidator
    {
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
            else if (!IsAlphanumeric(record.ShipmentId))
            {
                result.AddError($"Record {recordNumber}: ShipmentId '{record.ShipmentId}' must be alphanumeric");
            }

            if (string.IsNullOrWhiteSpace(record.Origin))
            {
                result.AddError($"Record {recordNumber}: Origin is empty");
            }

            if (string.IsNullOrWhiteSpace(record.Destination))
            {
                result.AddError($"Record {recordNumber}: Destination is empty");
            }

            if (record.ShipmentDate == DateTime.MinValue)
            {
                result.AddError($"Record {recordNumber}: Date is invalid or could not be parsed");
            }
            else if (record.ShipmentDate > DateTime.Now)
            {
                result.AddError($"Record {recordNumber}: Date '{record.ShipmentDate:yyyy-MM-dd}' cannot be in the future");
            }

            if (!record.Weight.HasValue)
            {
                result.AddError($"Record {recordNumber}: Weight is missing or invalid");
            }
            else if (record.Weight.Value <= 0)
            {
                result.AddError($"Record {recordNumber}: Weight must be greater than 0, got {record.Weight.Value}");
            }

            return result;
        }

        private bool IsAlphanumeric(string value)
        {
            return Regex.IsMatch(value, @"^[a-zA-Z0-9]+$");
        }
    }
}
