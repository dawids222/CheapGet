using LibLite.CheapGet.Business.Collections;
using LibLite.CheapGet.Business.Consts.CGQL;
using LibLite.CheapGet.Business.Exceptions.DSL;
using LibLite.CheapGet.Core.CGQL.Enums;
using LibLite.CheapGet.Core.CGQL.Expressions;
using LibLite.CheapGet.Core.CGQL.Services;
using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Models;

namespace LibLite.CheapGet.Business.Services.CGQL
{
    public class Interpreter : IInterpreter
    {
        private readonly IDictionary<string, IStoreService> _storeServices;
        private readonly IReportGenerator _reportGenerator;
        private readonly IFileService _fileService;

        public Interpreter(
            IDictionary<string, IStoreService> storeServices,
            IReportGenerator reportGenerator,
            IFileService fileService)
        {
            _storeServices = storeServices;
            _reportGenerator = reportGenerator;
            _fileService = fileService;
        }

        public Task InterpretAsync(Expression expression)
        {
            return expression switch
            {
                Select select => InterpretSelectAsync(select),
                Cls => InterpretClsAsync(),
                Exit => InterpretExitAsync(),
                _ => throw new UnsupportedExpressionException(expression),
            };
        }

        private async Task InterpretSelectAsync(Select select)
        {
            var products = await GetProductsAsync(select);
            await DisplayReportAsync(products);
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

        private async Task DisplayReportAsync(IEnumerable<Product> products)
        {
            var report = await _reportGenerator.GenerateReportAsync(products);
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
                Extension = report.Format switch
                {
                    ReportFormat.HTML => "html",
                    _ => throw new NotImplementedException(),
                },
                Content = report.GetBytes(),
            };
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
                Operators.EQUAL => StringRelationalOperator.EQUAL,
                Operators.CONTAIN => StringRelationalOperator.CONTAIN,
                _ => throw new NotImplementedException(),
            };
        }

        private static NumberRelationalOperator ToNumberRelationalOperator(string value)
        {
            return value switch
            {
                Operators.GREATER_OR_EQUAL => NumberRelationalOperator.GREATER_OR_EQUAL,
                Operators.GREATER => NumberRelationalOperator.GREATER,
                Operators.EQUAL => NumberRelationalOperator.EQUAL,
                Operators.NOT_EQUAL => NumberRelationalOperator.NOT_EQUAL,
                Operators.LESS => NumberRelationalOperator.LESS,
                Operators.LESS_OR_EQUAL => NumberRelationalOperator.LESS_OR_EQUAL,
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
            var asc = SortDirections.ASC.ToLower();
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

        private static Task InterpretClsAsync()
        {
            return Task.Run(() => Console.Clear());
        }

        private static Task InterpretExitAsync()
        {
            return Task.Run(() => Environment.Exit(0));
        }
    }
}
