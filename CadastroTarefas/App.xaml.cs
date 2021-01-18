using DataLayer;
using System.Windows;

namespace CadastroTarefas
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static User LoggedUser { get; set; }
        public static LiteDB.LiteDatabase Database { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Database = new LiteDB.LiteDatabase("Filename=appDatabase.db;Connection=shared");
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Database.Dispose();
        }
    }
}