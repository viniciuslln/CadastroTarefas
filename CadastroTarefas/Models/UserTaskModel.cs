using DataLayer;
using MvvmHelpers;

namespace CadastroTarefas.Models
{
    public class UserTaskModel : ObservableObject
    {
        private UserTask userTask;

        public UserTask UserTask
        {
            get { return userTask; }
            set { SetProperty(ref userTask, value); }
        }

        private bool isEditMode = false;

        public bool IsEditMode
        {
            get { return isEditMode; }
            set { SetProperty(ref isEditMode, value); }
        }
    }
}