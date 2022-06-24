namespace LibLite.CheapGet.Core.Tests.Collections.Models
{
    public class CollectionOperationModel
    {
        public string StringValue { get; init; }
        public double DoubleValue { get; init; }

        public CollectionOperationModel(string stringValue, double doubleValue)
        {
            StringValue = stringValue;
            DoubleValue = doubleValue;
        }
    }
}
