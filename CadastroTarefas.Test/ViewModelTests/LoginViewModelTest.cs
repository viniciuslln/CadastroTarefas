using CadastroTarefas.Resources;
using CadastroTarefas.Services;
using CadastroTarefas.ViewModels;
using DataLayer;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace CadastroTarefas.Test.ViewModelTests
{

    [TestClass]
    public class LoginViewModelTest
    {
        private readonly LiteDatabase _db;
        private readonly MemoryStream _memoryStream;
        private readonly LoggedUserService _loggedUserService;
        private readonly UserRepository _userRepo;

        public LoginViewModelTest()
        {
            _memoryStream = new MemoryStream();
            _db = new LiteDatabase(_memoryStream);
            var _loggedUserServiceMoq = new Moq.Mock<LoggedUserService>();
            _loggedUserService = _loggedUserServiceMoq.Object;
            _userRepo = new DataLayer.UserRepository(_db);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _memoryStream.Dispose();
        }

        [TestMethod]
        public async Task DeveRealizarLogin()
        {
            var username = "TestUser";
            var pass = "TestPass";

            await _userRepo.SaveNewUser(username, pass);

            var vm = new LoginViewModel(_loggedUserService, _userRepo);
            vm.LoginModel.Username = username;
            vm.LoginModel.Password = pass;

            await vm.DoLogin();

            Assert.IsNotNull(_loggedUserService.LoggedUser);
        }

        [TestMethod]
        public async Task NaoDeveRealizarUsernameVazio()
        {
            var username = "";
            var pass = "TestPass";

            var vm = new LoginViewModel(_loggedUserService, _userRepo);
            vm.LoginModel.Username = username;
            vm.LoginModel.Password = pass;

            await vm.DoLogin();

            Assert.IsNull(_loggedUserService.LoggedUser);
            Assert.AreEqual(Messages.UsernameEmptyMessage, vm.LoginModel.ErrorMessage);
        }

        [TestMethod]
        public async Task NaoDeveRealizarPasswordVazio()
        {
            var username = "TestUser";
            var pass = "";

            var vm = new LoginViewModel(_loggedUserService, _userRepo);
            vm.LoginModel.Username = username;
            vm.LoginModel.Password = pass;

            await vm.DoLogin();

            Assert.IsNull(_loggedUserService.LoggedUser);
            Assert.AreEqual(Messages.PasswordEmptyMessage, vm.LoginModel.ErrorMessage);
        }

        [TestMethod]
        public async Task NaoDeveRealizarLoginUsuarioInexistente()
        {
            var username = "TestUser";
            var pass = "TestPass";

            await _userRepo.SaveNewUser(username, pass);

            var vm = new LoginViewModel(_loggedUserService, _userRepo);
            vm.LoginModel.Username = username;
            vm.LoginModel.Password = "SomeWrongPass";

            await vm.DoLogin();

            Assert.IsNull(_loggedUserService.LoggedUser);
            Assert.AreEqual(Messages.UserPasswordWrongMessage, vm.LoginModel.ErrorMessage);
        }
    }
}
