using DataLayer;
using System;

namespace CadastroTarefas.Services
{
    public class LoggedUserService
    {
        private User _loggedUser;
        public User LoggedUser
        {
            get => _loggedUser; set
            {
                _loggedUser = value;
                if (value == null)
                {
                    UserSignout.Invoke();
                }
                else
                {
                    UserLogged?.Invoke();
                }
            }
        }

        public Action UserLogged { get; set; }
        public Action UserSignout { get; set; }

        public LoggedUserService() { }

    }
}
