﻿using LibLite.CheapGet.Business.Services.Reports;
using LibLite.CheapGet.Business.Services.Serializers;
using LibLite.CheapGet.Business.Services.Stores;
using LibLite.CheapGet.Client.Console.Consts;
using LibLite.CheapGet.Client.Console.Services.Application;
using LibLite.CheapGet.Core.Services;
using LibLite.CheapGet.Core.Stores;
using LibLite.CheapGet.Core.Stores.Games.GoG;
using LibLite.CheapGet.Core.Stores.Games.Steam;
using LibLite.CheapGet.DAL.Clients.Games;
using LibLite.CheapGet.DAL.Clients.Games.GoG;
using LibLite.CheapGet.DAL.Services;
using LibLite.DI.Lite;
using System;
using System.Linq;

namespace LibLite.CheapGet.Client.Console.Extensions
{
    public static class ContainerExtensions
    {
        public static void RegisterDependencies(this Container container)
        {
            container.Factory(_ => new System.Net.Http.HttpClient());
            container.Factory<IHttpClient, HttpClient>();

            container.Scoped<IFileService, FileService>();
            container.Scoped(provider => (IResourceService)provider.Get<IFileService>());

            container.Scoped<IReportGenerator, HtmlReportGenerator>();
            container.Scoped<ISerializer, SystemTextJsonSerializer>();

            container.Scoped<ISteamClient, SteamClient>();
            container.Scoped<IGogClient, GogClient>();

            container.Scoped(Tags.Stores.Steam, provider => (IStoreClient)provider.Get<ISteamClient>());
            container.Scoped(Tags.Stores.GoG, provider => (IStoreClient)provider.Get<IGogClient>());

            container.Scoped<IStoreService>(Tags.StoreServices.Games, provider =>
            {
                var names = new[] { Tags.Stores.Steam, Tags.Stores.GoG };
                var stores = names
                    .Select(name => provider.Get<IStoreClient>(name))
                    .ToList();
                return new StoreService(stores);
            });

            container.Scoped<IApplication, TemporaryApplication>();

            var constructabilityReport = container.GetConstructabilityReport();
            if (!constructabilityReport.IsConstructable)
            {
                foreach (var report in constructabilityReport.FailedConstructabilityReports)
                {
                    System.Console.WriteLine(report.Error);
                }
                Environment.Exit(1);
            }
        }
    }
}
