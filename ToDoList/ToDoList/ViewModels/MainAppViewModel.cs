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
        private CategoryDto? _previousCategory;

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

        private CategoryDto? _currentCustomCategory;
        public CategoryDto? CurrentCustomCategory
        {
            get => _currentCustomCategory;
            set
            {
                _currentCustomCategory = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AssignmentDto> _toDoAssignments = [];
        public ObservableCollection<AssignmentDto> ToDoAssignments
        {
            get => _toDoAssignments;
            set
            {
                _toDoAssignments = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AssignmentDto> _completedAssignments = [];
        public ObservableCollection<AssignmentDto> CompletedAssignments
        {
            get => _completedAssignments;
            set
            {
                _completedAssignments = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<AssignmentStepDto> _assignmentSteps = [];
        public ObservableCollection<AssignmentStepDto> AssignmentSteps
        {
            get => _assignmentSteps;
            set
            {
                _assignmentSteps = value;
                OnPropertyChanged();
            }
        }


        public ICommand CategoryChangedCommand { get; set; }
        public ICommand LogOutCommand { get; set; }


        public MainAppViewModel(INavigationService navigationService, IUserContextService userContextService)
        {
            _navigationService = navigationService;
            _userContextService = userContextService;

            CategoryChangedCommand = new RelayCommand(CategoryChanged, _ => true);
            LogOutCommand = new RelayCommand(LogOut, _ => true);

            Initialize();
        }

        private void Initialize()
        {
            Username = _userContextService.CurrentUserDto!.Name;
            //Categories = ....
        }

        private void CategoryChanged(object obj)
        {
           //aktualizacja poprzedniej kategorii... (w repo bede trzymac te pobrane juz kategorie itd).
           //_previousCategory = Cur
        }

        private void LogOut(object obj)
        {
            //aktualizacja aktualnej kategorii...
            _userContextService.CurrentUserDto = null;
            NavigationService.NavigateTo<MainMenuViewModel>();
        }
    }
}
