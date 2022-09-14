using LibLite.CheapGet.Business.Consts.CGQL;
using LibLite.CheapGet.Business.Services.CGQL;
using LibLite.CheapGet.Core.CGQL.Expressions;
using LibLite.CheapGet.Core.CGQL.Services;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Models;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LibLite.CheapGet.Business.Tests.Services.CGQL
{
    [TestFixture]
    public class InterpreterTests
    {
        private IDictionary<string, IStoreService> _storeServicesMock;
        private Mock<IStoreService> _storeServiceMock;
        private Mock<IReportGenerator> _reportGeneratorMock;
        private Mock<IFileService> _fileServiceMock;

        private Mock<ILexer> _lexerMock;
        private Mock<IParser> _parserMock;

        private Interpreter _interpreter;

        [SetUp]
        public void SetUp()
        {
            _storeServiceMock = new();
            _storeServicesMock = new Dictionary<string, IStoreService>
            {
                { Categories.GAMES, _storeServiceMock.Object }
            };
            _reportGeneratorMock = new();
            _fileServiceMock = new();
            _lexerMock = new();
            _parserMock = new();

            _interpreter = new(
                _storeServicesMock,
                _reportGeneratorMock.Object,
                _fileServiceMock.Object,
                _lexerMock.Object,
                _parserMock.Object);
        }

        [Test]
        public async Task InterpretAsync_X_X()
        {
            var expression = new Select
            {
                Take = new(new(10)),
                From = new(new(Categories.GAMES)),
                Filters = new List<Filter>(),
                Sorts = new List<Sort>(),
            };
            var report = new Report
            {
                Content = "<html></html>",
                Format = ReportFormat.HTML,
            };
            var products = new Product[] { };
            _storeServiceMock
                .Setup(x => x.GetDiscountedProductsAsync(It.IsAny<GetProductsRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);
            _reportGeneratorMock
                .Setup(x => x.GenerateReportAsync(products))
                .ReturnsAsync(report);

            await _interpreter.InterpretAsync(expression);

            _fileServiceMock.Verify(x => x.SaveAsync(It.Is<FileModel>(x => true)), Times.Once);
            _fileServiceMock.Verify(x => x.Open(It.Is<FileModel>(x => true)), Times.Once);
            _fileServiceMock.Verify(x => x.Delete(It.Is<FileModel>(x => true)), Times.Once);
        }
    }
}
