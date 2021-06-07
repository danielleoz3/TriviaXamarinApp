using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TriviaXamarinApp.Models;
using TriviaXamarinApp.Services;
using Xamarin.Forms;
using TriviaXamarinApp.Views;
using System.Threading.Tasks;

namespace TriviaXamarinApp.ViewModels
{
    class HomePageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<string> CurrentAnswers { get; set; }

        private AmericanQuestion currentQuestion;
        public AmericanQuestion CurrentAmericanQuestion 
        {
            get => this.currentQuestion; 
            set
            {
                if (value != this.currentQuestion)
                {
                    this.currentQuestion = value;
                    OnPropertyChanged("CurrentAmericanQuestion");
                }
            }
        }

        private string selectedAnswer;
        public string SelectedAnswer
        {
            get => this.selectedAnswer;
            set
            {
                if (value != this.selectedAnswer)
                {
                    this.selectedAnswer = value;
                    OnPropertyChanged("SelectedAnswer");
                }
            }
        }

        private int counterCorrectAnswers;
        public int CounterCorrectAnswers
        {
            get => this.counterCorrectAnswers;
            set
            {
                if (value != this.counterCorrectAnswers)
                {
                    this.counterCorrectAnswers = value;
                    OnPropertyChanged("CounterCorrectAnswers");
                }
            }
        }

        private bool showLogout;
        public bool ShowLogout
        {
            get => this.showLogout;
            set
            {
                if (value != this.showLogout)
                {
                    this.showLogout = value;
                    OnPropertyChanged("ShowLogout");
                }
            }
        }

        public HomePageViewModel()
        {
            CurrentAnswers = new ObservableCollection<string>();
            CounterCorrectAnswers = 0;
            Task task = GetRandomQuestion(null);
        }

        private async Task GetRandomQuestion(AmericanQuestion prevAmericanQuestion)
        {
            AmericanQuestion newQuestion = null;
            TriviaWebAPIProxy triviaWebAPIProxy = TriviaWebAPIProxy.CreateProxy();

            while ((newQuestion == null) ||
                   (newQuestion.QText == null) || (newQuestion.QText == "") ||
                   (newQuestion.CorrectAnswer == null) || (newQuestion.CorrectAnswer == "") ||
                   (newQuestion.OtherAnswers == null) ||
                   (newQuestion.OtherAnswers.Length < 3) ||
                   (newQuestion.OtherAnswers[0] == null) || (newQuestion.OtherAnswers[0] == "") ||
                   (newQuestion.OtherAnswers[1] == null) || (newQuestion.OtherAnswers[1] == "") ||
                   (newQuestion.OtherAnswers[2] == null) || (newQuestion.OtherAnswers[2] == "") ||
                   ((prevAmericanQuestion != null) && (newQuestion.QText == prevAmericanQuestion.QText)))
            {
                newQuestion = await triviaWebAPIProxy.GetRandomQuestion();
            }

            CurrentAnswers.Clear();
            
            CurrentAmericanQuestion = newQuestion;
            Random r = new Random();
            int indexCorrectAnswer = r.Next(0, 4);
            int otherAnswersIndex = 0;

            for (int i = 0; i < 4; i++)
            {
                if (i == indexCorrectAnswer)
                    CurrentAnswers.Add(CurrentAmericanQuestion.CorrectAnswer);
                else
                {
                    if (otherAnswersIndex < 3)
                    {
                        CurrentAnswers.Add(CurrentAmericanQuestion.OtherAnswers[otherAnswersIndex]);
                        otherAnswersIndex++;
                    }
                }
            }
            SelectedAnswer = null;
        }

        public ICommand SelectionQuestionCommand => new Command(SelectionQuestion);
        private async void SelectionQuestion()
        {
            if (CurrentAmericanQuestion != null && SelectedAnswer == CurrentAmericanQuestion.CorrectAnswer)
            {
                CounterCorrectAnswers++;
                TheMainTabbedPage theMainTabbedPage = (TheMainTabbedPage)Application.Current.MainPage;
                if ((CounterCorrectAnswers > 0) && (CounterCorrectAnswers % 3 == 0))
                {
                    if (((TheMainTabbedPageViewModel)theMainTabbedPage.BindingContext).LoginUser != null)
                    {
                        theMainTabbedPage.AddTab((theMainTabbedPage).addQTab);
                        bool moveToAddPage = await App.Current.MainPage.DisplayAlert("Trivia", "Well done! Now you can add new question \nDo you want to move to the add question page?", "Yes", "No");
                        if (moveToAddPage)
                        {
                            theMainTabbedPage.CurrentTab(theMainTabbedPage.addQTab);
                            await GetRandomQuestion(CurrentAmericanQuestion);
                            return;
                        }
                    }
                    else
                    {
                        bool moveToLogIn = await App.Current.MainPage.DisplayAlert("Trivia", "In order to add a new question, you need to login \nDo you want to move to the login page?", "Yes", "No");
                        if (moveToLogIn)
                        {
                            theMainTabbedPage.CurrentTab(theMainTabbedPage.logIn);
                            await GetRandomQuestion(CurrentAmericanQuestion);
                            return;
                        }
                    }
                }
                else
                {
                    theMainTabbedPage.RemoveTab((theMainTabbedPage).addQTab);
                }
            }
            if (SelectedAnswer != null)
                await GetRandomQuestion(CurrentAmericanQuestion);
        }

        public ICommand LogOutCommand => new Command(LogOut);
        private void LogOut()
        {
            TheMainTabbedPage theMainTabbedPage = (TheMainTabbedPage)Application.Current.MainPage;
            ((TheMainTabbedPageViewModel)theMainTabbedPage.BindingContext).LoginUser = null;
        }
    }
}
