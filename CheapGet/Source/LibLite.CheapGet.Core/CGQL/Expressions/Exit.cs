namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Exit : Expression
    {
        public override bool Equals(object obj)
        {
            return obj is Exit;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
