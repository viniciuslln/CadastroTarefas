using CadastroTarefas;
using CadastroTarefas.Services;
using CadastroTarefas.ViewModels;
using DataLayer;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CadastroTarefas.Test.ViewModelTests
{
    [TestClass]
    public class TasksViewModelTest
    {
        private readonly LiteDatabase _db;
        private readonly MemoryStream _memoryStream;
        private readonly LoggedUserService _loggedUserService;
        private readonly UserTaskRepository _userTaskRepo;

        public TasksViewModelTest()
        {
            _memoryStream = new MemoryStream();
            _db = new LiteDatabase(_memoryStream);
            var _loggedUserServiceMoq = new Moq.Mock<LoggedUserService>();
            _loggedUserService = _loggedUserServiceMoq.Object;
            _loggedUserService.LoggedUser = new User
            {
                Id = 1,
                Username = "TestUser",
                Password = "TestPass"
            };
            _userTaskRepo = new UserTaskRepository(_db);
        }


        [TestCleanup]
        public void TestCleanup()
        {
            _memoryStream.Dispose();
        }

        [TestMethod]
        public async Task DeveObterTarefasAFazer()
        {
            var testCases = new List<string>()
            {
                "Todo1",
                "Todo2",
                "Todo3",
            };

            var savedTasks = testCases.Select(ut => _userTaskRepo.SaveNewTask(ut, _loggedUserService.LoggedUser.Id).Result).ToList();

            var vm = new TasksViewModel(_loggedUserService, _userTaskRepo);
            await vm.LoadTasks();

            List<UserTask> expected = vm.TodoTasks.Select(m => m.UserTask).ToList();
            Assert.AreEqual(expected.Count, savedTasks.Count);
            savedTasks.ForEach(item => Assert.IsTrue(expected.Any((a) => a.Id == item.Id)));
        }

        [TestMethod]
        public async Task DeveObterTarefas()
        {
            var testCases = new List<string>()
            {
                "Todo1",
                "Todo2",
                "Completed",
            };

            var savedTasks = testCases.Select(ut => _userTaskRepo.SaveNewTask(ut, _loggedUserService.LoggedUser.Id).Result).ToList();
            var completedItem = savedTasks[2];
            await _userTaskRepo.SaveTaskAsCompleted(completedItem);
            savedTasks.RemoveAt(2);
            var vm = new TasksViewModel(_loggedUserService, _userTaskRepo);
            await vm.LoadTasks();

            Assert.AreEqual(vm.TodoTasks.Count, savedTasks.Count);
            Assert.AreEqual(vm.CompletedTasks.Count, 1);

            List<UserTask> expectedTodo = vm.TodoTasks.Select(m => m.UserTask).ToList();
            savedTasks.ForEach(item => Assert.IsTrue(expectedTodo.Any((a) => a.Id == item.Id)));

            List<UserTask> expectedCompleted = vm.CompletedTasks.Select(m => m.UserTask).ToList();
            Assert.IsTrue(expectedCompleted.Any((a) => a.Id == completedItem.Id));
        }

        [TestMethod]
        public async Task DeveSalvarTarefa()
        {
            const string descriptionTest = "DescriptionTest";

            var vm = new TasksViewModel(_loggedUserService, _userTaskRepo);
            vm.NewTaskDescription = descriptionTest;
            await vm.SaveTask();

            Assert.AreEqual(vm.TodoTasks.Count, 1);
            Assert.AreEqual(vm.TodoTasks.ElementAt(0).UserTask.Description, descriptionTest);
            Assert.AreEqual(vm.NewTaskDescription, "");
        }

        [TestMethod]
        public async Task NaoDeveSalvarTarefaDescricaoVazia()
        {
            const string descriptionTest = "";

            var vm = new TasksViewModel(_loggedUserService, _userTaskRepo);
            vm.NewTaskDescription = descriptionTest;
            await vm.SaveTask();

            Assert.AreEqual(vm.TodoTasks.Count, 0);
            Assert.AreEqual(vm.NewTaskDescription, "");
        }

        [TestMethod]
        public async Task DeveEntrarNoModoEdicao()
        {
            await _userTaskRepo.SaveNewTask("description", _loggedUserService.LoggedUser.Id);
            var vm = new TasksViewModel(_loggedUserService, _userTaskRepo);
            await vm.LoadTasks();
            var task = vm.TodoTasks.ElementAt(0);

            vm.EnterEditMode(task);

            Assert.AreEqual(task.UserTask.Description, vm.NewTaskDescription);
            Assert.AreEqual(true, vm.IsEditMode);
            Assert.AreEqual(true, task.IsEditMode);
        }

        [TestMethod]
        public async Task DeveSalvarTaskEditada()
        {
            const string newDescription = "NewDescription";

            await _userTaskRepo.SaveNewTask("description", _loggedUserService.LoggedUser.Id);
            var vm = new TasksViewModel(_loggedUserService, _userTaskRepo);
            await vm.LoadTasks();
            var task = vm.TodoTasks.ElementAt(0);

            vm.EnterEditMode(task);
            vm.NewTaskDescription = newDescription;
            await vm.SaveTask();

            Assert.AreEqual(vm.TodoTasks.ElementAt(0).UserTask.Description, newDescription);
            Assert.AreEqual(false, vm.IsEditMode);
            Assert.AreEqual(false, task.IsEditMode);
        }

        public async Task DeveRemoverTarefasAFazer()
        {
            await _userTaskRepo.SaveNewTask("description", _loggedUserService.LoggedUser.Id);
            await _userTaskRepo.SaveNewTask("description2", _loggedUserService.LoggedUser.Id);
            var vm = new TasksViewModel(_loggedUserService, _userTaskRepo);
            await vm.LoadTasks();

            var arg = vm.TodoTasks.ElementAt(0);
            await vm.RemoveTodoTask(arg);

            var savedTasks = await _userTaskRepo.GetTodoTasks(_loggedUserService.LoggedUser.Id);
            Assert.AreEqual(1, savedTasks.Count);
            Assert.AreEqual(arg.UserTask.Description, savedTasks.ElementAt(0).Description);
        }

        public async Task DeveRemoverTarefasConcluidas()
        {
            await _userTaskRepo.SaveNewTask("description", _loggedUserService.LoggedUser.Id);
            var toCOmplete = await _userTaskRepo.SaveNewTask("toCOmplete", _loggedUserService.LoggedUser.Id);
            await _userTaskRepo.SaveTaskAsCompleted(toCOmplete);
            var vm = new TasksViewModel(_loggedUserService, _userTaskRepo);
            await vm.LoadTasks();

            var arg = vm.TodoTasks.First( t => t.UserTask.Id == toCOmplete.Id);
            await vm.RemoveCompletedTask(arg);

            var savedTasks = await _userTaskRepo.GetCompletedTasks(_loggedUserService.LoggedUser.Id);
            Assert.AreEqual(0, savedTasks.Count);
            Assert.AreEqual(arg.UserTask.Description, savedTasks.ElementAt(0).Description);
        }

    }
}

