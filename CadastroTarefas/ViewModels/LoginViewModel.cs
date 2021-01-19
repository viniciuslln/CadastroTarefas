using CadastroTarefas.Models;
using CadastroTarefas.Resources;
using CadastroTarefas.Services;
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
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;
        private readonly LoggedUserService _loggedUserService;

        public LoginModel LoginModel { get; set; }

        public ICommand LoginCommand { get; }

        public ICommand SignUpCommand { get; }

        public LoginViewModel(LoggedUserService loggedUserService, UserRepository userRepository)
        {
            _loggedUserService = loggedUserService;
            _userRepository = userRepository;
            LoginModel = new LoginModel();
            LoginCommand = new AsyncCommand(DoLogin);
            SignUpCommand = new Command(GoToSignUp);
        }

        private void GoToSignUp(object obj)
        {
            NavigateToSignUpPage();
        }

        public async Task DoLogin()
        {
            if (!LoginModel.Validate())
            {
                return;
            }

            var user = await _userRepository.GetRegisteredUser(LoginModel.Username, LoginModel.PlainPassword);
            if (user != null)
            {
                _loggedUserService.LoggedUser = user;
            }
            else
            {
                LoginModel.ErrorMessage = Messages.UserPasswordWrongMessage;
            }
        }

        private void NavigateToSignUpPage()
        {
            (Application.Current.MainWindow as NavigationWindow)?.NavigationService?.Navigate(new Uri("Pages/SignUpPage.xaml", UriKind.Relative));
        }
    }
}