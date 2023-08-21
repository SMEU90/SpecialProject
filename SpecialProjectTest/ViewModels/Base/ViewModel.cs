using SpecialProjectTest.Views.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using SpecialProjectTest.Models;

namespace SpecialProjectTest.ViewModels.Base
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));

        protected virtual bool Set<T>(ref T field, T value, [CallerMemberName] string PropertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(PropertyName);
            return true;
        }
        protected bool SetCenterPositionAndOpen(AddQuestionnaireWnd window, ObservableCollection<Questionnaire> AllQuestionnaireItemsSource, 
            ObservableCollection<Country> AllCountryItemsSource)//Add Questionnaire
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            AddQuestionnaireViewModel questionnaireViewModel = new AddQuestionnaireViewModel(AllQuestionnaireItemsSource, AllCountryItemsSource);
            window.DataContext = questionnaireViewModel;
            return (bool)window.ShowDialog();
        }
        protected bool SetCenterPositionAndOpen(Window window, AddQuestionnaireViewModel questionnaireViewModel)//Edit Questionnaire
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.DataContext = questionnaireViewModel;
            return (bool)window.ShowDialog();
        }
        protected bool SetCenterPositionAndOpen(AddCountryWnd window, ObservableCollection<Country> AllCountryItemsSource)//Add Country
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            AddCountryViewModel countryViewModel = new AddCountryViewModel(AllCountryItemsSource);
            window.DataContext = countryViewModel;
            return (bool)window.ShowDialog();
        }
        protected bool SetCenterPositionAndOpen(Window window, AddCountryViewModel countryViewModel)//Edit Country
        {
            window.Owner = Application.Current.MainWindow;
            window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            window.DataContext = countryViewModel;
            return (bool)window.ShowDialog();
        }
    }
}
