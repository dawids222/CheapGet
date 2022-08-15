namespace LibLite.CheapGet.Business.Services.DSL
{
    public interface IParser
    {
        Expression Parse(IEnumerable<Token> tokens);
    }
}
