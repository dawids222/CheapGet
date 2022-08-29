using LibLite.CheapGet.Business.Services.CGQL;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace LibLite.CheapGet.Business.Tests.Services.CGQL
{
    [TestFixture]
    public class InterpreterTests
    {
        private IDictionary<string, IStoreService> _storeServicesMock;
        private Mock<IStoreService> _storeServiceMock;
        private Mock<IReportGenerator> _reportGeneratorMock;
        private Mock<IFileService> _fileServiceMock;

        private Interpreter _interpreter;

        [SetUp]
        public void SetUp()
        {
            _storeServiceMock = new();
            _storeServicesMock = new Dictionary<string, IStoreService>
            {
                { "Games", _storeServiceMock.Object }
            };
            _reportGeneratorMock = new();
            _fileServiceMock = new();

            _interpreter = new(
                _storeServicesMock,
                _reportGeneratorMock.Object,
                _fileServiceMock.Object);
        }


    }
}
