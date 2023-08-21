using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using SpecialProjectTest.Infrastructure.Commands.Base;
using System.Windows;
using SpecialProjectTest.Models;
using SpecialProjectTest.Services.Interfaces;
using SpecialProjectTest.ViewModels.Base;
using SpecialProjectTest.Infrastructure.ValidationRules;
using System.Globalization;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcServiceSpecialProjectTest;
using SpecialProjectTest.Mapper;
using SpecialProjectTest.Infrastructure.Commands;

namespace SpecialProjectTest.ViewModels
{
    class AddCountryViewModel : ViewModel
    {
        private readonly IUserDialog _UserDialog;
        private readonly IDataService _DataService;
        private ObservableCollection<Country> _allCountryItemsSource;
        private Country _oldCountry;
        private bool _isNewCountry = true;
        public AddCountryViewModel(ObservableCollection<Country> AllCountryItemsSource)
        {
            _Title = "Создание страны";
            AddButtonText = "Создать страну";
            _allCountryItemsSource = AllCountryItemsSource;
            NewCountry = new Country();
            _oldCountry = new Country();
            NewCountry.Name = "";
            AddNewCountryCmd = new LambdaCommand(OnOpenAddNewCountryCmdExecuted, CanOpenAddNewCountryCmdExecute);
        }
        public AddCountryViewModel(Country country, ObservableCollection<Country> AllCountryItemsSource)
        {
            _Title = "Редактирование страны";
            AddButtonText = "Редактировать";
            _allCountryItemsSource =AllCountryItemsSource;
            NewCountry = new Country();
            NewCountry.Name = country.Name;
            NewCountry.Id = country.Id;
            NewCountry.Questionnaires = country.Questionnaires;
            _oldCountry = country;
            _isNewCountry = false;
            AddNewCountryCmd = new LambdaCommand(OnOpenAddNewCountryCmdExecuted, CanOpenAddNewCountryCmdExecute);
        }
        #region Свойства
        public Country NewCountry { get; set; }
        public string AddButtonText { get; set; }
        #endregion
        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "";

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion
        #region Command : AddNewCountryCmd - Add or Update Country
        public Command AddNewCountryCmd { get; }
        private bool CanOpenAddNewCountryCmdExecute(object p) => true;
        private void OnOpenAddNewCountryCmdExecuted(object p)
        {
            try
            {
                if (_isNewCountry)
                    AddNewCountry();
                else
                    EditCountry();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
                Debug.WriteLine(ex);
            }
        }
        #endregion
        #region AddNewCountry - Add new Country
        private async void AddNewCountry()
        {
            var validationRule = new AlphabetTextBoxValidationRule();
            if (validationRule.Validate(NewCountry.Name, new CultureInfo("ru-RU", false)).IsValid)
            {
                NewCountry.Id = 0;
                if (!(await AddCountryGRPC())) MessageBox.Show("Ошибка при добавлении");
            } else MessageBox.Show("Проверьте корректность ввода!");



        }
        #endregion

        #region AddCountryGRPC - Add new Country in DB
        public async Task<bool> AddCountryGRPC()
        {
            bool flag = true;
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new ProductCountryService.ProductCountryServiceClient(channel);
                var check = await client.CheckCountryAsync(new CheckCountryRequest { Name = NewCountry.Name });
                if (check.IsCheked)
                {
                    MessageBox.Show("Данная страна уже существует");
                    return false;
                }
                var response = await client.CreateCountryAsync(MyMapperCountry.CountryMapper(NewCountry));
                if (response == null)
                    flag = false;
                else
                {
                    _allCountryItemsSource.Add(MyMapperCountry.CountryProtoMapper(response));
                    _oldCountry = new Country();
                    _oldCountry = NewCountry;
                    NewCountry = new Country
                    {
                        Name = _oldCountry.Name,
                    };
                    OnPropertyChanged("NewCountry");
                }
            }
            return flag;
        }
        #endregion

        #region EditCountry - Update Country
        private async void EditCountry()
        {
            if (_oldCountry.Name != NewCountry.Name)
            {
                var validationRule = new AlphabetTextBoxValidationRule();
                if (validationRule.Validate(NewCountry.Name, new CultureInfo("ru-RU", false)).IsValid)
                {
                    if (!(await EditCountryGRPC())) MessageBox.Show("Ошибка при редактировании");
                }
                else
                    MessageBox.Show("Проверьте корректность ввода!");
            }
            else
                MessageBox.Show("Вы не изменили ни одного поля");

        }
        #endregion

        #region EditCountryGRPC - Update Country in DB
        public async Task<bool> EditCountryGRPC()
        {
            bool flag = true;
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new ProductCountryService.ProductCountryServiceClient(channel);
                var check = await client.CheckCountryAsync(new CheckCountryRequest { Name= NewCountry.Name });
                if(check.IsCheked)
                {
                    MessageBox.Show("Данная страна уже существует");
                    return false;
                }
                var response = await client.UpdateCountryAsync(MyMapperCountry.CountryMapper(NewCountry));
                if (response == null)
                    flag = false;
                else
                {
                    Country country = new Country();
                    country.Name = NewCountry.Name;
                    country.Id = _oldCountry.Id;
                    country.Questionnaires= _oldCountry.Questionnaires;
                    _allCountryItemsSource.Remove(_oldCountry);
                    _oldCountry = country;
                    _allCountryItemsSource.Add(country);
                }
                OnPropertyChanged("NewCountry");
            }
            return flag;
        }
        #endregion

    }
}
