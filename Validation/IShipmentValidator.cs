using Models;

namespace Validation
{
    public interface IShipmentValidator
    {
        ValidationResult ValidateFile(List<ShipmentRecord> records, string fileName);
        ValidationResult ValidateRecord(ShipmentRecord record, int recordNumber);
    }
}
