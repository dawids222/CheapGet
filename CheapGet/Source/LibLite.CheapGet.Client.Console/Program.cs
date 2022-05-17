using DI_Lite;
using LibLite.CheapGet.Business.Services.Serializers;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.GoG;
using LibLite.CheapGet.Core.Stores.Games.Steam;
using LibLite.CheapGet.DAL.Clients.Games;
using LibLite.CheapGet.DAL.Clients.Games.GoG;
using LibLite.CheapGet.DAL.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

Console.WriteLine("Hello World!");

using var container = new Container();
container.Factory(_ => new System.Net.Http.HttpClient());
container.Factory<IHttpClient, HttpClient>();
container.Scoped<ISteamClient, SteamClient>();
container.Scoped<IStoreClient, SteamClient>("Steam");
container.Scoped<IGogClient, GogClient>();
container.Scoped<IStoreClient, GogClient>("GoG");
container.Scoped<ISerializer, SystemTextJsonSerializer>();

var constructabilityReport = container.GetConstructabilityReport();
if (!constructabilityReport.IsConstructable)
{
    Console.WriteLine(constructabilityReport);
    return;
}

using var scope = container.CreateScope();

var client = scope.Get<IGogClient>();
var products = await client.GetDiscountedProductsAsync(0, 10, CancellationToken.None);
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