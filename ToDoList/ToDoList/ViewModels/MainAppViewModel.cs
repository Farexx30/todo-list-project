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
using ToDoList.Services;
using ToDoList.Services.Repositories;

namespace ToDoList.ViewModels
{
    public class MainAppViewModel : BaseViewModel
    {
        private readonly IUserContextService _userContextService;
        private readonly ICategoryRepositoryService _categoryRepo;
        private readonly IAssignmentRepositoryService _assignmentRepo;
        private readonly IAssignmentStepRepositoryService _assignmentStepRepo;

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

        //Bindingi (jeszcze nie wszystkie są):
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


        public MainAppViewModel(INavigationService navigationService, IUserContextService userContextService, ICategoryRepositoryService categoryRepo, IAssignmentRepositoryService assignmentRepo, IAssignmentStepRepositoryService assignmentStepRepo)
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
                Name = "", //Bedzie binding !!!!!!!!          <--------
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
            CurrentCategory!.Name = ""; //Bedzie binding !!!!!!!!        <------------
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
                Name = "", //Bedzie binding !!!!!!!!          <--------
                IsChecked = false,
                IsImportant = false,
            };
            var newAssignmentWithIdDto = _assignmentRepo.AddAssignment(newAssignmentDto, _currentUser.Id, CurrentCategory!.Id);

            ToDoAssignments.Add(newAssignmentWithIdDto);
        }

        private void UpdateAssignment(object obj)
        {
            CurrentAssignment!.Name = ""; //Bedzie binding !!!!!!!!      <--------
            CurrentAssignment.Deadline = DateTime.Now; //Bedzie binding !!!!!!!!      <--------
            CurrentAssignment.IsChecked = true; //Bedzie binding !!!!!!!!      <--------
            CurrentAssignment.IsImportant = true; //Bedzie binding !!!!!!!!      <--------

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
                Name = "", //Bedzie binding !!!!!!!!          <--------
            };
            var assignmentStepWithIdDto = _assignmentStepRepo.AddAssignmentStep(newAssignmentStepDto, CurrentAssignment!.Id);

            AssignmentSteps.Add(assignmentStepWithIdDto);
        }
        private void UpdateAssignmentStep(object obj)
        {
            CurrentAssignmentStep!.Name = ""; // Bedzie binding !!!!!! <--------------
            _assignmentStepRepo.UpdateAssignmentStep(CurrentAssignmentStep);
        }

        private void DeleteAssignmentStep(object obj)
        {
            _assignmentStepRepo.DeleteAssignmentStep(CurrentAssignmentStep!.Id);
            AssignmentSteps.Remove(CurrentAssignmentStep);
            //i czy tutaj już CurrentAssignmentStep bedzie null???
        }

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
