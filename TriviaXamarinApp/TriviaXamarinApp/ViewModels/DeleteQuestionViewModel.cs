using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Input;
using TriviaXamarinApp.Models;
using TriviaXamarinApp.Views;
using Xamarin.Forms;
using TriviaXamarinApp.Services;

namespace TriviaXamarinApp.ViewModels
{
    class DeleteQuestionViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<AmericanQuestion> userQuestions;
        public ObservableCollection<AmericanQuestion> UserQuestions
        {
            get => this.userQuestions;
            set
            {
                if (value != this.userQuestions)
                {
                    this.userQuestions = value;
                    OnPropertyChanged("UserQuestions");
                }
            }
        }

        public DeleteQuestionViewModel()
        {
            UserQuestions = new ObservableCollection<AmericanQuestion>();
        }

        public ICommand DeleteQuestionCommand => new Command<AmericanQuestion>(DeleteQuestion);
        public async void DeleteQuestion(AmericanQuestion americanQuestion)
        {
            if (UserQuestions.Contains(americanQuestion))
            {
                TriviaWebAPIProxy triviaWebAPIProxy = TriviaWebAPIProxy.CreateProxy();
                bool isRemoved = await triviaWebAPIProxy.DeleteQuestion(americanQuestion);
                if (isRemoved)
                {
                    UserQuestions.Remove(americanQuestion);
                    ((TheMainTabbedPageViewModel)((TheMainTabbedPage)Application.Current.MainPage).BindingContext).LoginUserQuestions.Remove(americanQuestion);
                }
            }
        }
    }
}
