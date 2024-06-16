using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Commands;
using ToDoList.Services;
using ToDoList.ViewModels;

namespace ToDoList
{
    public class MainViewModel : BaseViewModel
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

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            Initialize();
        }

        private void Initialize() => GoToMainMenu();

        private void GoToMainMenu() => NavigationService.NavigateTo<MainMenuViewModel>();
    }
}
