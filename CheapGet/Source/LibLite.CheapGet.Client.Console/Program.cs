using DI_Lite;
using LibLite.CheapGet.Business.Services.Serializers;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.Steam;
using LibLite.CheapGet.DAL.Services;
using LibLite.CheapGet.DAL.Services.Games;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

Console.WriteLine("Hello World!");

var container = new Container();
container.Scoped(_ => new System.Net.Http.HttpClient());
container.Scoped<IHttpClient, HttpClient>();
container.Scoped<ISteamClient, SteamClient>();
container.Scoped<IStoreClient, SteamClient>("Steam");
container.Scoped<ISerializer, SystemTextJsonSerializer>();

var constructabilityReport = container.GetConstructabilityReport();
if (!constructabilityReport.IsConstructable)
{
    Console.WriteLine(constructabilityReport);
    return;
}

var scope = container.CreateScope();

var steamClient = scope.Get<ISteamClient>();
var products = await steamClient.GetDiscountedProductsAsync(0, 100, CancellationToken.None);
Console.WriteLine(ToString(products));

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