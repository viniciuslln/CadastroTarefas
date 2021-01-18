using CadastroTarefas.Models;
using CadastroTarefas.Resources;
using DataLayer;
using LiteDB;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Collections.Generic;
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
            CompleteCommand = new AsyncCommand<UserTaskModel>(CompleteTask);
            RemoveCommand = new AsyncCommand<UserTaskModel>(RemoveTask);
            EditCommand = new Command<UserTaskModel>(EditTask);
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
            await Task.Run(() => App.Database.GetCollection<UserTask>().Delete(arg.UserTask.Id));
            await LoadTasks();
        }

        private async Task CompleteTask(UserTaskModel arg)
        {
            arg.UserTask.TaskStatus = DataLayer.TaskStatus.COMPLETED;
            await Task.Run(() => App.Database.GetCollection<UserTask>().Update(arg.UserTask));

            await LoadTasks();
        }

        private void EditTask(UserTaskModel arg)
        {
            IsEditMode = arg.IsEditMode = true;
            editingTask = arg.UserTask;
            NewTaskDescription = arg.UserTask.Description;
        }

        private async Task LoadTasks()
        {
            TodoTasks.Clear();
            CompletedTasks.Clear();

            var todoTasks = await GetTodoTasks(App.Database, App.LoggedUser.Id);
            var completedTasks = await GetCompletedTasks(App.Database, App.LoggedUser.Id);

            todoTasks.ForEach(t => TodoTasks.Add(new UserTaskModel { UserTask = t }));
            completedTasks.ForEach(t => CompletedTasks.Add(new UserTaskModel { UserTask = t }));
        }

        private Task<List<UserTask>> GetTodoTasks(LiteDatabase db, int userId)
        {
            return Task.Run(() =>
                     db.GetCollection<UserTask>()
                        .Find(ut => ut.UserId == userId && ut.TaskStatus == DataLayer.TaskStatus.TODO)
                        .OrderByDescending(u => u.Id)
                        .ToList());
        }

        private Task<List<UserTask>> GetCompletedTasks(LiteDatabase db, int userId)
        {
            return Task.Run(() =>
                     db.GetCollection<UserTask>()
                        .Find(ut => ut.UserId == userId && ut.TaskStatus == DataLayer.TaskStatus.COMPLETED)
                        .OrderByDescending(u => u.Id)
                        .ToList());
        }

        private async Task SaveTask()
        {
            if (!string.IsNullOrEmpty(NewTaskDescription))
            {
                if (IsEditMode && editingTask != null)
                {
                    editingTask.Description = NewTaskDescription;
                    await Task.Run(() => App.Database.GetCollection<UserTask>().Update(editingTask));
                    CancelEdit();
                }
                else
                {
                    var task = new UserTask { Description = NewTaskDescription, UserId = App.LoggedUser.Id, TaskStatus = DataLayer.TaskStatus.TODO };
                    await Task.Run(() => App.Database.GetCollection<UserTask>().Insert(task));
                }

                NewTaskDescription = "";
                await LoadTasks();
            }
        }
    }
}