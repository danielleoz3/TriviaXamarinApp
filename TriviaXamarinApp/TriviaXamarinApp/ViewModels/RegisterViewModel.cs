using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;
using TriviaXamarinApp.Models;
using TriviaXamarinApp.Services;
using TriviaXamarinApp.Views;
using System.Collections.ObjectModel;
using System;

namespace TriviaXamarinApp.ViewModels
{
    class RegisterViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string entryNickName;
        public string EntryNickName
        {
            get => this.entryNickName;
            set
            {
                if (value != this.entryNickName)
                {
                    this.entryNickName = value;
                    OnPropertyChanged("EntryNickName");
                }
            }
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

        public RegisterViewModel()
        {
            
        }

        public ICommand RegisterCommand => new Command(Register);
        private async void Register()
        {
            if ((EntryNickName == "") || (EntryEmail == "") || (EntryPass == ""))
            {
                await App.Current.MainPage.DisplayAlert("Trivia", "Please fill all the fields", "Ok");
                return;
            }
            User user = new User();
            user.NickName = EntryNickName;
            user.Email = EntryEmail;
            user.Password = EntryPass;
            TriviaWebAPIProxy triviaWebAPIProxy = TriviaWebAPIProxy.CreateProxy();
            bool isRegister = await triviaWebAPIProxy.RegisterUser(user);
            if (isRegister)
            {
                TheMainTabbedPage theMainTabbedPage = (TheMainTabbedPage)Application.Current.MainPage;
                ((TheMainTabbedPageViewModel)(theMainTabbedPage).BindingContext).LoginUser = user;
                await App.Current.MainPage.DisplayAlert("Trivia", "You are logged in now!", "Ok");
                HomePageViewModel homePageViewModel = (HomePageViewModel)((theMainTabbedPage).home.BindingContext);
                if ((homePageViewModel.CounterCorrectAnswers > 0) && (homePageViewModel.CounterCorrectAnswers %3 == 0))
                    theMainTabbedPage.AddTab((theMainTabbedPage).addQTab);
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Trivia", "Register is failed, please try enter another fields", "Ok");
            }
            EntryEmail = "";
            EntryNickName = "";
            EntryPass = "";
        }
    }
}
