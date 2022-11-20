using LibLite.CheapGet.Core.Services.Models;
using LibLite.CheapGet.Core.Stores;

namespace LibLite.CheapGet.Core.Services
{
    public interface IReportGenerator
    {
        Task<Report> GenerateAsync(IEnumerable<Product> products);
    }
}
