using Models;
using System.Collections.Generic;
using System.IO;
using System;

namespace FileHandling
{
    public class CsvParser
    {
        public static List<ShipmentRecord> ParseCsvFile(string filePath)
        {
            var records = new List<ShipmentRecord>();
            string[] lines = FileReader.ReadAllLines(filePath);

            if (lines.Length == 0)
            {
                throw new InvalidDataException("CSV file is empty");
            }

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var record = ParseCsvLine(line);
                records.Add(record);
            }

            return records;
        }

        public static ShipmentRecord ParseCsvLine(string line)
        {
            var parts = line.Split(',');

            if (parts.Length != 5)
            {
                throw new InvalidDataException($"Invalid CSV format. Expected 5 columns, got {parts.Length}");
            }

            var record = new ShipmentRecord
            {
                ShipmentId = parts[0].Trim(),
                Origin = parts[1].Trim(),
                Destination = parts[2].Trim(),
                RawData = line
            };

            if (DateTime.TryParse(parts[3].Trim(), out DateTime shipmentDate))
            {
                record.ShipmentDate = shipmentDate;
            }

            if (decimal.TryParse(parts[4].Trim(), out decimal weight))
            {
                record.Weight = weight;
            }

            return record;
        }
    }
}
