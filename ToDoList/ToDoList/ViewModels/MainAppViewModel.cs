using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Commands;
using ToDoList.Services;

namespace ToDoList.ViewModels
{
    public class MainAppViewModel : BaseViewModel
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

        public RelayCommand NavigateToMainMenuCommand { get; set; } = null!;

        public MainAppViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToMainMenuCommand = new RelayCommand(GoToMainMenu, _ => true);
        }

        private void GoToMainMenu(object obj) => _navigationService.NavigateTo<MainMenuViewModel>();
    }
}
