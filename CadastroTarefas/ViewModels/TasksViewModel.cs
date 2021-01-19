using CadastroTarefas.Models;
using CadastroTarefas.Resources;
using CadastroTarefas.Services;
using DataLayer;
using LiteDB;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CadastroTarefas.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        private readonly LoggedUserService _loggedUserService;
        private readonly UserTaskRepository _userTaskRepository;
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
        public ICommand RemoveTodoCommand { get; set; }
        public ICommand RemoveCompletedCommand { get; set; }
        public ICommand CancelEditCommand { get; set; }

        public TasksViewModel(LoggedUserService loggedUserService, UserTaskRepository userTaskRepository)
        {
            _loggedUserService = loggedUserService;
            _userTaskRepository = userTaskRepository;

            SaveTaskCommand = new AsyncCommand(SaveTask);
            CompleteCommand = new AsyncCommand<UserTaskModel>(CompleteTask);
            RemoveTodoCommand = new AsyncCommand<UserTaskModel>(RemoveTodoTask);
            RemoveCompletedCommand = new AsyncCommand<UserTaskModel>(RemoveCompletedTask);
            EditCommand = new Command<UserTaskModel>(EnterEditMode);
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

        public async Task RemoveTodoTask(UserTaskModel arg)
        {
            await Task.Run(() => App.Database.GetCollection<UserTask>().Delete(arg.UserTask.Id));
            await LoadTodoTasks();
        }

        public async Task RemoveCompletedTask(UserTaskModel arg)
        {
            await _userTaskRepository.RemoveTask(arg.UserTask.Id);
            await LoadCompletedTasks();
        }

        private async Task CompleteTask(UserTaskModel arg)
        {
            await _userTaskRepository.SaveTaskAsCompleted(arg.UserTask);
            await LoadTasks();
        }

        public void EnterEditMode(UserTaskModel arg)
        {
            IsEditMode = arg.IsEditMode = true;
            editingTask = arg.UserTask;
            NewTaskDescription = arg.UserTask.Description;
        }

        public async Task LoadTasks()
        {
            await LoadTodoTasks();
            await LoadCompletedTasks();
        }

        private async Task LoadTodoTasks()
        {
            TodoTasks.Clear();
            var todoTasks = await _userTaskRepository.GetTodoTasks(_loggedUserService.LoggedUser.Id);
            todoTasks.ForEach(t => TodoTasks.Add(new UserTaskModel { UserTask = t }));
        }

        private async Task LoadCompletedTasks()
        {
            CompletedTasks.Clear();
            var completedTasks = await _userTaskRepository.GetCompletedTasks(_loggedUserService.LoggedUser.Id);
            completedTasks.ForEach(t => CompletedTasks.Add(new UserTaskModel { UserTask = t }));
        }

        public async Task SaveTask()
        {
            if (!string.IsNullOrEmpty(NewTaskDescription))
            {
                if (IsEditMode)
                {
                    await _userTaskRepository.SaveEditTask(editingTask, NewTaskDescription);
                    CancelEdit();
                }
                else
                {
                    await _userTaskRepository.SaveNewTask(NewTaskDescription, _loggedUserService.LoggedUser.Id);
                }

                NewTaskDescription = "";
                await LoadTodoTasks();
            }
        }

    }
}