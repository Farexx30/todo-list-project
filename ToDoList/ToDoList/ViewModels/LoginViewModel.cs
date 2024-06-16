using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Commands;
using ToDoList.Services;

namespace ToDoList.ViewModels
{
    public class LoginViewModel : BaseViewModel
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

        public RelayCommand NavigateToMainAppCommand { get; set; }

        public LoginViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToMainAppCommand = new RelayCommand(GoToMainApp, _ => true);
        }

        private void GoToMainApp(object obj) => NavigationService.NavigateTo<MainAppViewModel>();
    }
}
