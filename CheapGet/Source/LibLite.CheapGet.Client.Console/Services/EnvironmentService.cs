using LibLite.CheapGet.Core.Services;
using System;
using System.Threading.Tasks;

namespace LibLite.CheapGet.Client.Console.Services
{
    public class EnvironmentService : IEnvironmentService
    {
        public Task ClearInputAsync() => Task.Run(() => System.Console.Clear());
        public Task ExitApplicationAsync() => Task.Run(() => Environment.Exit(0));
    }
}
