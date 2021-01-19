﻿using CadastroTarefas.Models;
using CadastroTarefas.Resources;
using DataLayer;
using MvvmHelpers;
using MvvmHelpers.Commands;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

namespace CadastroTarefas.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly UserRepository _userRepository;

        public LoginModel LoginModel { get; set; }

        public ICommand LoginCommand { get; }

        public ICommand SignUpCommand { get; }

        public LoginViewModel()
        {
            _userRepository = new UserRepository(App.Database);
            LoginModel = new LoginModel();
            LoginCommand = new AsyncCommand(DoLogin);
            SignUpCommand = new Command(GoToSignUp);
        }

        private void GoToSignUp(object obj)
        {
            NavigateToSignUpPage();
        }

        private async Task DoLogin()
        {
            if (!LoginModel.Validate())
            {
                return;
            }

            var user = await _userRepository.GetRegisteredUser(LoginModel.Username, LoginModel.PlainPassword);
            if (user != null)
            {
                App.LoggedUser = user;
                NavigateToMainPage();
            }
            else
            {
                LoginModel.ErrorMessage = Messages.UserPasswordWrongMessage;
            }
        }

        private void NavigateToMainPage()
        {
            (Application.Current.MainWindow as NavigationWindow)?.NavigationService.Navigate(new Uri("Pages/TasksPage.xaml", UriKind.Relative));
        }

        private void NavigateToSignUpPage()
        {
            (Application.Current.MainWindow as NavigationWindow)?.NavigationService.Navigate(new Uri("Pages/SignUpPage.xaml", UriKind.Relative));
        }
    }
}