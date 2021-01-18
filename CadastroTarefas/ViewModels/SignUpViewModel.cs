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
    public class SignUpViewModel : BaseViewModel
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

        public SignUpViewModel()
        {
            LoginCommand = new AsyncCommand(DoSignUp);
            SignUpCommand = new Command(GoToSignUp);
        }

        private void GoToSignUp(object obj)
        {
            NavigateToSignUpPage();
        }

        private async Task DoSignUp()
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

            var registeredUser = await Task.Run(() => App.Database.GetCollection<User>().FindOne(u => u.Username == Username));
            if (registeredUser != null)
            {
                ErrorMessage = Messages.UserAlreadyRegisteredMessage;
                return;
            }

            var encripitedPassword = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.Default.GetBytes(plainPassword));
            var user = new User { Username = Username, Password = Encoding.Default.GetString(encripitedPassword) };
            var userId = await Task.Run(() => App.Database.GetCollection<User>().Insert(user));
            if (userId > 0)
            {
                App.LoggedUser = user;
                NavigateToMainPage();
            }
        }

        private void NavigateToMainPage()
        {
            (Application.Current.MainWindow as NavigationWindow).NavigationService.Navigate(new Uri("Pages/TasksPage.xaml", UriKind.Relative));
        }

        private void NavigateToSignUpPage()
        {
            (Application.Current.MainWindow as NavigationWindow).NavigationService.GoBack();
        }
    }
}