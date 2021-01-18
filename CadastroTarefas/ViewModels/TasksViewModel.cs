using CadastroTarefas.Models;
using CadastroTarefas.Resources;
using DataLayer;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CadastroTarefas.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        private UserTask editingTask = null;

        private string taskEditionTitle = PagesText.NewTasks;

        public string TaskEditionTitle
        {
            get => taskEditionTitle;
            set => SetProperty(ref taskEditionTitle, value);
        }

        private bool isEditMode = false;

        public bool IsEditMode
        {
            get => isEditMode;
            set => SetProperty(ref isEditMode, value);
        }

        private string newTaskDescription;

        public string NewTaskDescription
        {
            get => newTaskDescription;
            set => SetProperty(ref newTaskDescription, value);
        }

        public ObservableRangeCollection<UserTaskModel> TodoTasks { get; set; } = new ObservableRangeCollection<UserTaskModel>();
        public ObservableRangeCollection<UserTaskModel> CompletedTasks { get; set; } = new ObservableRangeCollection<UserTaskModel>();

        public ICommand SaveTaskCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CompleteCommand { get; set; }
        public ICommand RemoveCommand { get; set; }
        public ICommand CancelEditCommand { get; set; }

        public TasksViewModel()
        {
            LoadTasks().SafeFireAndForget();
            SaveTaskCommand = new AsyncCommand(SaveTask);
            EditCommand = new AsyncCommand<UserTaskModel>(EditTask);
            CompleteCommand = new AsyncCommand<UserTaskModel>(CompleteTask);
            RemoveCommand = new AsyncCommand<UserTaskModel>(RemoveTask);
            CancelEditCommand = new Command(CancelEdit);
        }

        private void CancelEdit()
        {
            var taskOnList = TodoTasks.FirstOrDefault(ut => ut.UserTask.Id == editingTask.Id);
            if (taskOnList != null)
            {
                taskOnList.IsEditMode = false;
            }
            IsEditMode = false;
            editingTask = null;
            NewTaskDescription = "";
        }

        private async Task RemoveTask(UserTaskModel arg)
        {
            using (var db = new CadastroTarefasContext())
            {
                db.UserTasks.Remove(arg.UserTask);
                await db.SaveChangesAsync();
                await LoadTasks();
            }
        }

        private async Task CompleteTask(UserTaskModel arg)
        {
            using (var db = new CadastroTarefasContext())
            {
                arg.UserTask.TaskStatus = DataLayer.TaskStatus.COMPLETED;
                db.UserTasks.Update(arg.UserTask);
                await db.SaveChangesAsync();
                await LoadTasks();
            }
        }

        private async Task EditTask(UserTaskModel arg)
        {
            IsEditMode = arg.IsEditMode = true;
            editingTask = arg.UserTask;
            NewTaskDescription = arg.UserTask.Description;
        }

        private async Task LoadTasks()
        {
            using (var db = new CadastroTarefasContext())
            {
                TodoTasks.Clear();
                CompletedTasks.Clear();

                db.UserTasks.Where(ut => ut.UserId == App.LoggedUser.Id && ut.TaskStatus == DataLayer.TaskStatus.TODO)
                    .OrderByDescending(u => u.Id).ToList()
                    .ForEach(t => TodoTasks.Add(new UserTaskModel { UserTask = t }));
                db.UserTasks.Where(ut => ut.UserId == App.LoggedUser.Id && ut.TaskStatus == DataLayer.TaskStatus.COMPLETED)
                    .OrderByDescending(u => u.Id).ToList()
                    .ForEach(t => CompletedTasks.Add(new UserTaskModel { UserTask = t }));
            }
        }

        private async Task SaveTask()
        {
            using (var db = new CadastroTarefasContext())
            {
                if (!string.IsNullOrEmpty(NewTaskDescription))
                {
                    if (IsEditMode && editingTask != null)
                    {
                        editingTask.Description = NewTaskDescription;
                        db.UserTasks.Update(editingTask);
                    }
                    else
                    {
                        var task = new UserTask { Description = NewTaskDescription, UserId = App.LoggedUser.Id, TaskStatus = DataLayer.TaskStatus.TODO };
                        await db.UserTasks.AddAsync(task);
                    }

                    await db.SaveChangesAsync();
                    NewTaskDescription = "";
                    await LoadTasks();
                }
            }
        }
    }
}