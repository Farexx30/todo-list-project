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
    public class RegisterViewModel : BaseViewModel
    {
        private readonly IRegisterUserRepository _registerUserRepo;
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
        public string Username
        {
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

        private string _passwordConfirmation = string.Empty;
        public string PasswordConfirmation
        {
            get => _passwordConfirmation;
            set
            {
                _passwordConfirmation = value;
                OnPropertyChanged();
            }
        }

        public ICommand RegisterCommand { get; set; }

        public RegisterViewModel(INavigationService navigationService, IRegisterUserRepository registerUserRepo, IUserContextService userContextService)
        {
            _navigationService = navigationService;
            _registerUserRepo = registerUserRepo;
            _userContextService = userContextService;

            RegisterCommand = new RelayCommand(Register, CanRegister);
        }

        private void Register(object obj)
        {
            var newUserDto = new RegisterOrLoginUserDto
            {
                Name = Username.Trim(),
                Password = Password.Trim(),
            };
            var loggedInUserDto = _registerUserRepo.RegisterUser(newUserDto);

            if (loggedInUserDto is not null)
            {   
                _userContextService.CurrentUser = loggedInUserDto;
                NavigationService.NavigateTo<MainAppViewModel>();
            }
            else
            {
                MessageBox.Show("Taki uzytkownik juz istnieje");
            }
        }

        private bool CanRegister(object obj) => !string.IsNullOrEmpty(Username.Trim())
                                             && !string.IsNullOrEmpty(Password.Trim())
                                             && Password.Trim() == PasswordConfirmation.Trim();
    }
}
