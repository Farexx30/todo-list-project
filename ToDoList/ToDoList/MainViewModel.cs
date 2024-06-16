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

            NavigateToMainMenuCommand = new RelayCommand(GoToMainMenu, _ => true);

            Initialize();
        }

        private void Initialize() => GoToMainMenu(this);

        private void GoToMainMenu(object obj) => NavigationService.NavigateTo<MainMenuViewModel>();
    }
}
