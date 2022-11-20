using LibLite.CheapGet.Core.Services.Models;

namespace LibLite.CheapGet.Core.Services
{
    public interface IReportPresenter
    {
        Task PresentAsync(Report report);
    }
}
