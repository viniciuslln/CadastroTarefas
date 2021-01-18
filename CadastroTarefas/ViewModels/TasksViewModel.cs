﻿using DataLayer;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CadastroTarefas.ViewModels
{
    public class TasksViewModel : BaseViewModel
    {
        private string newTaskDescription;

        public string NewTaskDescription
        {
            get { return newTaskDescription; }
            set { SetProperty(ref newTaskDescription, value); }
        }

        public ObservableRangeCollection<UserTask> TodoTasks { get; set; } = new ObservableRangeCollection<UserTask>();
        public ObservableRangeCollection<UserTask> CompletedTasks { get; set; } = new ObservableRangeCollection<UserTask>();

        public ICommand SaveTaskCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand CompleteCommand { get; set; }
        public ICommand RemoveCommand { get; set; }

        public TasksViewModel()
        {
            LoadTasks().SafeFireAndForget();
            SaveTaskCommand = new AsyncCommand(SaveTask);
            EditCommand = new AsyncCommand<UserTask>(EditTask);
            CompleteCommand = new AsyncCommand<UserTask>(CompleteTask);
            RemoveCommand = new AsyncCommand<UserTask>(RemoveTask);
        }

        private async Task RemoveTask(object arg)
        {
            using (var db = new CadastroTarefasContext())
            {
                if (arg is UserTask task)
                {
                    db.UserTasks.Remove(task);
                    await db.SaveChangesAsync();
                    await LoadTasks();
                }
            }
        }

        private async Task CompleteTask(object arg)
        {
            using (var db = new CadastroTarefasContext())
            {
                if (arg is UserTask task)
                {
                    task.TaskStatus = DataLayer.TaskStatus.COMPLETED;
                    db.UserTasks.Update(task);
                    await db.SaveChangesAsync();
                    await LoadTasks();
                }
            }
        }

        private async Task EditTask(UserTask arg)
        {

        }

        private async Task LoadTasks()
        {
            using (var db = new CadastroTarefasContext())
            {
                TodoTasks.Clear();
                CompletedTasks.Clear();

                db.UserTasks.Where(ut => ut.UserId == App.LoggedUser.Id && ut.TaskStatus == DataLayer.TaskStatus.TODO).ToList()
                    .ForEach(t => TodoTasks.Add(t));
                db.UserTasks.Where(ut => ut.UserId == App.LoggedUser.Id && ut.TaskStatus == DataLayer.TaskStatus.COMPLETED).ToList()
                    .ForEach(t => CompletedTasks.Add(t));
            }
        }

        private async Task SaveTask()
        {
            using (var db = new CadastroTarefasContext())
            {
                if (!string.IsNullOrEmpty(NewTaskDescription))
                {
                    var task = new UserTask { Description = NewTaskDescription, UserId = App.LoggedUser.Id, TaskStatus = DataLayer.TaskStatus.TODO };
                    await db.UserTasks.AddAsync(task);
                    await db.SaveChangesAsync();
                    NewTaskDescription = "";
                    await LoadTasks();
                }
            }
        }
    }
}
