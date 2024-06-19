using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoList.Commands;
using ToDoList.Models.Dtos;
using ToDoList.Services;

namespace ToDoList.ViewModels
{
    public class MainAppViewModel : BaseViewModel
    {
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
        public string Username { get; private set; } = null!;

        private ObservableCollection<CategoryDto> _categories = [];
        public ObservableCollection<CategoryDto> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogOutCommand { get; set; }

        public MainAppViewModel(INavigationService navigationService, IUserContextService userContextService)
        {
            _navigationService = navigationService;
            _userContextService = userContextService;

            LogOutCommand = new RelayCommand(LogOut, _ => true);

            Initialize();
        }

        private void Initialize()
        {
            Username = _userContextService.CurrentUserDto!.Name;
        }

        private void LogOut(object obj)
        {
            _userContextService.CurrentUserDto = null;
            NavigationService.NavigateTo<MainMenuViewModel>();
        }
    }
}
