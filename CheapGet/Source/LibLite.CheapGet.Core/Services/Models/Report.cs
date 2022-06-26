using LibLite.CheapGet.Core.Enums;
using System.Text;

namespace LibLite.CheapGet.Core.Services.Models
{
    public class Report
    {
        public ReportFormat Format { get; init; }
        public string Content { get; init; }

        public Report() { }
        public Report(ReportFormat format, string content)
        {
            Format = format;
            Content = content;
        }

        public byte[] GetBytes() => Encoding.UTF8.GetBytes(Content);
    }
}
