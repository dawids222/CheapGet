using LibLite.CheapGet.Business.Collections;
using LibLite.CheapGet.Client.Console.Consts;
using LibLite.CheapGet.Client.Console.Extensions;
using LibLite.CheapGet.Core.CGQL.Expressions;
using LibLite.CheapGet.Core.CGQL.Services;
using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.Steam;
using LibLite.CheapGet.Core.Stores.Models;
using LibLite.DI.Lite;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LibLite.CheapGet.Business.Tests.Services.CGQL
{
    [TestFixture]
    public class InterpreterTests : IDisposable
    {
        private readonly DI.Lite.Container _container;
        private readonly DependencyProvider _scope;

        private readonly IStoreService _gameStoreServiceMock;
        private readonly IFileService _fieServiceMock;
        private readonly IReportGenerator _reportGeneratorMock;
        private readonly IReportPresenter _reportPresenterMock;
        private readonly IEnvironmentService _environmentServiceMock;

        private ILexer _lexer => _scope.Get<ILexer>();
        private IParser _parser => _scope.Get<IParser>();
        private IInterpreter _interpreter => _scope.Get<IInterpreter>();

        public InterpreterTests()
        {
            _container = new();
            _container.RegisterDependencies();

            _container.Remove<IStoreService>(Tags.StoreServices.Games);
            _gameStoreServiceMock = Substitute.For<IStoreService>();
            _container.Scoped(Tags.StoreServices.Games, _ => _gameStoreServiceMock);

            _container.Remove<IFileService>();
            _fieServiceMock = Substitute.For<IFileService>();
            _container.Scoped(_ => _fieServiceMock);

            _container.Remove<IReportGenerator>();
            _reportGeneratorMock = Substitute.For<IReportGenerator>();
            _container.Scoped(_ => _reportGeneratorMock);

            _container.Remove<IReportPresenter>();
            _reportPresenterMock = Substitute.For<IReportPresenter>();
            _container.Scoped(_ => _reportPresenterMock);

            _container.Remove<IEnvironmentService>();
            _environmentServiceMock = Substitute.For<IEnvironmentService>();
            _container.Scoped(_ => _environmentServiceMock);

            _scope = _container.CreateScope();
        }

        [Test]
        public async Task InterpretAsync_Select_PresentsReportWithDefaultParameters()
        {
            var input = "select";
            var products = new Product[]
            {
                new SteamProduct(default, default, default, default, default),
            };
            var report = new Report();
            _gameStoreServiceMock
                .GetDiscountedProductsAsync(
                    Arg.Is<GetProductsRequest>(x =>
                        x.Count == Select.DEFAULT_TAKE &&
                        !x.Sorts.Any() &&
                        !x.Filters.Any()),
                    Arg.Any<CancellationToken>())
                .Returns(products);
            _reportGeneratorMock
                .GenerateAsync(products)
                .Returns(report);

            var tokens = _lexer.Lex(input);
            var expression = _parser.Parse(tokens);
            await _interpreter.InterpretAsync(expression);

            await _reportPresenterMock.Received(1).PresentAsync(report);
        }

        [Test]
        public async Task InterpretAsync_Wishlist_PresentsReportWithDefaultParameters()
        {
            var input = @"wishlist";
            var products = new Product[]
            {
                new SteamProduct(default, default, default, default, default),
            };
            var report = new Report();
            _gameStoreServiceMock
                .GetWishlistProductsAsync(
                    Arg.Is<GetWishlistProductsRequest>(x =>
                        x.Count == Wishlist.DEFAULT_MAX &&
                        !x.Filters.Any()),
                    Arg.Any<CancellationToken>())
                .Returns(products);
            _reportGeneratorMock
                .GenerateAsync(products)
                .Returns(report);

            var tokens = _lexer.Lex(input);
            var expression = _parser.Parse(tokens);
            await _interpreter.InterpretAsync(expression);

            await _reportPresenterMock.Received(1).PresentAsync(report);
        }

        [Test]
        public async Task InterpretAsync_Load_ExecutesCommandReadFromFileService()
        {
            var input = "load \"query.cgql\"";
            var products = new Product[]
            {
                new SteamProduct(default, default, default, default, default),
            };
            var report = new Report();
            _fieServiceMock
                .ReadAsync("query.cgql")
                .Returns("select");
            _gameStoreServiceMock
                .GetDiscountedProductsAsync(
                    Arg.Is<GetProductsRequest>(x =>
                        x.Count == Select.DEFAULT_TAKE &&
                        !x.Sorts.Any() &&
                        !x.Filters.Any()),
                    Arg.Any<CancellationToken>())
                .Returns(products);
            _reportGeneratorMock
                .GenerateAsync(products)
                .Returns(report);

            var tokens = _lexer.Lex(input);
            var expression = _parser.Parse(tokens);
            await _interpreter.InterpretAsync(expression);

            await _reportPresenterMock.Received(1).PresentAsync(report);
        }

        [Test]
        public async Task InterpretAsync_ComplexSelect_PresentsReportWithParameters()
        {
            var input = @"select
                          take 200
                          filter ""name"" <> ""the""
                          filter ""discounted_price"" <= 100
                          filter ""discount_percentage"" >= 49.5
                          sort ""store_name"" asc
                          sort ""discount_percentage"" desc";
            var products = new Product[]
            {
                new SteamProduct(default, default, default, default, default),
            };
            var report = new Report();
            _gameStoreServiceMock
                .GetDiscountedProductsAsync(
                    Arg.Is<GetProductsRequest>(x =>
                        x.Count == 200 &&
                        x.Filters.Count() == 3 &&
                        x.Filters.ElementAt(0) is CollectionStringFilter<Product> &&
                        ((CollectionStringFilter<Product>)x.Filters.ElementAt(0)).Operator == StringRelationalOperator.CONTAIN &&
                        ((CollectionStringFilter<Product>)x.Filters.ElementAt(0)).Value == "the" &&
                        x.Filters.ElementAt(1) is CollectionDoubleFilter<Product> &&
                        ((CollectionDoubleFilter<Product>)x.Filters.ElementAt(1)).Operator == NumberRelationalOperator.LESS_OR_EQUAL &&
                        ((CollectionDoubleFilter<Product>)x.Filters.ElementAt(1)).Value == 100 &&
                        x.Filters.ElementAt(2) is CollectionDoubleFilter<Product> &&
                        ((CollectionDoubleFilter<Product>)x.Filters.ElementAt(2)).Operator == NumberRelationalOperator.GREATER_OR_EQUAL &&
                        ((CollectionDoubleFilter<Product>)x.Filters.ElementAt(2)).Value == 49.5 &&
                        x.Sorts.Count() == 2 &&
                        x.Sorts.ElementAt(0) is CollectionSort<Product, string> &&
                        x.Sorts.ElementAt(0).SortDirection == Core.Enums.SortDirection.ASC &&
                        x.Sorts.ElementAt(1) is CollectionSort<Product, double> &&
                        x.Sorts.ElementAt(1).SortDirection == Core.Enums.SortDirection.DESC),
                    Arg.Any<CancellationToken>())
                .Returns(products);
            _reportGeneratorMock
                .GenerateAsync(products)
                .Returns(report);

            var tokens = _lexer.Lex(input);
            var expression = _parser.Parse(tokens);
            await _interpreter.InterpretAsync(expression);

            await _reportPresenterMock.Received(1).PresentAsync(report);
        }

        [Test]
        public async Task InterpretAsync_ComplexWishlist_PresentsReportWithParameters()
        {
            var input = @"wishlist
                          from ""Games""
                          wish 
                               filter ""name"" <> ""south park""
                               filter ""store_name"" <> ""steam""
                          wish 
                               filter ""name"" <> ""cyberpunk 2077""
                               filter ""discounted_price"" <= 60.1
                               filter ""store_name"" <> ""steam""
                          max 200";
            var products = new Product[]
            {
                new SteamProduct(default, default, default, default, default),
            };
            var report = new Report();
            _gameStoreServiceMock
                .GetWishlistProductsAsync(
                    Arg.Is<GetWishlistProductsRequest>(x =>
                        x.Count == 200 &&
                        x.Filters.Count() == 2),
                    Arg.Any<CancellationToken>())
                .Returns(products);
            _reportGeneratorMock
                .GenerateAsync(products)
                .Returns(report);

            var tokens = _lexer.Lex(input);
            var expression = _parser.Parse(tokens);
            await _interpreter.InterpretAsync(expression);

            await _reportPresenterMock.Received(1).PresentAsync(report);
        }

        [Test]
        public async Task InterpretAsync_Cls_ClearsInput()
        {
            var input = "cls";

            var tokens = _lexer.Lex(input);
            var expression = _parser.Parse(tokens);
            await _interpreter.InterpretAsync(expression);

            await _environmentServiceMock.Received(1).ClearInputAsync();
        }

        [Test]
        public async Task InterpretAsync_Exit_ExistsApplication()
        {
            var input = "exit";

            var tokens = _lexer.Lex(input);
            var expression = _parser.Parse(tokens);
            await _interpreter.InterpretAsync(expression);

            await _environmentServiceMock.Received(1).ExitApplicationAsync();
        }

        public void Dispose()
        {
            _scope.Dispose();
            _container.Dispose();
        }
    }
}
