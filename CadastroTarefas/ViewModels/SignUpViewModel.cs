using CadastroTarefas.Models;
using CadastroTarefas.Resources;
using CadastroTarefas.Services;
using DataLayer;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CadastroTarefas.ViewModels
{
    public class SignUpViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;
        private readonly LoggedUserService _loggedUserService;
        public LoginModel LoginModel { get; set; }

        public ICommand LoginCommand { get; }
        public ICommand SignUpCommand { get; }

        public SignUpViewModel(LoggedUserService loggedUserService, UserRepository userRepository)
        {
            _loggedUserService = loggedUserService;
            _userRepository = userRepository;
            LoginModel = new LoginModel();
            LoginCommand = new AsyncCommand(DoSignUp);
            SignUpCommand = new Command(GoToSignUp);
        }

        private void GoToSignUp(object obj)
        {
            NavigateToSignUpPage();
        }

        public async Task DoSignUp()
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
                _loggedUserService.LoggedUser = userSaved;
            }
        }

        private void NavigateToSignUpPage()
        {
            (Application.Current.MainWindow as NavigationWindow)?.NavigationService?.GoBack();
        }
    }
}