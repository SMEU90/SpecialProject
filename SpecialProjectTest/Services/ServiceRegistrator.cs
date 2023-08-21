using Microsoft.Extensions.DependencyInjection;
using SpecialProjectTest.Services.Interfaces;

namespace SpecialProjectTest.Services
{
    internal static class ServiceRegistrator
    {
        public static IServiceCollection AddServices(this IServiceCollection services) => services
           .AddTransient<IDataService, DataService>()
           .AddTransient<IUserDialog, UserDialog>()
        ;
    }
}
