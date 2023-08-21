using Microsoft.Extensions.DependencyInjection;

namespace SpecialProjectTest.ViewModels
{
    internal static class ViewModelRegistrator
    {
        public static IServiceCollection AddViews(this IServiceCollection services)
        {
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<AddQuestionnaireViewModel>();
            services.AddSingleton<AddCountryViewModel>();
            return services;
        }
    }
}