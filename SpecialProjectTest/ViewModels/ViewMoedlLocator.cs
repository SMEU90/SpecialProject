using Microsoft.Extensions.DependencyInjection;

namespace SpecialProjectTest.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();
        public AddCountryViewModel AddCountryModel => App.Services.GetRequiredService<AddCountryViewModel>();
        public AddQuestionnaireViewModel AddQuestionnaireModel => App.Services.GetRequiredService<AddQuestionnaireViewModel>();
    }
}
