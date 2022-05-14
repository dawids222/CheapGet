namespace LibLite.CheapGet.Core.Services
{
    public interface ISerializer
    {
        string Serialize(object value);
        T Deserialize<T>(string value);
    }
}
