using CadastroTarefas.ViewModels;
using DataLayer;
using System.Windows.Controls;

namespace CadastroTarefas.Pages
{
    /// <summary>
    /// Interaction logic for SingUpPage.xaml
    /// </summary>
    public partial class SignUpPage : Page
    {
        public SignUpPage()
        {
            InitializeComponent();
            DataContext = new SignUpViewModel(App.LoggedUserService, new UserRepository(App.Database));
        }
    }
}