using DataLayer;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace CadastroTarefas
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User LoggedUser { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            using (var db = new CadastroTarefasContext())
            {
                db.Database.EnsureCreated();
                db.Database.Migrate();
            }
            base.OnStartup(e);
        }
    }
}