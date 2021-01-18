using CadastroTarefas.ViewModels;
using System.Windows.Controls;

namespace CadastroTarefas.Pages
{
    /// <summary>
    /// Interaction logic for TasksPage.xaml
    /// </summary>
    public partial class TasksPage : Page
    {
        public TasksPage()
        {
            InitializeComponent();
            DataContext = new TasksViewModel();
        }
    }
}