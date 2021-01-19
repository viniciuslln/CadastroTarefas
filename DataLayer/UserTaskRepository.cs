using DataLayer;
using LiteDB;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CadastroTarefas
{
    public class UserTaskRepository
    {
        private readonly LiteDatabase _db;

        public UserTaskRepository(LiteDatabase db)
        {
            _db = db;
        }

        public Task RemoveTask(int id)
        {
            return Task.Run(() => _db.GetCollection<UserTask>().Delete(id));
        }

        public Task<List<UserTask>> GetTodoTasks(int userId)
        {
            return Task.Run(() =>
                     _db.GetCollection<UserTask>()
                        .Find(ut => ut.UserId == userId && ut.TaskStatus == DataLayer.TaskStatus.TODO)
                        .OrderByDescending(u => u.Id)
                        .ToList());
        }

        public Task<List<UserTask>> GetCompletedTasks(int userId)
        {
            return Task.Run(() =>
                     _db.GetCollection<UserTask>()
                        .Find(ut => ut.UserId == userId && ut.TaskStatus == DataLayer.TaskStatus.COMPLETED)
                        .OrderByDescending(u => u.Id)
                        .ToList());
        }

        public async Task SaveTaskAsCompleted(UserTask taskToEdit)
        {
            taskToEdit.TaskStatus = DataLayer.TaskStatus.COMPLETED;
            await Task.Run(() => _db.GetCollection<UserTask>().Update(taskToEdit));
        }

        public async Task SaveEditTask(UserTask taskToEdit, string description)
        {
            if (taskToEdit != null)
            {
                taskToEdit.Description = description;
                await Task.Run(() => _db.GetCollection<UserTask>().Update(taskToEdit));
            }
        }

        public async Task<UserTask> SaveNewTask(string description, int userId)
        {
            var task = new UserTask { Description = description, UserId = userId, TaskStatus = DataLayer.TaskStatus.TODO };
            await Task.Run(() => _db.GetCollection<UserTask>().Insert(task));
            return task;
        }

    }
}
