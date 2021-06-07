
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using TriviaXamarinApp.Services;
using TriviaXamarinApp.Models;
using TriviaXamarinApp.Views;
using System.Collections.ObjectModel;

namespace TriviaXamarinApp.ViewModels
{
    class AddQuestionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private string entryQText;
        public string EntryQText
        {
            get => this.entryQText;
            set
            {
                if (value != this.entryQText)
                {
                    this.entryQText = value;
                    OnPropertyChanged("EntryQText");
                }
            }
        }

        private string entryCorrectAns;
        public string EntryCorrectAns
        {
            get => this.entryCorrectAns;
            set
            {
                if (value != this.entryCorrectAns)
                {
                    this.entryCorrectAns = value;
                    OnPropertyChanged("EntryCorrectAns");
                }
            }
        }

        private string entryOtherAns1;
        public string EntryOtherAns1 
        {
            get => this.entryOtherAns1;
            set
            {
                if (value != this.entryOtherAns1)
                {
                    this.entryOtherAns1 = value;
                    OnPropertyChanged("EntryOtherAns1");
                }
            }
        }

        private string entryOtherAns2;
        public string EntryOtherAns2
        {
            get => this.entryOtherAns2;
            set
            {
                if (value != this.entryOtherAns2)
                {
                    this.entryOtherAns2 = value;
                    OnPropertyChanged("EntryOtherAns2");
                }
            }
        }

        private string entryOtherAns3;
        public string EntryOtherAns3
        {
            get => this.entryOtherAns3;
            set
            {
                if (value != this.entryOtherAns3)
                {
                    this.entryOtherAns3 = value;
                    OnPropertyChanged("EntryOtherAns3");
                }
            }
        }
        
        public ICommand AddQuestionCommand => new Command(AddQuestion);
        private async void AddQuestion()
        {
            if ((EntryQText == "") || (EntryCorrectAns == "") || (EntryOtherAns1 == "") || (EntryOtherAns2 == "") || (EntryOtherAns3 == ""))
            {
                await App.Current.MainPage.DisplayAlert("Trivia", "Please fill all the fields", "Ok");
                return;
            }
            AmericanQuestion americanQuestion = new AmericanQuestion();
            americanQuestion.QText = EntryQText;
            americanQuestion.CorrectAnswer = EntryCorrectAns;
            americanQuestion.OtherAnswers = new string[3] { EntryOtherAns1, EntryOtherAns2, EntryOtherAns3 };
            TheMainTabbedPageViewModel theMainTabbedPageViewModel = (TheMainTabbedPageViewModel)((TheMainTabbedPage)Application.Current.MainPage).BindingContext;
            User user = theMainTabbedPageViewModel.LoginUser;
            americanQuestion.CreatorNickName = user.NickName;
            TriviaWebAPIProxy triviaWebAPIProxy = TriviaWebAPIProxy.CreateProxy();
            bool isAdded = await triviaWebAPIProxy.PostNewQuestion(americanQuestion);
            if (isAdded)
            {
                if (user.Questions == null)
                    theMainTabbedPageViewModel.LoginUserQuestions = new ObservableCollection<AmericanQuestion>();
                theMainTabbedPageViewModel.LoginUserQuestions.Add(americanQuestion);
                ((DeleteQuestionViewModel)((TheMainTabbedPage)Application.Current.MainPage).deleteQTab.BindingContext).UserQuestions = ((TheMainTabbedPageViewModel)((TheMainTabbedPage)Application.Current.MainPage).BindingContext).LoginUserQuestions;
                ((TheMainTabbedPage)Application.Current.MainPage).RemoveTab(((TheMainTabbedPage)Application.Current.MainPage).addQTab);
                await App.Current.MainPage.DisplayAlert("Trivia", "The question is added succesfuly", "Ok");
                EntryQText = "";
                EntryCorrectAns = "";
                EntryOtherAns1 = "";
                EntryOtherAns2 = "";
                EntryOtherAns3 = "";
            }
        }
    }
}
