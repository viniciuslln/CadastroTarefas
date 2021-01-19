using CadastroTarefas.Resources;
using MvvmHelpers;

namespace CadastroTarefas.Models
{
    public class LoginModel : ObservableObject
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
            get => new string('●', plainPassword.Length);
        }

        private string errorMessage;

        public string ErrorMessage
        {
            get => errorMessage;
            set => SetProperty(ref errorMessage, value);
        }
        public string PlainPassword { get => plainPassword; }

        public bool Validate()
        {
            ErrorMessage = "";

            if (string.IsNullOrEmpty(Username))
            {
                ErrorMessage = Messages.UsernameEmptyMessage;
                return false;
            }

            if (string.IsNullOrEmpty(plainPassword))
            {
                ErrorMessage = Messages.PasswordEmptyMessage;
                return false;
            }

            return true;
        }
    }
}
