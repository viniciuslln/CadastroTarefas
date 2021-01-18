using CadastroTarefas.Resources;
using DataLayer;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Text;
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
            get => username;
            set => SetProperty(ref username, value);
        }

        private string plainPassword = string.Empty;

        public string Password
        {
            set
            {
                plainPassword = value;
                OnPropertyChanged("Password");
            }
            get => new String('●', plainPassword.Length);
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
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

            if (string.IsNullOrEmpty(plainPassword))
            {
                ErrorMessage = Messages.PasswordEmptyMessage;
                return;
            }

            var encripitedPassword = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.Default.GetBytes(plainPassword));
            var user = await Task.Run(() => App.Database.GetCollection<User>().FindOne(u => u.Username == Username && u.Password == Encoding.Default.GetString(encripitedPassword)));
            if (user != null)
            {
                App.LoggedUser = user;
                NavigateToMainPage();
            }
            else
            {
                ErrorMessage = Messages.UserPasswordWrongMessage;
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