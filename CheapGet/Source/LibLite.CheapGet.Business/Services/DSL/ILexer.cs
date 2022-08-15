namespace LibLite.CheapGet.Business.Services.DSL
{
    public interface ILexer
    {
        IEnumerable<Token> Lex(string input);
    }
}
