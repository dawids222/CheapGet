using DI_Lite;
using LibLite.CheapGet.Business.Collections;
using LibLite.CheapGet.Business.Services.Reports;
using LibLite.CheapGet.Business.Services.Serializers;
using LibLite.CheapGet.Business.Services.Stores;
using LibLite.CheapGet.Core.Collections;
using LibLite.CheapGet.Core.Enums;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Services.Models;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.GoG;
using LibLite.CheapGet.Core.Stores.Games.Steam;
using LibLite.CheapGet.Core.Stores.Models;
using LibLite.CheapGet.DAL.Clients.Games;
using LibLite.CheapGet.DAL.Clients.Games.GoG;
using LibLite.CheapGet.DAL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using StringRelationalOperator = LibLite.CheapGet.Core.Enums.StringRelationalOperator;

Console.WriteLine("Hello World!");

using var container = new Container();
container.Factory(_ => new System.Net.Http.HttpClient());
container.Scoped<IFileService, FileService>();
container.Scoped<IResourceService, FileService>();
container.Scoped<IReportGenerator, HtmlReportGenerator>();
container.Scoped<ISerializer, SystemTextJsonSerializer>();
container.Factory<IHttpClient, HttpClient>();
container.Scoped<ISteamClient, SteamClient>();
container.Scoped<IStoreClient, SteamClient>("Steam");
container.Scoped<IGogClient, GogClient>();
container.Scoped<IStoreClient, GogClient>("GoG");
container.Scoped<IStoreService>("Games", provider =>
{
    var names = new[] { "Steam", "GoG" };
    var stores = names
        .Select(name => provider.Get<IStoreClient>(name))
        .ToList();
    return new StoreService(stores);
});

var constructabilityReport = container.GetConstructabilityReport();
if (!constructabilityReport.IsConstructable)
{
    Console.WriteLine(constructabilityReport);
    return;
}

using var scope = container.CreateScope();
var service = scope.Get<IStoreService>("Games");
var reportGenerator = scope.Get<IReportGenerator>();
var fileService = scope.Get<IFileService>();

var count = 0;
var filters = new List<ICollectionFilter<Product>>();
var sorts = new List<ICollectionSort<Product>>();

while (true)
{
    try
    {
        await MakeRequest();
    }
    catch (Exception ex) { }
}

async Task MakeRequest()
{
    filters.Clear();
    sorts.Clear();

    var input = string.Empty;
    Console.Write("Count: ");
    count = int.Parse(Console.ReadLine().Trim());
    while (input != "fetch")
    {
        Console.Write("Input: ");
        input = Console.ReadLine().Trim();
        if (input == "exit") { Environment.Exit(0); }
        else if (input == "cls") { Console.Clear(); }
        else if (input == "fetch") { continue; }
        else
        {
            HandleInput(input);
        }
    }

    var parameters = new GetProductsRequest(count, filters, sorts);
    var products = await service.GetDiscountedProductsAsync(parameters, CancellationToken.None);
    var report = await reportGenerator.GenerateReportAsync(products);
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
    await fileService.SaveAsync(file);
    fileService.Open(file);
    await Task.Delay(1000);
    fileService.Delete(file);
}

void HandleInput(string input)
{
    var splited = input.Split(' ');
    if (splited.Length == 4)
    {
        var type = splited[0];
        // filter base_price >= 100
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
        // sort by name asc
        if (type == "sort")
        {
            var name = splited[2];
            var direction = splited[3] == "asc" ? SortDirection.ASC : SortDirection.DESC;
            var sort = CreateSort(name, direction);
            sorts.Add(sort);
        }
    }
}

ICollectionSort<Product> CreateSort(string name, SortDirection direction)
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

Enum ToEnum(string value)
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

ICollectionFilter<Product> CreateDoubleFilter(string name, NumberRelationalOperator @operator, double value)
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

ICollectionFilter<Product> CreateStringFilter(string name, StringRelationalOperator operation, string value)
{
    return name switch
    {
        "name" => new CollectionStringFilter<Product>(x => x.Name, operation, value),
        "store_name" => new CollectionStringFilter<Product>(x => x.StoreName, operation, value),
        _ => throw new NotImplementedException(),
    };
}

static string ToString(IEnumerable<Product> products)
{
    var stringBuilder = new StringBuilder();
    foreach (var product in products)
    {
        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine($"    StoreName: {product.StoreName},");
        stringBuilder.AppendLine($"    Name: {product.Name}");
        stringBuilder.AppendLine($"    BasePrice: {product.BasePrice}");
        stringBuilder.AppendLine($"    DiscountedPrice: {product.DiscountedPrice}");
        stringBuilder.AppendLine($"    DiscountPercentage: {product.DiscountPercentage}");
        stringBuilder.AppendLine($"    DiscountValue: {product.DiscountValue}");
        stringBuilder.AppendLine("},");
    }
    return stringBuilder.ToString();
}