using LibLite.CheapGet.Business.Collections;
using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Models;

namespace LibLite.CheapGet.Business.Services.DSL
{
    public class Interpreter : IInterpreter
    {
        private readonly IStoreService _storeService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IFileService _fileService;

        public Interpreter(
            IStoreService storeService, // TODO: FROM keyword won't work with this approach...
            IReportGenerator reportGenerator,
            IFileService fileService)
        {
            _storeService = storeService;
            _reportGenerator = reportGenerator;
            _fileService = fileService;
        }

        public Task InterpretAsync(Expression expression)
        {
            return expression switch
            {
                Select select => SelectAsync(select),
                Cls => ClearScreenAsync(),
                Exit => ExitAsync(),
                _ => throw new Exception("Error"), // TODO: Provide meaningful error
            };
        }

        private async Task SelectAsync(Select select)
        {
            var take = select.Take.Value.Value;
            var filters = GetFilters(select.Filters);
            var sorts = GetSorts(select.Sorts);

            var parameters = new GetProductsRequest(take, filters, sorts);
            var products = await _storeService.GetDiscountedProductsAsync(parameters, CancellationToken.None);
            var report = await _reportGenerator.GenerateReportAsync(products);
            // TODO: This probably should be abstracted..
            var file = new FileModel
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
            await _fileService.SaveAsync(file);
            _fileService.Open(file);
            await Task.Delay(1000);
            _fileService.Delete(file);
        }

        private static IEnumerable<ICollectionFilter<Product>> GetFilters(IEnumerable<Filter> filters)
        {
            foreach (var filter in filters)
            {
                yield return filter.Value.Type switch
                {
                    TokenType.TEXT => CreateStringFilter(filter.Property.Value, ToStringRelationalOperator(filter.Comparison.Value), filter.Value.AsText().Value),
                    TokenType.INTEGER => CreateDoubleFilter(filter.Property.Value, ToNumberRelationalOperator(filter.Comparison.Value), filter.Value.AsInteger().Value),
                    TokenType.FLOATING => CreateDoubleFilter(filter.Property.Value, ToNumberRelationalOperator(filter.Comparison.Value), filter.Value.AsDecimal().Value),
                    _ => throw new NotImplementedException(),
                };
            };
        }

        private static ICollectionFilter<Product> CreateDoubleFilter(string name, NumberRelationalOperator @operator, double value)
        {
            return name switch
            {
                "base_price" => new CollectionDoubleFilter<Product>(x => x.BasePrice, @operator, value),
                "discounted_price" => new CollectionDoubleFilter<Product>(x => x.DiscountedPrice, @operator, value),
                "discount_percentage" => new CollectionDoubleFilter<Product>(x => x.DiscountPercentage, @operator, value),
                "discount_value" => new CollectionDoubleFilter<Product>(x => x.DiscountValue, @operator, value),
                _ => throw new NotImplementedException(),
            };
        }

        private static ICollectionFilter<Product> CreateStringFilter(string name, StringRelationalOperator operation, string value)
        {
            return name switch
            {
                "name" => new CollectionStringFilter<Product>(x => x.Name, operation, value),
                "store_name" => new CollectionStringFilter<Product>(x => x.StoreName, operation, value),
                _ => throw new NotImplementedException(),
            };
        }

        private static StringRelationalOperator ToStringRelationalOperator(string value)
        {
            return value switch
            {
                "=" => StringRelationalOperator.EQUAL,
                "<>" => StringRelationalOperator.CONTAIN,
                _ => throw new NotImplementedException(),
            };
        }

        private static NumberRelationalOperator ToNumberRelationalOperator(string value)
        {
            return value switch
            {
                ">=" => NumberRelationalOperator.GREATER_OR_EQUAL,
                ">" => NumberRelationalOperator.GREATER,
                "=" => NumberRelationalOperator.EQUAL,
                "!=" => NumberRelationalOperator.NOT_EQUAL,
                "<" => NumberRelationalOperator.LESS,
                "<=" => NumberRelationalOperator.LESS_OR_EQUAL,
                _ => throw new NotImplementedException(),
            };
        }

        private static IEnumerable<ICollectionSort<Product>> GetSorts(IEnumerable<Sort> sorts)
        {
            foreach (var sort in sorts)
            {
                var name = sort.Property.Value;
                var direction = sort.Direction.Value.ToLower() == "asc"
                    ? Core.Enums.SortDirection.ASC
                    : Core.Enums.SortDirection.DESC;
                yield return name switch
                {
                    // TODO: Move those literals to consts?
                    "store_name" => new CollectionSort<Product, string>(x => x.StoreName, direction),
                    "name" => new CollectionSort<Product, string>(x => x.Name, direction),
                    "base_price" => new CollectionSort<Product, double>(x => x.BasePrice, direction),
                    "discounted_price" => new CollectionSort<Product, double>(x => x.DiscountedPrice, direction),
                    "discount_percentage" => new CollectionSort<Product, double>(x => x.DiscountPercentage, direction),
                    "discount_value" => new CollectionSort<Product, double>(x => x.DiscountValue, direction),
                    _ => throw new NotImplementedException(),
                };
            }
        }

        private static Task ClearScreenAsync()
        {
            return Task.Run(() => Console.Clear());
        }

        private static Task ExitAsync()
        {
            return Task.Run(() => Environment.Exit(0));
        }
    }
}
