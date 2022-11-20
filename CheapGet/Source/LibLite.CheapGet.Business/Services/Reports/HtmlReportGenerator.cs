using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;
using LibLite.CheapGet.Core.Stores;
using System.Globalization;
using System.Text;

namespace LibLite.CheapGet.Business.Services.Reports
{
    public class HtmlReportGenerator : IReportGenerator
    {
        public const string TEMPLATE_FILENAME = "html_report_template.html";
        public const string TEMPLATE_CONTENT_PLACEHOLDER = "<!--CONTENT-->";

        private readonly IResourceService _resourceService;

        public HtmlReportGenerator(IResourceService resourceService)
        {
            _resourceService = resourceService;
        }

        public async Task<Report> GenerateAsync(IEnumerable<Product> products)
        {
            var builder = new StringBuilder();
            foreach (var product in products)
            {
                var row = CreateTableRow(product);
                builder.Append(row);
            }
            var rows = builder.ToString();
            var content = await CreateContent(rows);
            return new Report(ReportFormat.HTML, content);
        }

        private static string CreateTableRow(Product product)
        {
            return @$"
        <tr>
            <td>{product.StoreName}</td>
            <td><img src=""{product.ImgUrl}""></img></td>
            <td><a href=""{product.Url}"" target=""_blank"">{product.Name}</a></td>
            <td>{product.BasePrice.ToString("0.00", CultureInfo.InvariantCulture)}</td>
            <td>{product.DiscountedPrice.ToString("0.00", CultureInfo.InvariantCulture)}</td>
            <td>-{product.DiscountValue.ToString("0.00", CultureInfo.InvariantCulture)}</td>
            <td>-{product.DiscountPercentage.ToString("0.00", CultureInfo.InvariantCulture)}%</td>
        </tr>";
        }

        private async Task<string> CreateContent(string rows)
        {
            var template = await _resourceService.ReadAllTextFromResourceAsync(TEMPLATE_FILENAME);
            return template.Replace(TEMPLATE_CONTENT_PLACEHOLDER, rows);
        }
    }
}
