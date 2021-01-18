using CadastroTarefas.Resources;
using DataLayer;
using Microsoft.EntityFrameworkCore;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CadastroTarefas.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private string username;
        public string Username
        {
            get { return username; }
            set { SetProperty(ref username, value); }
        }
       
        private string password;
        public string Password
        {
            get { return password; }
            set { SetProperty(ref password, value); }
        }

        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set { SetProperty(ref errorMessage, value); }
        }

        public ICommand LoginCommand { get; private set; }
        
        public ICommand SignUpCommand { get; private set; }

        public LoginViewModel()
        {
            LoginCommand = new AsyncCommand(DoLogin);
            SignUpCommand = new Command(GoToSignUp);
        }

        private void GoToSignUp(object obj)
        {
            NavigateToSignUpPage();
        }

        private async Task DoLogin()
        {
            IsBusy = true;
            ErrorMessage = "";

            if (string.IsNullOrEmpty(Username))
            {
                ErrorMessage = Messages.UsernameEmptyMessage;
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = Messages.PasswordEmptyMessage;
                return;
            }

            using (var db = new CadastroTarefasContext())
            {
                var user = await db.Users.FirstOrDefaultAsync(u => u.Username == Username && u.Password == Password);
                if (user != null)
                {
                    NavigateToMainPage();
                }
                else
                {
                    ErrorMessage = Messages.UserPasswordWrongMessage;
                }
            }
        }

        private void NavigateToMainPage()
        {
            (Application.Current.MainWindow as NavigationWindow).NavigationService.Navigate(new Uri("Pages/TasksPage.xaml", UriKind.Relative));
        }

        private void NavigateToSignUpPage()
        {
            (Application.Current.MainWindow as NavigationWindow).NavigationService.Navigate(new Uri("Pages/SignUpPage.xaml", UriKind.Relative));
        }
    }
}
