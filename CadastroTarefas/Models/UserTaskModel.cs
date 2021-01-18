using DataLayer;
using MvvmHelpers;

namespace CadastroTarefas.Models
{
    public class UserTaskModel : ObservableObject
    {
        private UserTask userTask;

        public UserTask UserTask
        {
            get => userTask;
            set => SetProperty(ref userTask, value);
        }

        private bool isEditMode = false;

        public bool IsEditMode
        {
            get => isEditMode;
            set => SetProperty(ref isEditMode, value);
        }
    }
}