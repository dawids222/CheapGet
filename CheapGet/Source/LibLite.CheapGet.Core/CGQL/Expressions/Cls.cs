namespace LibLite.CheapGet.Core.CGQL.Expressions
{
    public class Cls : Expression
    {

        public override bool Equals(object obj)
        {
            return obj is Cls;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}
