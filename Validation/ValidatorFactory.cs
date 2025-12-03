namespace Validation
{
    public class ValidatorFactory
    {
        public static IShipmentValidator GetNorthValidator()
        {
            return new NorthValidator();
        }

        public static IShipmentValidator GetSouthValidator() 
        {
            return new SouthValidator();
        }
    }
}
