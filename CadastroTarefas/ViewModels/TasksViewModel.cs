using DataLayer;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CadastroTarefas.ViewModels
{
    public class TasksViewModel: BaseViewModel
    {
        private string newTaskDescription;

        public string NewTaskDescription
        {
            get { return newTaskDescription; }
            set { SetProperty(ref newTaskDescription, value); }
        }

        public ObservableRangeCollection<object> TodoTasks { get; set; } = new ObservableRangeCollection<object>();

        public ICommand SaveTaskCommand { get; set; }
        public TasksViewModel()
        {
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());

            SaveTaskCommand = new AsyncCommand(SaveTask);
        }

        private async Task SaveTask()
        {
            using (var db = new CadastroTarefasContext())
            {
                var task = new UserTask { Description = NewTaskDescription, UserId = 1, TaskStatus = DataLayer.TaskStatus.TODO };
                await db.UserTasks.AddAsync(task);
                await db.SaveChangesAsync();
            }
        }
    }
}
