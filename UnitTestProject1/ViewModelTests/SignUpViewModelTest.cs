using CadastroTarefas.Resources;
using CadastroTarefas.Services;
using CadastroTarefas.ViewModels;
using DataLayer;
using LiteDB;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Threading.Tasks;

namespace UnitTestProject1.ViewModelTests
{

    [TestClass]
    public class SignUpViewModelTest
    {
        private readonly LiteDatabase _db;
        private readonly MemoryStream _memoryStream;
        private readonly LoggedUserService _loggedUserService;
        private readonly UserRepository _userRepo;

        public SignUpViewModelTest()
        {
            _memoryStream = new MemoryStream();
            _db = new LiteDatabase(_memoryStream);
            var _loggedUserServiceMoq = new Moq.Mock<LoggedUserService>();
            _loggedUserService = _loggedUserServiceMoq.Object;
            _userRepo = new UserRepository(_db);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _memoryStream.Dispose();
        }

        [TestMethod]
        public async Task DeveRealizarRegistro()
        {
            var username = "TestUser";
            var pass = "TestPass";

            var vm = new SignUpViewModel(_loggedUserService, _userRepo);
            vm.LoginModel.Username = username;
            vm.LoginModel.Password = pass;

            await vm.DoSignUp();

            Assert.IsNotNull(_loggedUserService.LoggedUser);
        }

        [TestMethod]
        public async Task NaoDeveRealizarUsernameVazio()
        {
            var username = "";
            var pass = "TestPass";

            var vm = new SignUpViewModel(_loggedUserService, _userRepo);
            vm.LoginModel.Username = username;
            vm.LoginModel.Password = pass;

            await vm.DoSignUp();

            Assert.IsNull(_loggedUserService.LoggedUser);
            Assert.AreEqual(Messages.UsernameEmptyMessage, vm.LoginModel.ErrorMessage);
        }

        [TestMethod]
        public async Task NaoDeveRealizarPasswordVazio()
        {
            var username = "TestUser";
            var pass = "";

            var vm = new SignUpViewModel(_loggedUserService, _userRepo);
            vm.LoginModel.Username = username;
            vm.LoginModel.Password = pass;

            await vm.DoSignUp();

            Assert.IsNull(_loggedUserService.LoggedUser);
            Assert.AreEqual(Messages.PasswordEmptyMessage, vm.LoginModel.ErrorMessage);
        }

        [TestMethod]
        public async Task NaoDeveRealizarRegistroUsuarioExistente()
        {
            var username = "TestUser";
            var pass = "TestPass";

            await _userRepo.SaveNewUser(username, pass);

            var vm = new SignUpViewModel(_loggedUserService, _userRepo);
            vm.LoginModel.Username = username;
            vm.LoginModel.Password = pass;

            await vm.DoSignUp();

            Assert.IsNull(_loggedUserService.LoggedUser);
            Assert.AreEqual(Messages.UserAlreadyRegisteredMessage, vm.LoginModel.ErrorMessage);
        }
    }
}
