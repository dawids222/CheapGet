namespace LibLite.CheapGet.Core.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNumericType(this Type type)
        {
            return Type.GetTypeCode(type) switch
            {
                TypeCode.Byte or
                TypeCode.SByte or
                TypeCode.UInt16 or
                TypeCode.UInt32 or
                TypeCode.UInt64 or
                TypeCode.Int16 or
                TypeCode.Int32 or
                TypeCode.Int64 or
                TypeCode.Decimal or
                TypeCode.Double or
                TypeCode.Single => true,
                _ => false,
            };
        }
    }
}
