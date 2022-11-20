using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;

namespace LibLite.CheapGet.Business.Services.Reports
{
    public class HtmlReportPresenter : IReportPresenter
    {
        private readonly IFileService _fileService;

        public HtmlReportPresenter(IFileService fileService)
        {
            _fileService = fileService;
        }

        public async Task PresentAsync(Report report)
        {
            if (report.Format != ReportFormat.HTML)
                throw new Exception("Expected report to be in HTML format.");

            var file = CreateReportFile(report);
            await _fileService.SaveAsync(file);
            _fileService.Open(file);
            await Task.Delay(1000);
            _fileService.Delete(file);
        }

        private static FileModel CreateReportFile(Report report)
        {
            return new FileModel
            {
                Path = $"{Directory.GetCurrentDirectory()}\\Reports",
                Name = DateTime.Now.ToString("yyyy-MM-ddTHH.mm.ss.fffffff"),
                Extension = "html",
                Content = report.GetBytes(),
            };
        }
    }
}
