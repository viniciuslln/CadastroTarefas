using CadastroTarefas.Models;
using CadastroTarefas.Resources;
using DataLayer;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CadastroTarefas.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;

        public LoginModel LoginModel { get; set; }

        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }

        public SignUpViewModel()
        {
            _userRepository = new UserRepository(App.Database);
            LoginModel = new LoginModel();
            LoginCommand = new AsyncCommand(DoSignUp);
            SignUpCommand = new Command(GoToSignUp);
        }

        private void GoToSignUp(object obj)
        {
            NavigateToSignUpPage();
        }

        private async Task DoSignUp()
        {
            if (!LoginModel.Validate())
            {
                return;
            }

            var registeredUser = await _userRepository.SearchRegisteredUser(LoginModel.Username);
            if (registeredUser != null)
            {
                LoginModel.ErrorMessage = Messages.UserAlreadyRegisteredMessage;
                return;
            }

            var userSaved = await _userRepository.SaveNewUser(LoginModel.Username, LoginModel.PlainPassword);

            if (userSaved.Id > 0)
            {
                App.LoggedUser = userSaved;
                NavigateToMainPage();
            }
        }


        private void NavigateToMainPage()
        {
            (Application.Current.MainWindow as NavigationWindow)?.NavigationService?.Navigate(new Uri("Pages/TasksPage.xaml", UriKind.Relative));
        }

        private void NavigateToSignUpPage()
        {
            (Application.Current.MainWindow as NavigationWindow)?.NavigationService?.GoBack();
        }
    }
}