using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using TriviaXamarinApp.Models;
using TriviaXamarinApp.Services;
using TriviaXamarinApp.Views;
using System.Collections.ObjectModel;

namespace TriviaXamarinApp.ViewModels
{
    class LogInViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string entryEmail;
        public string EntryEmail
        {
            get => this.entryEmail;
            set
            {
                if (value != this.entryEmail)
                {
                    this.entryEmail = value;
                    OnPropertyChanged("EntryEmail");
                }
            }
        }

        private string entryPass;
        public string EntryPass
        {
            get => this.entryPass;
            set
            {
                if (value != this.entryPass)
                {
                    this.entryPass = value;
                    OnPropertyChanged("EntryPass");
                }
            }
        }

        public LogInViewModel()
        {
            
        }

        public ICommand LogInCommand => new Command(LogIn);
        private async void LogIn()
        {
            TriviaWebAPIProxy triviaWebAPIProxy = TriviaWebAPIProxy.CreateProxy();
            User user = await triviaWebAPIProxy.LoginAsync(EntryEmail, EntryPass);
            if (user != null)
            {
                TheMainTabbedPage theMainTabbedPage = (TheMainTabbedPage)Application.Current.MainPage;
                ((TheMainTabbedPageViewModel)(theMainTabbedPage).BindingContext).LoginUser = user;
                await App.Current.MainPage.DisplayAlert("Trivia", "You are logged in now!", "Ok");
                HomePageViewModel homePageViewModel = (HomePageViewModel)((theMainTabbedPage).home.BindingContext);
                if ((homePageViewModel.CounterCorrectAnswers > 0) && (homePageViewModel.CounterCorrectAnswers % 3 == 0))
                    theMainTabbedPage.AddTab((theMainTabbedPage).addQTab);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Trivia", "LogIn is failed, please try again", "Ok");
            }
            EntryPass = "";
            EntryEmail = "";
        }

        public ICommand RegisterCommand => new Command(GoToRegister);
        private async void GoToRegister()
        {
            ((TheMainTabbedPage)Application.Current.MainPage).CurrentTab(((TheMainTabbedPage)Application.Current.MainPage).register);
        }
    }
}
