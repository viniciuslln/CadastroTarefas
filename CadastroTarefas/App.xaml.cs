using CadastroTarefas.Services;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace CadastroTarefas
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static LoggedUserService LoggedUserService { get; private set; }
        public static LiteDB.LiteDatabase Database { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            LoggedUserService = new LoggedUserService();
            LoggedUserService.UserLogged += OnUserLogged;
            LoggedUserService.UserSignout += OnUserSignOut;
            Database = new LiteDB.LiteDatabase("Filename=appDatabase.db;Connection=shared");
        }

        private void OnUserSignOut()
        {
            ClearHistory();
            (MainWindow as NavigationWindow)?.NavigationService?.Navigate(new Uri("Pages/LoginPage.xaml", UriKind.Relative));
        }

        private void OnUserLogged()
        {
            (MainWindow as NavigationWindow)?.NavigationService?.Navigate(new Uri("Pages/TasksPage.xaml", UriKind.Relative));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            LoggedUserService.UserLogged -= OnUserLogged;
            LoggedUserService.UserSignout -= OnUserSignOut;
            Database.Dispose();
        }

        public void ClearHistory()
        {
            if ((MainWindow as NavigationWindow)?.CanGoBack == false && (MainWindow as NavigationWindow)?.CanGoForward == false)
            {
                return;
            }

            var entry = (MainWindow as NavigationWindow)?.RemoveBackEntry();
            while (entry != null)
            {
                entry = (MainWindow as NavigationWindow)?.RemoveBackEntry();
            }

        }
    }
}