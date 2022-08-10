using LibLite.CheapGet.Business.Collections;
using LibLite.CheapGet.Client.Console.Consts;
using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Models;
using LibLite.DI.Lite.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LibLite.CheapGet.Client.Console.Services.Application
{
    // NOTE: This is just a temporary solution.
    // TODO: Design and implement my own proper DSL with Lexer, Parser, Interpreter pattern.
    public class TemporaryApplication : IApplication
    {
        private readonly IStoreService _storeService;
        private readonly IReportGenerator _reportGenerator;
        private readonly IFileService _fileService;

        private readonly List<ICollectionFilter<Product>> filters = new();
        private readonly List<ICollectionSort<Product>> sorts = new();
        private int count = 100;

        public TemporaryApplication(
            [WithTag(Tags.StoreServices.Games)]
            IStoreService storeService,
            IReportGenerator reportGenerator,
            IFileService fileService)
        {
            _storeService = storeService;
            _reportGenerator = reportGenerator;
            _fileService = fileService;
        }

        public async Task StartAsync()
        {
            while (true)
            {
                try { await MakeRequest(); }
                catch (Exception ex)
                {
                    System.Console.WriteLine(ex.Message);
                }
            }
        }

        private async Task MakeRequest()
        {
            filters.Clear();
            sorts.Clear();
            count = 100;

            var input = string.Empty;
            while (input != "fetch")
            {
                System.Console.Write("Input: ");
                input = System.Console.ReadLine().Trim();
                if (input == "exit") { Environment.Exit(0); }
                else if (input == "cls") { System.Console.Clear(); }
                else if (input == "fetch") { continue; }
                else
                {
                    HandleInput(input);
                }
            }

            var parameters = new GetProductsRequest(count, filters, sorts);
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

        private void HandleInput(string input)
        {
            var splited = input.Split(' ');
            if (splited.Length == 2)
            {
                var type = splited[0];
                if (type == "count")
                {
                    count = int.Parse(splited[1]);
                }
            }
            if (splited.Length == 4)
            {
                var type = splited[0];
                if (type == "filter")
                {
                    var value = splited[3];
                    var operation = ToEnum(splited[2]);
                    var name = splited[1];
                    var filter = operation switch
                    {
                        NumberRelationalOperator num => CreateDoubleFilter(name, num, double.Parse(value)),
                        StringRelationalOperator str => CreateStringFilter(name, str, value),
                        _ => throw new NotImplementedException(),
                    };
                    filters.Add(filter);
                }
                if (type == "sort")
                {
                    var name = splited[2];
                    var direction = splited[3] == "asc" ? SortDirection.ASC : SortDirection.DESC;
                    var sort = CreateSort(name, direction);
                    sorts.Add(sort);
                }
            }
        }

        private ICollectionSort<Product> CreateSort(string name, SortDirection direction)
        {
            return name switch
            {
                "store_name" => new CollectionSort<Product, string>(x => x.StoreName, direction),
                "name" => new CollectionSort<Product, string>(x => x.Name, direction),
                "base_price" => new CollectionSort<Product, double>(x => x.BasePrice, direction),
                "discounted_price" => new CollectionSort<Product, double>(x => x.DiscountedPrice, direction),
                "discount_percentage" => new CollectionSort<Product, double>(x => x.DiscountPercentage, direction),
                "discount_value" => new CollectionSort<Product, double>(x => x.DiscountValue, direction),
                _ => throw new NotImplementedException(),
            };
        }

        private static Enum ToEnum(string value)
        {
            return value switch
            {
                ">=" => NumberRelationalOperator.GREATER_OR_EQUAL,
                ">" => NumberRelationalOperator.GREATER,
                "=" => NumberRelationalOperator.EQUAL,
                "!=" => NumberRelationalOperator.NOT_EQUAL,
                "<" => NumberRelationalOperator.LESS,
                "<=" => NumberRelationalOperator.LESS_OR_EQUAL,
                "equal" => StringRelationalOperator.EQUAL,
                "cotain" => StringRelationalOperator.CONTAIN,
                _ => throw new NotImplementedException()
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
    }
}
