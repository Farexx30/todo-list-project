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

        public RelayCommand NavigateToMainMenuCommand { get; set; } = null!;

        public MainViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService;

            NavigateToMainMenuCommand = new RelayCommand(NavigateToMainMenu, _ => true);

            Initialize();
        }

        private void Initialize() => NavigateToMainMenu(this);

        private void NavigateToMainMenu(object obj) => NavigationService.NavigateTo<MainMenuViewModel>();
    }
}
