using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToDoList.Commands;
using ToDoList.Services;

namespace ToDoList.ViewModels
{
    public class RegisterViewModel : BaseViewModel
    {
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

        public RelayCommand NavigateToMainAppCommand { get; set; }

        public RegisterViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToMainAppCommand = new RelayCommand(GoToMainApp, _ => true);
        }

        private void GoToMainApp(object obj)
        {
            MessageBox.Show(Username);
            MessageBox.Show(Password);
            MessageBox.Show(PasswordConfirmation);
            NavigationService.NavigateTo<MainAppViewModel>();
        }
    }
}
