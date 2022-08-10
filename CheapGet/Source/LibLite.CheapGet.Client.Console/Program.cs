using LibLite.CheapGet.Client.Console.Extensions;
using LibLite.CheapGet.Client.Console.Services.Application;
using LibLite.DI.Lite;

using var container = new Container();
container.RegisterDependencies();

using var scope = container.CreateScope();
var application = scope.Get<IApplication>();

await application.StartAsync();