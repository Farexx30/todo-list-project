using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using ToDoList.Commands;
using ToDoList.Models;
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
        private CategoryMode _categoryMode = CategoryMode.MyDay;

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
        private string _username = string.Empty;
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

        private string _currentCategoryName = string.Empty;
        public string CurrentCategoryName
        {
            get => _currentCategoryName;
            set
            {
                _currentCategoryName = value;
                OnPropertyChanged();
            }
        }

        private string _newCategoryName = string.Empty;
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
                DbCategoryChanged();
            }
        }

        private CategoryDto? _builtInCategory;
        public CategoryDto? BuiltInCategory
        {
            get => _builtInCategory;
            set
            {
                _builtInCategory = value;
                OnPropertyChanged();
            }
        }


        //Assignment bindings:
        private string _newAssignmentName = string.Empty;
        public string NewAssignmentName
        {
            get => _newAssignmentName;
            set
            {
                _newAssignmentName = value;
                OnPropertyChanged();
            }
        }

        private DateTime? _assignmentDeadline;
        public DateTime? AssignmentDeadline
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
                AssignmentChanged();
            }
        }


        //AssignmentSteps bindings:
        private string _currentAssignmentStepName = string.Empty;
        public string CurrentAssignmentStepName
        {
            get => _currentAssignmentStepName;
            set
            {
                _currentAssignmentStepName = value;
                OnPropertyChanged();
            }
        }

        private string _newAssignmentStepName = string.Empty;
        public string NewAssignmentStepName
        {
            get => _newAssignmentStepName;
            set
            {
                _newAssignmentStepName = value;
                OnPropertyChanged();
            }
        }

        private bool _isAssignmentStepChecked;
        public bool IsAssignmentStepChecked
        {
            get => _isAssignmentStepChecked;
            set
            {
                _isAssignmentStepChecked = value;
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
        public ICommand MyDayCategoryClickedCommand { get; set; }
        public ICommand ImportantCategoryClickedCommand { get; set; }
        public ICommand PlannedCategoryClickedCommand { get; set; }
        public ICommand BuiltInCategoryClickedCommand { get; set; }
        public ICommand CategoryNameLostFocusCommand { get; set; }
        public ICommand AssignmentNameLostFocusCommand { get; set; }
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

            CategoryNameLostFocusCommand = new RelayCommand(CategoryNameLostFocus);
            AssignmentNameLostFocusCommand = new RelayCommand(AssignmentNameLostFocus);

            MyDayCategoryClickedCommand = new RelayCommand(MyDayCategoryClicked, _ => true);
            ImportantCategoryClickedCommand = new RelayCommand(ImportantCategoryClicked, _ => true);
            PlannedCategoryClickedCommand = new RelayCommand(PlannedCategoryClicked, _ => true);
            BuiltInCategoryClickedCommand = new RelayCommand(BuiltInCategoryClicked, _ => true);
            LogOutCommand = new RelayCommand(LogOut, _ => true);

            Initialize();
        }

        private void Initialize()
        {
            _currentUser = _userContextService.CurrentUser!;

            Username = _currentUser.Name;
            BuiltInCategory = _categoryRepo.GetBuiltInCategory();
            Categories = new(_categoryRepo.GetCategories(_currentUser.Id));
            MyDayCategoryClicked(this);
        }


        //Add Category:
        private void AddCategory(object obj)
        {
            var newCategoryDto = new CategoryDto
            {
                Name = "Category",
                IsBuiltIn = false
            };
            var newCategoryWithIdDto = _categoryRepo.AddCategory(newCategoryDto, _currentUser.Id);

            VerifyNewCategory(newCategoryWithIdDto);
        }

        private void VerifyNewCategory(CategoryDto? newCategory)
        {
            if (newCategory is null)
            {
                MessageBox.Show("Taka kategoria już istnieje");
            }
            else
            {
                Categories.Add(newCategory);
                CurrentCategory = newCategory;
            }
        }

        //Update Category:
        private void UpdateCategory(object obj)
        {
            CurrentCategory!.Name = CurrentCategoryName;
            _categoryRepo.UpdateCategory(CurrentCategory);
        }

        //Update Category:
        private void DeleteCategory(object obj)
        {
            _categoryRepo.DeleteCategory(CurrentCategory!.Id);
            Categories.Remove(CurrentCategory);
            //i czy CurrentCategory jest nullem juz ???
        }


        //Add Assignment:
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

        //Update Assignment:
        private void UpdateAssignment(object obj)
        {
            CurrentAssignment!.Deadline = AssignmentDeadline is not null 
                ? DateOnly.FromDateTime(AssignmentDeadline.Value) 
                : null;
            CurrentAssignment.IsChecked = IsAssignmentChecked;
            CurrentAssignment.IsImportant = IsAssignmentImportant;

            _assignmentRepo.UpdateAssignment(CurrentAssignment);
        }

        //Delete Assignment:
        private void DeleteAssignment(object obj)
        {
            _assignmentRepo.DeleteAssignment(CurrentAssignment!.Id);

            if (CurrentAssignment.IsChecked)
            {
                CompletedAssignments.Remove(CurrentAssignment);
            }
            else
            {
                ToDoAssignments.Remove(CurrentAssignment);
            }
            //i czy CurrentAssignment jest nullem juz ???
        }

        //Share Assignment
        private void ShareAssignment(object obj)
        {
            _assignmentRepo.ShareAssignment(CurrentAssignment!.Id);
        }


        //Add AssignmentStep:
        private void AddAssignmentStep(object obj)
        {
            var newAssignmentStepDto = new AssignmentStepDto
            {
                Name = NewAssignmentStepName,
                IsChecked = false
            };
            var assignmentStepWithIdDto = _assignmentStepRepo.AddAssignmentStep(newAssignmentStepDto, CurrentAssignment!.Id);

            AssignmentSteps.Add(assignmentStepWithIdDto);
        }

        //Update AssignmentStep:
        private void UpdateAssignmentStep(object obj)
        {
            CurrentAssignmentStep!.Name = CurrentAssignmentStepName;
            CurrentAssignmentStep.IsChecked = IsAssignmentChecked;
            _assignmentStepRepo.UpdateAssignmentStep(CurrentAssignmentStep);
        }

        //DeleteAssignmentStep:
        private void DeleteAssignmentStep(object obj)
        {
            _assignmentStepRepo.DeleteAssignmentStep(CurrentAssignmentStep!.Id);
            AssignmentSteps.Remove(CurrentAssignmentStep);
            //i czy tutaj już CurrentAssignmentStep bedzie null???
        }


        //Category changed:
        private void DbCategoryChanged()
        {
            _assignmentStepRepo.SaveAssignmentStepsChanges();

            if (CurrentCategory is not null)
            {
                CurrentCategoryName = CurrentCategory.Name;
                _categoryMode = CategoryMode.Database;
                AssignmentSteps.Clear();

                var loadedAssignments = _assignmentRepo.GetAssignments(_categoryMode, _currentUser.Id, CurrentCategory.Id);
                SetAssignmentsCollections(loadedAssignments);
            }
        }

        //AssignmentChanged:
        private void AssignmentChanged()
        {
            _assignmentStepRepo.SaveAssignmentStepsChanges();

            if (CurrentAssignment is not null)
            {
                AssignmentSteps = new(_assignmentStepRepo.GetAssignmentSteps(CurrentAssignment.Id));
            }
        }

        //LostFocuses:
        private void CategoryNameLostFocus(object obj)
        {
            MessageBox.Show("CategoryNameLostFocus");

            UpdateCategory(this);

            //To do Xamla na LostFocus przy konkretnym TextBox.
            /*
                         <i:Interaction.Triggers>
                <i:EventTrigger EventName="LostFocus">
                    <i:InvokeCommandAction Command="{Binding CategoryNameLostFocusCommand}" />
                </i:EventTrigger>
            </i:Interaction.Triggers>
             */
        }
        
        private void AssignmentNameLostFocus(object obj)
        {
            MessageBox.Show("AssignmentNameLostFocus");

            //To do Xamla na LostFocus przy konkretnym TextBox.
            /*
                         <i:Interaction.Triggers>
                <i:EventTrigger EventName="LostFocus">
                    <i:InvokeCommandAction Command="{Binding AssignmentNameLostFocusCommand}" />
                </i:EventTrigger>
            </i:Interaction.Triggers>
             */
        }

        //Other (built-in or dynamic categories):
        private void MyDayCategoryClicked(object obj)
        {
            if (CurrentCategory is not null) CurrentCategory = null;
            CurrentCategoryName = "Mój dzień";
            _categoryMode = CategoryMode.MyDay;
            AssignmentSteps.Clear();

            var loadedAssignments = _assignmentRepo.GetAssignments(_categoryMode, _currentUser.Id);
            SetAssignmentsCollections(loadedAssignments);
        }

        private void ImportantCategoryClicked(object obj)
        {
            if (CurrentCategory is not null) CurrentCategory = null;
            CurrentCategoryName = "Ważne";
            _categoryMode = CategoryMode.Important;
            AssignmentSteps.Clear();

            var loadedAssignments = _assignmentRepo.GetAssignments(_categoryMode, _currentUser.Id);
            SetAssignmentsCollections(loadedAssignments);
        }

        private void PlannedCategoryClicked(object obj)
        {
            if (CurrentCategory is not null) CurrentCategory = null;
            CurrentCategoryName = "Zaplanowane";
            _categoryMode = CategoryMode.Planned;
            AssignmentSteps.Clear();

            var loadedAssignments = _assignmentRepo.GetAssignments(_categoryMode, _currentUser.Id);
            SetAssignmentsCollections(loadedAssignments);
        }

        private void BuiltInCategoryClicked(object obj)
        {
            CurrentCategory = null;
            CurrentCategory = BuiltInCategory;
            DbCategoryChanged();
        }

        //Automatically set assignments collections:
        private void SetAssignmentsCollections(List<AssignmentDto> loadedAssignments)
        {
            ToDoAssignments = new(loadedAssignments.Where(a => a.IsChecked == false));
            CompletedAssignments = new(loadedAssignments.Where(a => a.IsChecked == true));
        }

        //Log out:
        private void LogOut(object obj)
        {
            _assignmentStepRepo.SaveAssignmentStepsChanges();
            _userContextService.CurrentUser = null;

            NavigationService.NavigateTo<MainMenuViewModel>();
        }
    }
}
