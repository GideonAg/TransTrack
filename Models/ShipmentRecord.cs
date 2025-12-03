using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ShipmentRecord
    {
        public string ShipmentId { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateTime ShipmentDate { get; set; }
        public string RawData { get; set; }

        // North-specific
        public decimal? Weight { get; set; }

        // South-specific
        public string Region { get; set; }
        public string LoadType { get; set; }

        public ShipmentRecord()
        {
            ShipmentId = string.Empty;
            Origin = string.Empty;
            Destination = string.Empty;
            RawData = string.Empty;
        }
    }
}
