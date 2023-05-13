using LibLite.CheapGet.Business.Collections;
using LibLite.CheapGet.Business.Consts.CGQL;
using LibLite.CheapGet.Business.Exceptions.CGQL;
using LibLite.CheapGet.Core.CGQL.Enums;
using LibLite.CheapGet.Core.CGQL.Expressions;
using LibLite.CheapGet.Core.CGQL.Services;
using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Models;

namespace LibLite.CheapGet.Business.Services.CGQL
{
    public class Interpreter : IInterpreter
    {
        private readonly IDictionary<string, IStoreService> _storeServices;
        private readonly IEnvironmentService _environmentService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IReportPresenter _reportPresenter;
        private readonly IFileService _fileService;
        private readonly ILexer _lexer;
        private readonly IParser _parser;

        public Interpreter(
            IDictionary<string, IStoreService> storeServices,
            IEnvironmentService environmentService,
            IReportGenerator reportGenerator,
            IReportPresenter reportPresenter,
            IFileService fileService,
            ILexer lexer,
            IParser parser)
        {
            _storeServices = storeServices;
            _environmentService = environmentService;
            _reportGenerator = reportGenerator;
            _reportPresenter = reportPresenter;
            _fileService = fileService;
            _lexer = lexer;
            _parser = parser;
        }

        public Task InterpretAsync(Expression expression)
        {
            return expression switch
            {
                Select select => InterpretSelectAsync(select),
                Wishlist wishlist => InterpretWishlistAsync(wishlist),
                Load load => InterpretLoadAsync(load),
                Cls => InterpretClsAsync(),
                Exit => InterpretExitAsync(),
                _ => throw new UnsupportedExpressionException(expression),
            };
        }

        private async Task InterpretSelectAsync(Select select)
        {
            var products = await GetProductsAsync(select);
            var report = await _reportGenerator.GenerateAsync(products);
            await _reportPresenter.PresentAsync(report);
        }

        private Task<IEnumerable<Product>> GetProductsAsync(Select select)
        {
            var count = select.Take.Value.Value;
            var filters = InterpretFilter(select.Filters);
            var sorts = InterpretSort(select.Sorts);
            var from = select.From.Text.Value;

            var storeService = _storeServices[from];
            var parameters = new GetProductsRequest(count, filters, sorts);
            return storeService.GetDiscountedProductsAsync(parameters, CancellationToken.None);
        }

        private static IEnumerable<ICollectionFilter<Product>> InterpretFilter(IEnumerable<Filter> filters)
        {
            return filters
                .Select(InterpretFilter)
                .ToList();
        }

        private static ICollectionFilter<Product> InterpretFilter(Filter filter)
        {
            return filter.Value.Type switch
            {
                TokenType.TEXT => CreateStringFilter(filter.Property.Value, ToStringRelationalOperator(filter.Comparison.Value), filter.Value.AsText().Value),
                TokenType.INTEGER => CreateDoubleFilter(filter.Property.Value, ToNumberRelationalOperator(filter.Comparison.Value), filter.Value.AsInteger().Value),
                TokenType.FLOATING => CreateDoubleFilter(filter.Property.Value, ToNumberRelationalOperator(filter.Comparison.Value), filter.Value.AsDecimal().Value),
                _ => throw new NotImplementedException(),
            };
        }

        private static ICollectionFilter<Product> CreateDoubleFilter(string name, NumberRelationalOperator @operator, double value)
        {
            return name switch
            {
                Properties.BASE_PRICE => new CollectionDoubleFilter<Product>(x => x.BasePrice, @operator, value),
                Properties.DISCOUNTED_PRICE => new CollectionDoubleFilter<Product>(x => x.DiscountedPrice, @operator, value),
                Properties.DISCOUNT_PERCENTAGE => new CollectionDoubleFilter<Product>(x => x.DiscountPercentage, @operator, value),
                Properties.DISCOUNT_VALUE => new CollectionDoubleFilter<Product>(x => x.DiscountValue, @operator, value),
                _ => throw new NotImplementedException(),
            };
        }

        private static ICollectionFilter<Product> CreateStringFilter(string name, StringRelationalOperator operation, string value)
        {
            return name switch
            {
                Properties.NAME => new CollectionStringFilter<Product>(x => x.Name, operation, value),
                Properties.STORE_NAME => new CollectionStringFilter<Product>(x => x.StoreName, operation, value),
                _ => throw new NotImplementedException(),
            };
        }

        private static StringRelationalOperator ToStringRelationalOperator(string value)
        {
            return value switch
            {
                Comparisons.EQUAL => StringRelationalOperator.EQUAL,
                Comparisons.CONTAIN => StringRelationalOperator.CONTAIN,
                _ => throw new NotImplementedException(),
            };
        }

        private static NumberRelationalOperator ToNumberRelationalOperator(string value)
        {
            return value switch
            {
                Comparisons.GREATER_OR_EQUAL => NumberRelationalOperator.GREATER_OR_EQUAL,
                Comparisons.GREATER => NumberRelationalOperator.GREATER,
                Comparisons.EQUAL => NumberRelationalOperator.EQUAL,
                Comparisons.NOT_EQUAL => NumberRelationalOperator.NOT_EQUAL,
                Comparisons.LESS => NumberRelationalOperator.LESS,
                Comparisons.LESS_OR_EQUAL => NumberRelationalOperator.LESS_OR_EQUAL,
                _ => throw new NotImplementedException(),
            };
        }

        private static IEnumerable<ICollectionSort<Product>> InterpretSort(IEnumerable<Sort> sorts)
        {
            return sorts
                .Select(InterpretSort)
                .ToList();
        }

        private static ICollectionSort<Product> InterpretSort(Sort sort)
        {
            var name = sort.Property.Value;
            var value = sort.Direction.Value.ToLower();
            var asc = Keywords.ASC.ToLower();
            var direction = value == asc
                ? Core.Enums.SortDirection.ASC
                : Core.Enums.SortDirection.DESC;

            return name switch
            {
                Properties.STORE_NAME => new CollectionSort<Product, string>(x => x.StoreName, direction),
                Properties.NAME => new CollectionSort<Product, string>(x => x.Name, direction),
                Properties.BASE_PRICE => new CollectionSort<Product, double>(x => x.BasePrice, direction),
                Properties.DISCOUNTED_PRICE => new CollectionSort<Product, double>(x => x.DiscountedPrice, direction),
                Properties.DISCOUNT_PERCENTAGE => new CollectionSort<Product, double>(x => x.DiscountPercentage, direction),
                Properties.DISCOUNT_VALUE => new CollectionSort<Product, double>(x => x.DiscountValue, direction),
                _ => throw new NotImplementedException(),
            };
        }

        private async Task InterpretWishlistAsync(Wishlist wishlist)
        {
            var products = await GetProductsAsync(wishlist);
            var report = await _reportGenerator.GenerateAsync(products);
            await _reportPresenter.PresentAsync(report);
        }

        private async Task<IEnumerable<Product>> GetProductsAsync(Wishlist wishlist)
        {
            var count = wishlist.Max.Value.Value;
            var filters = InterpretWish(wishlist.Wishes);
            var from = wishlist.From.Text.Value;

            var storeService = _storeServices[from];
            var tasks = storeService.Stores
                .Select(x => x.GetDiscountedProductsAsync(count, CancellationToken.None))
                .ToList();
            var results = await Task.WhenAll(tasks);
            var products = results.SelectMany(x => x).ToList();
            var filter = filters.Aggregate((current, next) => current.Or(next));
            var result = filter.Apply(products).ToList();
            return result;
        }

        private static IEnumerable<ICollectionFilter<Product>> InterpretWish(IEnumerable<Wish> wishes)
        {
            return wishes
                .Select(InterpretWish)
                .ToList();
        }

        private static ICollectionFilter<Product> InterpretWish(Wish wish)
        {
            ICollectionFilter<Product> result = null;
            var filters = wish.Filters;
            foreach (var filter in filters)
            {
                var partial = filter.Value.Type switch
                {
                    TokenType.TEXT => CreateStringFilter(filter.Property.Value, ToStringRelationalOperator(filter.Comparison.Value), filter.Value.AsText().Value),
                    TokenType.INTEGER => CreateDoubleFilter(filter.Property.Value, ToNumberRelationalOperator(filter.Comparison.Value), filter.Value.AsInteger().Value),
                    TokenType.FLOATING => CreateDoubleFilter(filter.Property.Value, ToNumberRelationalOperator(filter.Comparison.Value), filter.Value.AsDecimal().Value),
                    _ => throw new NotImplementedException(),
                };
                result = result is not null
                    ? result.And(partial)
                    : partial;
            }
            return result;
        }

        private async Task InterpretLoadAsync(Load load)
        {
            var path = load.Source.Value;
            var query = await _fileService.ReadAsync(path);
            var tokens = _lexer.Lex(query);
            var expression = _parser.Parse(tokens);
            await InterpretAsync(expression);
        }

        private Task InterpretClsAsync() => _environmentService.ClearInputAsync();
        private Task InterpretExitAsync() => _environmentService.ExitApplicationAsync();
    }
}
