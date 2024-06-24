using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDoList.Commands;
using ToDoList.Models.Dtos;
using ToDoList.Models.Repositories;
using ToDoList.Services;

namespace ToDoList.ViewModels
{
    public class LoginViewModel : BaseViewModel
    {
        private readonly ILoginUserRepository _loginUserRepo;
        private readonly IUserContextService _userContextService;

        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get => _navigationService;
            private set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        //Bindingi:
        private string _username = string.Empty;
        public string Username {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }

        private string _password = string.Empty;
        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; set; }
        public ICommand GoBackToMenuCommand { get; set; }

        public LoginViewModel(INavigationService navigationService, ILoginUserRepository loginUserRepo, IUserContextService userContextService)
        {
            _navigationService = navigationService;
            _loginUserRepo = loginUserRepo;
            _userContextService = userContextService;

            LoginCommand = new RelayCommand(Login, CanLogin);
            GoBackToMenuCommand = new RelayCommand(GoBackToMenu, _ => true);
        }

        private void GoBackToMenu(object obj)
        {
            NavigationService.NavigateTo<MainMenuViewModel>();
        }

        private void Login(object obj)
        {
            var loginUserDto = new RegisterOrLoginUserDto
            {
                Name = Username.Trim(),
                Password = Password.Trim(),
            };
            var loggedInUserDto = _loginUserRepo.LoginUser(loginUserDto);

            if (loggedInUserDto is not null)
            {
                _userContextService.CurrentUser = loggedInUserDto;
                NavigationService.NavigateTo<MainAppViewModel>();
            }
            else
            {
                MessageBox.Show("Niepoprawny username lub hasło");
            }
        }

        private bool CanLogin(object obj) => !string.IsNullOrEmpty(Username.Trim())
                                             && !string.IsNullOrEmpty(Password.Trim())
                                             && (Password.Trim().Length>=6);
    }
}
