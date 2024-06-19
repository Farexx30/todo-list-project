using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoList.Commands;
using ToDoList.Services;

namespace ToDoList.ViewModels
{
    public class MainMenuViewModel : BaseViewModel
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

        public ICommand NavigateToLoginCommand { get; set; }
        public ICommand NavigateToRegisterCommand { get; set; }

        public MainMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToLoginCommand = new RelayCommand(GoToLogin, _ => true);
            NavigateToRegisterCommand = new RelayCommand(GoToRegister, _ => true);
        }

        private void GoToLogin(object obj) => NavigationService.NavigateTo<LoginViewModel>();
        private void GoToRegister(object obj) => NavigationService.NavigateTo<RegisterViewModel>();
    }
}
