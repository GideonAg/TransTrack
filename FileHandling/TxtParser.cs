using Models;
using System.Collections.Generic;
using System.IO;
using System;

namespace FileHandling
{
    public class TxtParser
    {
        public static List<ShipmentRecord> ParseTxtFile(string filePath)
        {
            var records = new List<ShipmentRecord>();
            string[] lines = FileReader.ReadAllLines(filePath);

            if (lines.Length == 0)
            {
                throw new InvalidDataException("TXT file is empty");
            }

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var record = ParseTxtLine(line);
                records.Add(record);
            }

            return records;
        }

        public static ShipmentRecord ParseTxtLine(string line)
        {
            var parts = line.Split('|');

            if (parts.Length != 5)
            {
                throw new InvalidDataException($"Invalid TXT format. Expected 5 fields, got {parts.Length}");
            }

            var record = new ShipmentRecord
            {
                ShipmentId = parts[0].Trim(),
                Region = parts[1].Trim(),
                Destination = parts[2].Trim(),
                LoadType = parts[4].Trim(),
                RawData = line
            };

            if (DateTime.TryParse(parts[3].Trim(), out DateTime shipmentDate))
            {
                record.ShipmentDate = shipmentDate;
            }

            return record;
        }
    }
}
