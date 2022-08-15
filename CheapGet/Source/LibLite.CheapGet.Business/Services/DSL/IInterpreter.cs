namespace LibLite.CheapGet.Business.Services.DSL
{
    public interface IInterpreter
    {
        Task InterpretAsync(Expression expression);
    }
}
