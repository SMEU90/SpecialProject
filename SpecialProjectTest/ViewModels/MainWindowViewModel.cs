using SpecialProjectTest.Infrastructure.Commands.Base;
using SpecialProjectTest.Services.Interfaces;
using SpecialProjectTest.ViewModels.Base;
using SpecialProjectTest.Views.Windows;
using System;
using System.Collections.ObjectModel;
using SpecialProjectTest.Models;
using System.Collections.Generic;
using System.Diagnostics;
using Grpc.Net.Client;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using SpecialProjectTest.Infrastructure.Commands;
using GrpcServiceSpecialProjectTest;
using SpecialProjectTest.Mapper;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;


namespace SpecialProjectTest.ViewModels
{
    internal class MainWindowViewModel : ViewModel
    {
        private readonly IUserDialog _UserDialog;
        private readonly IDataService _DataService;
        private string _filePath;

        #region Title : string - Заголовок окна

        /// <summary>Заголовок окна</summary>
        private string _Title = "Тестовое здание ООО УК СпецПроект Дегтерев С.О.";

        /// <summary>Заголовок окна</summary>
        public string Title { get => _Title; set => Set(ref _Title, value); }

        #endregion

        #region Status : string - Статус

        /// <summary>Статус</summary>
        private string _Status = "Готов!";

        /// <summary>Статус</summary>
        public string Status { get => _Status; set => Set(ref _Status, value); }

        #endregion

        public MainWindowViewModel(IUserDialog UserDialog, IDataService DataService)
        {
            _UserDialog = UserDialog;
            _DataService = DataService;
            //получение всех стран
            GetAllCountryFromDB();
            //получение всех анкет
            GetAllQuestionnaireFromDB();
            OpenAddNewQuestionnaireWnd = new LambdaCommand(OnOpenAddNewQuestionnaireWndExecuted, CanOpenAddNewQuestionnaireWndExecute);
            DeleteQuestionnaire = new LambdaCommand(OnDeleteQuestionnaireExecuted, CanDeleteQuestionnaireExecute);
            EditQuestionnaire = new LambdaCommand(OnEditQuestionnaireExecuted, CanEditQuestionnaireExecute);
            OpenAddNewCountryWnd = new LambdaCommand(OnOpenAddNewCountryWndExecuted, CanOpenAddNewCountryWndExecute);
            DeleteCountry = new LambdaCommand(OnDeleteCountryExecuted, CanDeleteCountryExecute);
            EditCountry = new LambdaCommand(OnEditCountryExecuted, CanEditCountryExecute);
            DownloadQuestionnaireCmd = new LambdaCommand(OnDownloadQuestionnaireCmdExecuted, CanDownloadQuestionnaireCmdExecute);
            SelectFolderLocationCmd = new LambdaCommand(OnOpenSelectFolderLocationCmdExecuted, CanOpenSelectFolderLocationCmdExecute);
            SaveXMLFileCmd = new LambdaCommand(OnOpenSaveXMLFileCmdExecuted, CanOpenSaveXMLFileCmdExecute);
            SaveAllXMLFileCmd = new LambdaCommand(OnOpenSaveAllXMLFileCmdExecuted, CanOpenSaveAllXMLFileCmdExecute);
            CanSaveFile = false;
        }

        #region GetAllCountryFromDB -  получение всех стран
        private void GetAllCountryFromDB()
        {
            AllCountryItemsSource = new ObservableCollection<Country>();
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new ProductCountryService.ProductCountryServiceClient(channel);
                var listAllCountry = client.GetAllCountry(new EmptyMessageCountry());
                if (listAllCountry.Items.Count > 0)
                    foreach (var el in listAllCountry.Items)
                        AllCountryItemsSource.Add(MyMapperCountry.CountryProtoMapper(el));
            }
        }
        #endregion
        #region GetAllQuestionnaireFromDB -  получение всех анкет
        private void GetAllQuestionnaireFromDB()
        {
            AllQuestionnaireItemsSource = new ObservableCollection<Questionnaire>();
            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))
            {
                var client = new ProductQuestionnaireService.ProductQuestionnaireServiceClient(channel);
                var listAllQuestionnaire = client.GetAllQuestionnaire(new EmptyMessageQuestionnaire());
                if (listAllQuestionnaire.Items.Count > 0)
                    foreach (var el in listAllQuestionnaire.Items)
                        AllQuestionnaireItemsSource.Add(MyMapperQuestionnaire.QuestionnaireProtoMapper(el, AllCountryItemsSource));
            }
        }
        #endregion
        #region Анкета
        #region Свойства привязки к DataGrid
        public Questionnaire QuestionnaireSelectedItem { get; set; }
        public ObservableCollection<Questionnaire> AllQuestionnaireItemsSource { get; set; }


        #endregion

        #region Command : OpenAddNewQuestionnaireWnd - открытие окна добавления анкеты
        public Command OpenAddNewQuestionnaireWnd { get; }
        private bool CanOpenAddNewQuestionnaireWndExecute(object p) => true;
        private void OnOpenAddNewQuestionnaireWndExecuted(object p)
        {
            AddQuestionnaireWnd newQuestionnaireWindow = new AddQuestionnaireWnd();
            SetCenterPositionAndOpen(newQuestionnaireWindow, AllQuestionnaireItemsSource, AllCountryItemsSource);
        }
        #endregion

        #region Command : DeleteQuestionnaire  - удаление анкеты
        public Command DeleteQuestionnaire { get; }
        private bool CanDeleteQuestionnaireExecute(object p) => true;
        private void OnDeleteQuestionnaireExecuted(object p)
        {
            DeleteQuestionnaireAsync();
        }
        public async void DeleteQuestionnaireAsync()
        {
            try
            {
                using var channel = GrpcChannel.ForAddress("https://localhost:5001");
                var client = new ProductQuestionnaireService.ProductQuestionnaireServiceClient(channel);
                var response = await client.DeleteQuestionnaireAsync(MyMapperQuestionnaire.QuestionnaireMapper(QuestionnaireSelectedItem));
                if (response == null)
                    System.Windows.MessageBox.Show("Ошибка при удалении");
                else AllQuestionnaireItemsSource.Remove(QuestionnaireSelectedItem);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка при удалении");
                Debug.WriteLine(ex);
            }
        }
        #endregion

        #region Command : EditQuestionnaire  - редактирование анкеты
        public Command EditQuestionnaire { get; }
        private bool CanEditQuestionnaireExecute(object p) => true;
        private void OnEditQuestionnaireExecuted(object p)
        {
            AddQuestionnaireWnd newQuestionnaireWindow = new AddQuestionnaireWnd();
            AddQuestionnaireViewModel newQuestionnaireVM = new AddQuestionnaireViewModel(QuestionnaireSelectedItem, AllQuestionnaireItemsSource, AllCountryItemsSource);
            SetCenterPositionAndOpen(newQuestionnaireWindow, newQuestionnaireVM);
        }
        #endregion

        #endregion
        #region Страна
        #region Свойства привязки к DataGrid
        public Country CountrySelectedItem { get; set; }
        public ObservableCollection<Country> AllCountryItemsSource { get; set; }


        #endregion

        #region Command : OpenAddNewCountryWnd - открытие окна добавления страны
        public Command OpenAddNewCountryWnd { get; }
        private bool CanOpenAddNewCountryWndExecute(object p) => true;
        private void OnOpenAddNewCountryWndExecuted(object p)
        {
            AddCountryWnd newCountryWindow = new AddCountryWnd();
            SetCenterPositionAndOpen(newCountryWindow, AllCountryItemsSource);
        }
        #endregion

        #region Command : DeleteCountry  - удаление страны
        public Command DeleteCountry { get; }
        private bool CanDeleteCountryExecute(object p) => true;
        private void OnDeleteCountryExecuted(object p)
        {
            DeleteCountryAsync();
        }
        public async void DeleteCountryAsync()
        {
            try
            {
                var flag = true;
                foreach (var el in AllQuestionnaireItemsSource)
                    if (el.Country.Name == CountrySelectedItem.Name)
                        flag = false;
                if (flag)
                {
                    using var channel = GrpcChannel.ForAddress("https://localhost:5001");
                    var client = new ProductCountryService.ProductCountryServiceClient(channel);
                    var response = await client.DeleteCountryAsync(MyMapperCountry.CountryMapper(CountrySelectedItem));
                    if (response == null)
                        System.Windows.MessageBox.Show("Ошибка при удалении");
                    else AllCountryItemsSource.Remove(CountrySelectedItem);
                } else System.Windows.MessageBox.Show("Невозможно удалить!\nДанная страна используется");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка при удалении");
                Debug.WriteLine(ex);
            }
        }
        #endregion

        #region Command : EditCountry  - редактирование страны
        public Command EditCountry { get; }
        private bool CanEditCountryExecute(object p) => true;
        private void OnEditCountryExecuted(object p)
        {
            AddCountryWnd newPersonalWindow = new AddCountryWnd();
            SetCenterPositionAndOpen(newPersonalWindow, new AddCountryViewModel(CountrySelectedItem, AllCountryItemsSource));
        }
        #endregion
        private async Task<bool> DownloadXMLElement(XElement el)
        {
            try
            {
#nullable enable
                XAttribute? id = el.Attribute("Id");
                XAttribute? firstName = el.Attribute("FirstName");
                XAttribute? middleName = el.Attribute("MiddleName");
                XAttribute? lastName = el.Attribute("LastName");
                XAttribute? birthday = el.Attribute("Birthday");
                XAttribute? countryId = el.Attribute("CountryId");
                XElement? countryName = el.Element("CountryName");
#nullable disable
                if (firstName != null && middleName != null && lastName != null
                    && birthday != null && countryName != null && id != null && countryId != null)
                {
                    var flag = true;
                    foreach (var element in AllQuestionnaireItemsSource)//поиск анкеты в AllQuestionnaireItemsSource
                        if (element.Id == Int32.Parse(id.Value)) flag = false;//найдена, ничего не делаем
                    if (flag)//не найдена, создаем новую
                    {
                        var questionnaire = new Questionnaire
                        {
                            Id = Int32.Parse(id.Value),
                            CountryId = Int32.Parse(countryId.Value),
                            FirstName = firstName.Value,
                            MiddleName = middleName.Value,
                            LastName = lastName.Value,
                            Birthday = DateTime.Parse(birthday.Value),
                        };
                        var check = true;
                        foreach (var element in AllCountryItemsSource)//поиск страны в AllCountryItemsSource
                            if (element.Id == Int32.Parse(countryId.Value)) check = false;//найдена
                        if (check)//не найдена, создаем новую
                        {
                            using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))//проверяем появилась ли в БД данная страна
                            {
                                var client = new ProductCountryService.ProductCountryServiceClient(channel);
                                var checkCountryName = await client.CheckCountryAsync(new CheckCountryRequest { Name = countryName.Value });
                                if (checkCountryName.IsCheked)//если появилась в бд
                                {
                                    var response = await client.GetCountryAsync(new CheckCountryRequest { Name = countryName.Value });
                                    AllCountryItemsSource.Add(MyMapperCountry.CountryProtoMapper(response));
                                    questionnaire.Country = MyMapperCountry.CountryProtoMapper(response);

                                }
                                else// если не появилась в бд
                                {
                                    var newCountry = new Country();
                                    newCountry.Id = Int32.Parse(countryId.Value);
                                    newCountry.Name = countryName.Value;
                                    questionnaire.Country = newCountry;
                                    newCountry.Questionnaires = new List<Questionnaire>();
                                    newCountry.Questionnaires.Add(questionnaire);
                                    var response = await client.CreateCountryAsync(MyMapperCountry.CountryMapper(newCountry));
                                    questionnaire.CountryId = response.Id;
                                    AllCountryItemsSource.Add(MyMapperCountry.CountryProtoMapper(response));
                                }
                            }
                        }
                        else
                            questionnaire.Country = AllCountryItemsSource.FirstOrDefault(p => p.Id == questionnaire.CountryId);
                        using (var channel = GrpcChannel.ForAddress("https://localhost:5001"))//добавляем в бд
                        {
                            var client = new ProductQuestionnaireService.ProductQuestionnaireServiceClient(channel);
                            var response = await client.CreateQuestionnaireAsync(MyMapperQuestionnaire.QuestionnaireMapper(questionnaire));
                            AllQuestionnaireItemsSource.Add(questionnaire);
                        }
                    }
                    return true;
                }
            } catch(Exception ex) { throw ex;}
            return false;
        }
        #region Command : DownloadQuestionnaireCmd  - загрузка анкеты
        public Command DownloadQuestionnaireCmd { get; }
        private bool CanDownloadQuestionnaireCmdExecute(object p) => true;
        private async void OnDownloadQuestionnaireCmdExecuted(object p)
        {
            System.Windows.MessageBox.Show("Если анкета уже создана, то она не будет загружена из файла");
            try
            {
                var dialog = new Microsoft.Win32.OpenFileDialog();
                if (dialog.ShowDialog() == true)
                {
                    var filePath = dialog.FileName;
                    XDocument xmlDocument = XDocument.Load(filePath);
                    XElement allQuestionnaire = xmlDocument.Element("AllQuestionnaire");
                    if(allQuestionnaire!=null)
                    {
                        foreach (var el in allQuestionnaire.Elements("Questionnaire"))
                        {
                            if (!await DownloadXMLElement(el))
                                throw new Exception();
                        }
                    }
                }
                else
                    System.Windows.MessageBox.Show("Файл не выбран!");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{ex.Message}");
                System.Windows.MessageBox.Show("Ошибка при чтении файла!");
            }

        }
        #endregion

        #endregion 
        #region Сохранение/получение анкет
        public bool CanSaveFile { get; set; }
        #region Command : SaveALLXMLFileCmd - Save all Questionnaire xml-file in select folder
        public Command SaveAllXMLFileCmd { get; }
        private bool CanOpenSaveAllXMLFileCmdExecute(object p) => CanSaveFile;
        private async void OnOpenSaveAllXMLFileCmdExecuted(object p)
        {
            try
            {
                int integrator = 0;
                while (true)
                {
                    if (File.Exists(_filePath + @"\" + "AllQuestionnaire" + integrator.ToString() +".xml"))
                        integrator++;
                    else break;
                }
                XDocument xmlDocument = new XDocument();
                XElement allQuestionnaireElement = new XElement("AllQuestionnaire");
                foreach (var el in AllQuestionnaireItemsSource)
                {
                    XElement questionnaireElement = new XElement("Questionnaire");
                    /*XAttribute xattrId = new XAttribute("Id", el.Id.ToString());
                    XAttribute xattrFirstName = new XAttribute("FirstName", el.FirstName);
                    XAttribute xattrMiddleName = new XAttribute("MiddleName", el.MiddleName);
                    XAttribute xattrLastName = new XAttribute("LastName", el.LastName);
                    XAttribute xattrBirthday = new XAttribute("Id", el.Birthday.ToString());
                    XAttribute xattrCountryId = new XAttribute("CountryId", el.CountryId.ToString());
                    XElement countryNameElement = new XElement("CountryName", el.Country.Name);*/
                    questionnaireElement.Add(new XAttribute("Id", el.Id.ToString()));
                    questionnaireElement.Add(new XAttribute("FirstName", el.FirstName));
                    questionnaireElement.Add(new XAttribute("MiddleName", el.MiddleName));
                    questionnaireElement.Add(new XAttribute("LastName", el.LastName));
                    questionnaireElement.Add(new XAttribute("Birthday", el.Birthday.ToString()));
                    questionnaireElement.Add(new XAttribute("CountryId", el.CountryId.ToString()));
                    questionnaireElement.Add(new XElement("CountryName", el.Country.Name));
                    allQuestionnaireElement.Add(questionnaireElement);
                }
                xmlDocument.Add(allQuestionnaireElement);

                await using var stream = new FileStream(_filePath + @"\" + "AllQuestionnaire" + integrator.ToString() + ".xml",
                    FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 4096,
                    useAsync: true);

                await xmlDocument.SaveAsync(stream, new SaveOptions(), new CancellationToken());
            }
            catch (Exception ex) { Debug.WriteLine(ex); System.Windows.MessageBox.Show("Ошибка при записи!"); }
        }
        #endregion
        #region Command : SelectFolderLocationCmd - Select folder location to save xml-file
        public Command SelectFolderLocationCmd { get; }
        private bool CanOpenSelectFolderLocationCmdExecute(object p) => true;
        private void OnOpenSelectFolderLocationCmdExecuted(object p)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(dialog.SelectedPath) && dialog.SelectedPath != "")
                {
                    _filePath = dialog.SelectedPath;
                    CanSaveFile = true;
                }
                else
                {
                    CanSaveFile = false;
                    System.Windows.MessageBox.Show("Ошибка при выборе файла!");
                }
            }
        }
        #endregion
        #region Command : SaveXMLFileCmd - Save xml-file in select folder
        public Command SaveXMLFileCmd { get; }
        private bool CanOpenSaveXMLFileCmdExecute(object p) => CanSaveFile;
        private void OnOpenSaveXMLFileCmdExecuted(object p)
        {
                try
                {
                XDocument xmlDocument = new XDocument();
                XElement allQuestionnaireElement = new XElement("AllQuestionnaire");
                var questionnaire = new XElement("Questionnaire",
                        new XAttribute("Id", QuestionnaireSelectedItem.Id.ToString()),
                        new XAttribute("FirstName", QuestionnaireSelectedItem.FirstName),
                        new XAttribute("MiddleName", QuestionnaireSelectedItem.MiddleName),
                        new XAttribute("LastName", QuestionnaireSelectedItem.LastName),
                        new XAttribute("Birthday", QuestionnaireSelectedItem.Birthday.ToString()),
                        new XAttribute("CountryId", QuestionnaireSelectedItem.CountryId.ToString()),
                        new XElement("CountryName", QuestionnaireSelectedItem.Country.Name));
                allQuestionnaireElement.Add(questionnaire);
                xmlDocument.Add(allQuestionnaireElement);
                    int integrator = 0;
                    while (true)
                    {
                        if (File.Exists(_filePath + @"\" + QuestionnaireSelectedItem.Id.ToString() + QuestionnaireSelectedItem.LastName + QuestionnaireSelectedItem.FirstName + QuestionnaireSelectedItem.MiddleName + ".xml"))
                            integrator++;
                        else
                        {
                            xmlDocument.Save(_filePath + @"\" + QuestionnaireSelectedItem.Id.ToString() + QuestionnaireSelectedItem.LastName + QuestionnaireSelectedItem.FirstName +
                            QuestionnaireSelectedItem.MiddleName + ".xml");
                            break;
                        }
                        if (File.Exists(_filePath + @"\" + QuestionnaireSelectedItem.Id.ToString() + QuestionnaireSelectedItem.LastName + QuestionnaireSelectedItem.FirstName +
                            QuestionnaireSelectedItem.MiddleName + integrator.ToString() + ".xml"))
                            integrator++;
                        else
                        {
                            xmlDocument.Save(_filePath + @"\" + QuestionnaireSelectedItem.Id.ToString() + QuestionnaireSelectedItem.LastName + QuestionnaireSelectedItem.FirstName +
                            QuestionnaireSelectedItem.MiddleName + integrator.ToString() + ".xml");
                            break;
                        }
                    }
                }
                catch (Exception ex) { Debug.WriteLine(ex); System.Windows.MessageBox.Show("Ошибка при записи!"); }
        }
        #endregion
        #endregion
    }
}
