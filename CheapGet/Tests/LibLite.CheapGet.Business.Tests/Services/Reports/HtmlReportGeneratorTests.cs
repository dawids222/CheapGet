using LibLite.CheapGet.Business.Services.Reports;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.GoG;
using LibLite.CheapGet.Core.Stores.Games.Steam;
using Moq;
using NUnit.Framework;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LibLite.CheapGet.Business.Tests.Services.Reports
{
    [TestFixture]
    public class HtmlReportGeneratorTests
    {
        private Mock<IResourceService> _resourceServiceMock;

        private HtmlReportGenerator _reportGenerator;

        [SetUp]
        public void SetUp()
        {
            _resourceServiceMock = new();

            _reportGenerator = new(_resourceServiceMock.Object);
        }

        [Test]
        public async Task GenerateReportAsync_Success_ReturnsFilledTemplate()
        {
            _resourceServiceMock
                .Setup(x => x.ReadAllTextFromResourceAsync(HtmlReportGenerator.TEMPLATE_FILENAME))
                .ReturnsAsync($"<html><head></head><body><table>{HtmlReportGenerator.TEMPLATE_CONTENT_PLACEHOLDER}</table></body></html>");
            var products = new Product[]
            {
                new SteamProduct("steam_game", 10, 5, "http://steam_game.com", "http://steam_game.com/app/1234"),
                new GogProduct("gog_game", 20, 7.5, "http://gog_game.com", "http://gog_game.com/app/4321"),
            };

            var result = await _reportGenerator.GenerateAsync(products);

            var expected = @$"
            <html><head></head><body><table>
                <tr>
                    <td>{products[0].StoreName}</td>
                    <td><img src=""{products[0].ImgUrl}""></img></td>
                    <td><a href=""http://steam_game.com/app/1234"" target=""_blank"">{products[0].Name}</a></td>
                    <td>{products[0].BasePrice.ToString("0.00", CultureInfo.InvariantCulture)}</td>
                    <td>{products[0].DiscountedPrice.ToString("0.00", CultureInfo.InvariantCulture)}</td>
                    <td>-{products[0].DiscountValue.ToString("0.00", CultureInfo.InvariantCulture)}</td>
                    <td>-{products[0].DiscountPercentage.ToString("0.00", CultureInfo.InvariantCulture)}%</td>
                </tr>
                <tr>
                    <td>{products[1].StoreName}</td>
                    <td><img src=""{products[1].ImgUrl}""></img></td>
                    <td><a href=""http://gog_game.com/app/4321"" target=""_blank"">{products[1].Name}</a></td>
                    <td>{products[1].BasePrice.ToString("0.00", CultureInfo.InvariantCulture)}</td>
                    <td>{products[1].DiscountedPrice.ToString("0.00", CultureInfo.InvariantCulture)}</td>
                    <td>-{products[1].DiscountValue.ToString("0.00", CultureInfo.InvariantCulture)}</td>
                    <td>-{products[1].DiscountPercentage.ToString("0.00", CultureInfo.InvariantCulture)}%</td>
                </tr>
            </table></body></html>";
            expected = Regex.Replace(expected, @"\s+", "");
            var actual = Regex.Replace(result.Content, @"\s+", "");
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(ReportFormat.HTML, result.Format);
        }

        [Test]
        public void GenerateReportAsync_ResourceServiceThrows_ThrowsTheSameException()
        {
            var exception = new Exception("Error!");
            _resourceServiceMock
                .Setup(x => x.ReadAllTextFromResourceAsync(It.IsAny<string>()))
                .ThrowsAsync(exception);
            var products = new Product[]
            {
                new SteamProduct("steam_game", 10, 5, "http://steam_game.com", "http://steam_game.com/app/1234"),
                new GogProduct("gog_game", 20, 7.5, "http://gog_game.com", "http://gog_game.com/app/4321"),
            };

            Task act() => _reportGenerator.GenerateAsync(products);

            Assert.ThrowsAsync<Exception>(act, exception.Message);
        }
    }
}
