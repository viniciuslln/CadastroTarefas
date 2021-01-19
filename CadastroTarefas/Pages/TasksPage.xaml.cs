using CadastroTarefas.ViewModels;
using MvvmHelpers;
using System.Windows.Controls;

namespace CadastroTarefas.Pages
{
    /// <summary>
    /// Interaction logic for TasksPage.xaml
    /// </summary>
    public partial class TasksPage : Page
    {
        private readonly TasksViewModel vm;

        public TasksPage()
        {
            DataContext = vm = new TasksViewModel(App.LoggedUserService, new UserTaskRepository(App.Database));
            InitializeComponent();
        }

        private void Page_Initialized(object sender, System.EventArgs e)
        {
            vm.LoadTasks().SafeFireAndForget();
        }
    }
}