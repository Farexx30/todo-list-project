using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public RelayCommand NavigateToLoginCommand { get; set; } = null!;
        public RelayCommand NavigateToRegisterCommand { get; set; } = null!;

        public MainMenuViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToLoginCommand = new RelayCommand(GoToLogin, _ => true);
            NavigateToRegisterCommand = new RelayCommand(GoToRegister, _ => true);
        }

        private void GoToLogin(object obj) => _navigationService.NavigateTo<LoginViewModel>();
        private void GoToRegister(object obj) => _navigationService.NavigateTo<RegisterViewModel>();
    }
}
