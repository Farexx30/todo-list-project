using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ToDoList.Commands;
using ToDoList.Models.Dtos;
using ToDoList.Models.Entities;
using ToDoList.Models.Repositories;
using ToDoList.Services;

namespace ToDoList.ViewModels
{
    public class MainAppViewModel : BaseViewModel
    {
        private readonly IUserContextService _userContextService;
        private readonly ICategoryRepository _categoryRepo;
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly IAssignmentStepRepository _assignmentStepRepo;

        private UserDto _currentUser = null!;

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

        //Bindings:


        //User bindings:
        private string _username = null!;
        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged();
            }
        }


        //Category bindings:
        private string _currentCategoryName = null!;
        public string CurrentCategoryName
        {
            get => _currentCategoryName;
            set
            {
                _currentCategoryName = value;
                OnPropertyChanged();
            }
        }

        private string _newCategoryName = null!;
        public string NewCategoryName
        {
            get => _newCategoryName;
            set
            {
                _newCategoryName = value;
                OnPropertyChanged();
            }
        }

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

        private CategoryDto? _currentCategory;
        public CategoryDto? CurrentCategory
        {
            get => _currentCategory;
            set
            {
                _currentCategory = value;
                OnPropertyChanged();
            }
        }


        //Assignment bindings:
        private string _currentAssignmentName = null!;
        public string CurrentAssignmentName
        {
            get => _currentAssignmentName;
            set
            {
                _currentAssignmentName = value;
                OnPropertyChanged();
            }
        }

        private string _newAssignmentName = null!;
        public string NewAssignmentName
        {
            get => _newAssignmentName;
            set
            {
                _newAssignmentName = value;
                OnPropertyChanged();
            }
        }

        private DateTime _assignmentDeadline;
        public DateTime AssignmentDeadline
        {
            get => _assignmentDeadline;
            set
            {
                _assignmentDeadline = value;
                OnPropertyChanged();
            }
        }

        private bool _isAssignmentChecked;
        public bool IsAssignmentChecked
        {
            get => _isAssignmentChecked;
            set
            {
                _isAssignmentChecked = value;
                OnPropertyChanged();
            }
        }

        private bool _isAssignmentImportant;
        public bool IsAssignmentImportant
        {
            get => _isAssignmentImportant;
            set
            {
                _isAssignmentImportant = value;
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

        private AssignmentDto? _currentAssignment;
        public AssignmentDto? CurrentAssignment
        {
            get => _currentAssignment;
            set
            {
                _currentAssignment = value;
                OnPropertyChanged();
            }
        }


        //AssignmentSteps bindings:
        private string _currentAssignmentStepName = null!;
        public string CurrentAssignmentStepName
        {
            get => _currentAssignmentStepName;
            set
            {
                _currentAssignmentStepName = value;
                OnPropertyChanged();
            }
        }

        private string _newAssignmentStepName = null!;
        public string NewAssignmentStepName
        {
            get => _newAssignmentStepName;
            set
            {
                _newAssignmentStepName = value;
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

        private AssignmentStepDto? _currentAssignmentStep;
        public AssignmentStepDto? CurrentAssignmentStep
        {
            get => _currentAssignmentStep;
            set
            {
                _currentAssignmentStep = value;
                OnPropertyChanged();
            }
        }


        //Commands:
        public ICommand AddCategoryCommand { get; set; }
        public ICommand UpdateCategoryCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }
        public ICommand AddAssignmentCommand { get; set; }
        public ICommand UpdateAssignmentCommand { get; set; }
        public ICommand DeleteAssignmentCommand { get; set; }
        public ICommand ShareAssignmentCommand { get; set; }
        public ICommand AddAssignmentStepCommand { get; set; }
        public ICommand UpdateAssignmentStepCommand { get; set; }
        public ICommand DeleteAssignmentStepCommand { get; set; }
        public ICommand CategoryChangedCommand { get; set; }
        public ICommand AssignmentChangedCommand { get; set; }
        public ICommand LogOutCommand { get; set; }


        public MainAppViewModel(INavigationService navigationService, IUserContextService userContextService, ICategoryRepository categoryRepo, IAssignmentRepository assignmentRepo, IAssignmentStepRepository assignmentStepRepo)
        {
            _navigationService = navigationService;
            _userContextService = userContextService;
            _categoryRepo = categoryRepo;
            _assignmentRepo = assignmentRepo;
            _assignmentStepRepo = assignmentStepRepo;

            AddCategoryCommand = new RelayCommand(AddCategory, _ => true);
            UpdateCategoryCommand = new RelayCommand(UpdateCategory);
            DeleteCategoryCommand = new RelayCommand(DeleteCategory, _ => true);

            AddAssignmentCommand = new RelayCommand(AddAssignment, _ => true);
            UpdateAssignmentCommand = new RelayCommand(UpdateAssignment);
            DeleteAssignmentCommand = new RelayCommand(DeleteAssignment, _ => true);
            ShareAssignmentCommand = new RelayCommand(ShareAssignment, _ => true);

            AddAssignmentStepCommand = new RelayCommand(AddAssignmentStep, _ => true);
            UpdateAssignmentStepCommand = new RelayCommand(UpdateAssignmentStep, _ => true);
            DeleteAssignmentStepCommand = new RelayCommand(DeleteAssignmentStep, _ => true);

            CategoryChangedCommand = new RelayCommand(CategoryChanged, _ => true);
            AssignmentChangedCommand = new RelayCommand(AssignmentChanged, _ => true);
            LogOutCommand = new RelayCommand(LogOut, _ => true);

            Initialize();
        }

        private void Initialize()
        {
            _currentUser = _userContextService.CurrentUser!;

            Username = _currentUser.Name;
            Categories = new(_categoryRepo.GetCategories(_currentUser.Id));
        }


        //Category actions:
        private void AddCategory(object obj)
        {
            var newCategoryDto = new CategoryDto
            {
                Name = NewCategoryName,
                IsBuiltIn = false
            };
            var newCategoryWithIdDto = _categoryRepo.AddCategory(newCategoryDto, _currentUser.Id);

            if (newCategoryWithIdDto is null)
            {
                //Błąd! Kategoria o takiej nazwie juz istnieje...
            }
            else
            {
                Categories.Add(newCategoryWithIdDto);
                //CurrentCategory = newCategoryWithIdDto; //Raczej niepotrzebne - podobnie jak przy Assignment itd. (zalezy czy po dodaniu do listboxa automatycznie zaznacza sie nowo dodany item).
            }
        }

        private void UpdateCategory(object obj)
        {
            CurrentCategory!.Name = CurrentCategoryName;
            _categoryRepo.UpdateCategory(CurrentCategory);
        }

        private void DeleteCategory(object obj)
        {
            _categoryRepo.DeleteCategory(CurrentCategory!.Id);
            Categories.Remove(CurrentCategory);
            //i czy CurrentCategory jest nullem juz ???
        }


        //Assignment actions:
        private void AddAssignment(object obj)
        {
            var newAssignmentDto = new AssignmentDto
            {
                Name = NewAssignmentName,
                IsChecked = false,
                IsImportant = false,
            };
            var newAssignmentWithIdDto = _assignmentRepo.AddAssignment(newAssignmentDto, _currentUser.Id, CurrentCategory!.Id);

            ToDoAssignments.Add(newAssignmentWithIdDto);
        }

        private void UpdateAssignment(object obj)
        {
            CurrentAssignment!.Name = CurrentAssignmentName;
            CurrentAssignment.Deadline = AssignmentDeadline;
            CurrentAssignment.IsChecked = IsAssignmentChecked;
            CurrentAssignment.IsImportant = IsAssignmentImportant;

            _assignmentRepo.UpdateAssignment(CurrentAssignment);
        }

        private void DeleteAssignment(object obj)
        {
            _assignmentRepo.DeleteAssignment(CurrentAssignment!.Id);

            if(CurrentAssignment.IsChecked)
            {
                CompletedAssignments.Remove(CurrentAssignment);
            }
            else
            {
                ToDoAssignments.Remove(CurrentAssignment);
            }
            //i czy CurrentAssignment jest nullem juz ???
        }

        private void ShareAssignment(object obj)
        {
            _assignmentRepo.ShareAssignment(CurrentAssignment!.Id);
        }


        //AssignmentStep actions:
        private void AddAssignmentStep(object obj)
        {
            var newAssignmentStepDto = new AssignmentStepDto
            {
                Name = NewAssignmentStepName,
            };
            var assignmentStepWithIdDto = _assignmentStepRepo.AddAssignmentStep(newAssignmentStepDto, CurrentAssignment!.Id);

            AssignmentSteps.Add(assignmentStepWithIdDto);
        }

        private void UpdateAssignmentStep(object obj)
        {
            CurrentAssignmentStep!.Name = CurrentAssignmentStepName;
            _assignmentStepRepo.UpdateAssignmentStep(CurrentAssignmentStep);
        }

        private void DeleteAssignmentStep(object obj)
        {
            _assignmentStepRepo.DeleteAssignmentStep(CurrentAssignmentStep!.Id);
            AssignmentSteps.Remove(CurrentAssignmentStep);
            //i czy tutaj już CurrentAssignmentStep bedzie null???
        }


        //Eventy, albo cos w tym rodzaju, zobaczy sie.
        private void CategoryChanged(object obj)
        {
            _assignmentStepRepo.SaveAssignmentStepsChanges();
            //i reszta... Obsluzenie zdarzenia przejscia do innej kategorii...
        }

        private void AssignmentChanged(object obj)
        {
            _assignmentStepRepo.SaveAssignmentStepsChanges();
            //i reszta... Obsluzenie zdarzenia przejscia do innego Assignment...
        }

        private void LogOut(object obj)
        {
            _assignmentStepRepo.SaveAssignmentStepsChanges();
            //i reszta... np. aktualizacja wszystkich ewentualnych pozostalosci...

            _userContextService.CurrentUser = null;
            NavigationService.NavigateTo<MainMenuViewModel>();
        }

        //I jeszcze trzeba bedzie CanExecuty porobić oraz pokombinować z zapisem przy ewentualnym zamknieciu aplikacji.
    }
}
