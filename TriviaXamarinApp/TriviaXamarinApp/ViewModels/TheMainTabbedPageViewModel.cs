using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using TriviaXamarinApp.Models;
using TriviaXamarinApp.Views;
using Xamarin.Forms;

namespace TriviaXamarinApp.ViewModels
{
    class TheMainTabbedPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private User loginUser = null;
        public User LoginUser
        {
            get { return loginUser; }
            set {
                    loginUser = value;
                    TheMainTabbedPage theMainTabbedPage = (TheMainTabbedPage)Application.Current.MainPage;
                    if (loginUser == null) //Logout
                    {
                        theMainTabbedPage.AddTab(theMainTabbedPage.logIn);
                        theMainTabbedPage.AddTab(theMainTabbedPage.register);
                        theMainTabbedPage.RemoveTab(theMainTabbedPage.addQTab);
                        theMainTabbedPage.RemoveTab(theMainTabbedPage.deleteQTab);
                        this.LoginUserQuestions = new ObservableCollection<AmericanQuestion>();
                        ((HomePageViewModel)(theMainTabbedPage.home.BindingContext)).ShowLogout = false;
                }
                    else // Login
                    {
                        theMainTabbedPage.RemoveTab(theMainTabbedPage.logIn);
                        theMainTabbedPage.RemoveTab(theMainTabbedPage.register);
                        theMainTabbedPage.AddTab(theMainTabbedPage.deleteQTab); 
                        if (loginUser.Questions == null)
                            this.LoginUserQuestions = new ObservableCollection<AmericanQuestion>();
                        else
                            this.LoginUserQuestions = new ObservableCollection<AmericanQuestion>(loginUser.Questions);
                        ((DeleteQuestionViewModel)(theMainTabbedPage.deleteQTab.BindingContext)).UserQuestions = ((TheMainTabbedPageViewModel)(theMainTabbedPage).BindingContext).LoginUserQuestions;
                        ((HomePageViewModel)(theMainTabbedPage.home.BindingContext)).ShowLogout = true;
                    }
                }
        }

        public ObservableCollection<AmericanQuestion> LoginUserQuestions { get; set; }
    }
}
