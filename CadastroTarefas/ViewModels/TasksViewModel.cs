using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CadastroTarefas.ViewModels
{
    public class TasksViewModel: BaseViewModel
    {
        public ObservableRangeCollection<object> TodoTasks { get; set; } = new ObservableRangeCollection<object>();

        public TasksViewModel()
        {
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
            TodoTasks.Add(new object());
        }
    }
}
