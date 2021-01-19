using CadastroTarefas.ViewModels;
using DataLayer;
using System.Windows.Controls;

namespace CadastroTarefas.Pages
{
    /// <summary>
    /// Interaction logic for LoginPage.xaml
    /// </summary>
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            DataContext = new LoginViewModel(App.LoggedUserService, new UserRepository(App.Database));
        }
    }
}