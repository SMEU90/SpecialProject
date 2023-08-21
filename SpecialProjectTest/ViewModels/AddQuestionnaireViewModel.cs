using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using SpecialProjectTest.ViewModels.Base;
using SpecialProjectTest.Infrastructure.Commands.Base;
using SpecialProjectTest.Infrastructure.Commands;
using SpecialProjectTest.Services.Interfaces;
using System.Windows;
using SpecialProjectTest.Models;
using SpecialProjectTest.Infrastructure.ValidationRules;
using System.Globalization;
using GrpcServiceSpecialProjectTest;
using Grpc.Net.Client;
using SpecialProjectTest.Views.Windows;
using SpecialProjectTest.Mapper;
using System.Threading.Tasks;


namespace SpecialProjectTest.ViewModels
{
    class AddQuestionnaireViewModel : ViewModel
    {
        private readonly IUserDialog _UserDialog;
        private readonly IDataService _DataService;
        private ObservableCollection<Questionnaire> _allQuestionnaireItemsSource;
        private ObservableCollection<Country> _allCountryItemsSource;
        private Questionnaire _oldQuestionnaire;
        private bool _isNewQuestionnaire = true;
        public AddQuestionnaireViewModel(ObservableCollection<Questionnaire> AllQuestionnaireItemsSource, ObservableCollection<Country> AllCountryItemsSource) //Add
        {
            _Title = "Создание анкеты";
            AddButtonText = "Создать анкету";
            _allQuestionnaireItemsSource = AllQuestionnaireItemsSource;
            _allCountryItemsSource = AllCountryItemsSource;
            NewQuestionnaire = new Questionnaire();
            _oldQuestionnaire = new Questionnaire();
            NewQuestionnaire.Birthday = new DateTime(1980, 1, 1);
            OnPropertyChanged("NewWorker.Birthday");
            NewQuestionnaire.FirstName = "";
            NewQuestionnaire.MiddleName = "";
            NewQuestionnaire.LastName = "";
            AddNewQuestionnaireCmd = new LambdaCommand(OnOpenAddNewQuestionnaireCmdExecuted, CanOpenAddNewQuestionnaireCmdExecute);
            AddNewCountryWnd = new LambdaCommand(OnOpenAddNewCountryWndExecuted, CanOpenAddNewCountryWndExecute);
        }
        public AddQuestionnaireViewModel(Questionnaire questionnaire, ObservableCollection<Questionnaire> AllQuestionnaireItemsSource, ObservableCollection<Country> AllCountryItemsSource) //Edit
        {
            _Title = "Редактирование анкеты";
            AddButtonText = "Редактировать";
            _allQuestionnaireItemsSource = AllQuestionnaireItemsSource;
            _allCountryItemsSource = AllCountryItemsSource;
            NewQuestionnaire = new Questionnaire();
            NewQuestionnaire.Id= questionnaire.Id;
            NewQuestionnaire.FirstName = questionnaire.FirstName;
            NewQuestionnaire.MiddleName= questionnaire.MiddleName;
            NewQuestionnaire.LastName = questionnaire.LastName;
            NewQuestionnaire.Birthday=questionnaire.Birthday;
            NewQuestionnaire.Country= questionnaire.Country;
            OnPropertyChanged("NewQuestionnaire.Country");
            NewQuestionnaire.CountryId = questionnaire.CountryId;
            _oldQuestionnaire = questionnaire;
            _isNewQuestionnaire = false;
            AddNewQuestionnaireCmd = new LambdaCommand(OnOpenAddNewQuestionnaireCmdExecuted, CanOpenAddNewQuestionnaireCmdExecute);
            AddNewCountryWnd = new LambdaCommand(OnOpenAddNewCountryWndExecuted, CanOpenAddNewCountryWndExecute);
        }
        #region Свойства
        public Questionnaire NewQuestionnaire { get; set; }
        public string AddButtonText { get; set; }
        public ObservableCollection<Country> AllCountry { get => _allCountryItemsSource;}
        #endregion
        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "";

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion
        #region Command
        #region Command : AddNewQuestionnaireCmd - Add or Update Questionnaire
        public Command AddNewQuestionnaireCmd { get; }
        private bool CanOpenAddNewQuestionnaireCmdExecute(object p) => true;
        private void OnOpenAddNewQuestionnaireCmdExecuted(object p)
        {
            try
            {
                if (_isNewQuestionnaire)
                    AddNewQuestionnaire();
                else
                    EditQuestionnaire();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка");
                Debug.WriteLine(ex);
            }
        }
        #endregion

        #region AddNewQuestionnaire - Add new Questionnaire
        private async void AddNewQuestionnaire()
        {
            var validationRule = new AlphabetTextBoxValidationRule();
            if (validationRule.Validate(NewQuestionnaire.FirstName, new CultureInfo("ru-RU", false)).IsValid &&
                validationRule.Validate(NewQuestionnaire.MiddleName, new CultureInfo("ru-RU", false)).IsValid &&
                validationRule.Validate(NewQuestionnaire.LastName, new CultureInfo("ru-RU", false)).IsValid && NewQuestionnaire.Country!=null)
            {
                NewQuestionnaire.Id = 0;
                if(NewQuestionnaire.Country.Questionnaires == null)
                    NewQuestionnaire.Country.Questionnaires = new List<Questionnaire>();
                NewQuestionnaire.Country.Questionnaires.Add(NewQuestionnaire);
                if (!(await AddQuestionnaireGRPC())) MessageBox.Show("Ошибка при добавлении");
            }
            else MessageBox.Show("Проверьте корректность ввода!");



        }
        #endregion

        #region AddQuestionnaireGRPC - Add new Questionnaire in DB
        public async Task<bool> AddQuestionnaireGRPC()
        {
            bool flag = true;
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new ProductQuestionnaireService.ProductQuestionnaireServiceClient(channel);
                var response = await client.CreateQuestionnaireAsync(MyMapperQuestionnaire.QuestionnaireMapper(NewQuestionnaire));
                if (response == null)
                    flag = false;
                else
                {
                    _allQuestionnaireItemsSource.Add(MyMapperQuestionnaire.QuestionnaireProtoMapper(response));
                    _oldQuestionnaire = new Questionnaire();
                    _oldQuestionnaire = NewQuestionnaire;
                    NewQuestionnaire = new Questionnaire
                    {
                        Id = _oldQuestionnaire.Id,
                        FirstName = _oldQuestionnaire.FirstName,
                        MiddleName = _oldQuestionnaire.MiddleName,
                        LastName = _oldQuestionnaire.LastName,
                        Birthday = _oldQuestionnaire.Birthday,
                        Country = _oldQuestionnaire.Country,
                        CountryId= _oldQuestionnaire.Country.Id,
                    };
                    OnPropertyChanged("NewQuestionnaire");
                }
            }
            return flag;
        }
        #endregion

        #region EditQuestionnaire - Update Questionnaire
        private async void EditQuestionnaire()
        {
            if (_oldQuestionnaire.FirstName != NewQuestionnaire.FirstName || _oldQuestionnaire.MiddleName != NewQuestionnaire.MiddleName ||
                _oldQuestionnaire.LastName != NewQuestionnaire.LastName || _oldQuestionnaire.Birthday != NewQuestionnaire.Birthday ||
                _oldQuestionnaire.Country.Name != NewQuestionnaire.Country.Name)
            {
                var validationRule = new AlphabetTextBoxValidationRule();
                if (validationRule.Validate(NewQuestionnaire.FirstName, new CultureInfo("ru-RU", false)).IsValid &&
                validationRule.Validate(NewQuestionnaire.MiddleName, new CultureInfo("ru-RU", false)).IsValid &&
                validationRule.Validate(NewQuestionnaire.LastName, new CultureInfo("ru-RU", false)).IsValid && NewQuestionnaire.Country!=null)
                {
                    if (!(await EditQuestionnaireGRPC())) MessageBox.Show("Ошибка при редактировании");
                }
                else
                    MessageBox.Show("Проверьте корректность ввода!");
            }
            else
                MessageBox.Show("Вы не изменили ни одного поля");

        }
        #endregion

        #region EditQuestionnaireGRPC - Update Questionnaire in DB
        public async Task<bool> EditQuestionnaireGRPC()
        {
            bool flag = true;
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new ProductQuestionnaireService.ProductQuestionnaireServiceClient(channel);
                var response = await client.UpdateQuestionnaireAsync(MyMapperQuestionnaire.QuestionnaireMapper(NewQuestionnaire));
                if (response == null)
                    flag = false;
                else
                {
                    _allQuestionnaireItemsSource.Remove(_oldQuestionnaire);

                    Questionnaire questionnaire = new Questionnaire();
                    questionnaire.FirstName = NewQuestionnaire.FirstName;
                    questionnaire.MiddleName = NewQuestionnaire.MiddleName;
                    questionnaire.LastName = NewQuestionnaire.LastName;
                    questionnaire.Birthday = NewQuestionnaire.Birthday;
                    questionnaire.Country = NewQuestionnaire.Country;
                    questionnaire.Id = NewQuestionnaire.Id;
                    questionnaire.CountryId= NewQuestionnaire.CountryId;
                    _oldQuestionnaire = questionnaire;
                    _allQuestionnaireItemsSource.Add(questionnaire);
                }
                OnPropertyChanged("NewQuestionnaire");
            }
            return flag;
        }
        #endregion

        #region Command : AddNewCountryWnd - Add Country
        public Command AddNewCountryWnd { get; }
        private bool CanOpenAddNewCountryWndExecute(object p) => true;
        private void OnOpenAddNewCountryWndExecuted(object p)
        {
            AddCountryWnd newCountryWindow = new AddCountryWnd();
            SetCenterPositionAndOpen(newCountryWindow, _allCountryItemsSource);
            OnPropertyChanged("AllCountry");
        }
        #endregion
        #endregion
    }
}
