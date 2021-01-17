using DataLayer;
using System.Windows;

namespace CadastroTarefas
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            using (var db = new CadastroTarefasContext())
            {
                db.Database.EnsureCreated();
            }
            base.OnStartup(e);
        }
    }
}
