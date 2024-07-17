using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
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
        private readonly List<AssignmentDto> _toDoAssignments = [];
        private CategoryMode _categoryMode = CategoryMode.MyDay;
        AssignmentDto? _clickedAssignment = null;
        bool _isCurrentlySetting = false;


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

        //Search phrase:
        private string _searchPhrase = string.Empty;
        public string SearchPhrase
        {
            get => _searchPhrase;
            set
            {
                _searchPhrase = value;
                OnPropertyChanged();

                SearchPhraseChanged();
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

        private string _currentAssignmentName = string.Empty;
        public string CurrentAssignmentName
        {
            get => _currentAssignmentName;
            set
            {
                _currentAssignmentName = value;
                OnPropertyChanged();
            }
        }

        private bool _isAssignmentNameEnabled;
        public bool IsAssignmentNameEnabled
        {
            get => _isAssignmentNameEnabled;
            set
            {
                _isAssignmentNameEnabled = value;
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

                if (!_isCurrentlySetting)
                {
                    UpdateAssignmentCheck();
                }
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

                if (!_isCurrentlySetting)
                {
                    UpdateAssignmentImportance();
                }
            }
        }

        private bool _isAssignmentShared;
        public bool IsAssignmentShared
        {
            get => _isAssignmentShared;
            set
            {
                _isAssignmentShared = value;
                OnPropertyChanged();

                if (!_isCurrentlySetting)
                {
                    UpdateAssignmentSharing();
                }
            }
        }

        private bool _isDateEnabled;
        public bool IsDateEnabled
        {
            get => _isDateEnabled;
            set
            {
                _isDateEnabled = value;
                OnPropertyChanged();

                AssignmentDeadlineChanged();

                if (!value && !_isCurrentlySetting)
                {
                    UpdateAssignmentDeadline(this);
                }

                UpdateIsEnabledDatePicker();
            }
        }

        private ObservableCollection<AssignmentDto> _filteredToDoAssignments = [];
        public ObservableCollection<AssignmentDto> FilteredToDoAssignments
        {
            get => _filteredToDoAssignments;
            set
            {
                _filteredToDoAssignments = value;
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
                IsAssignmentNameEnabledChanged();
            }
        }


        //AssignmentSteps bindings:
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

                if(!_isCurrentlySetting) UpdateAssignmentStep(this);
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
                AssignmentStepChanged();
            }
        }


        //Commands:
        public ICommand AddCategoryCommand { get; set; }
        public ICommand UpdateCategoryCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }
        public ICommand AddAssignmentCommand { get; set; }
        public ICommand UpdateAssignmentNameCommand { get; set; }
        public ICommand UpdateAssignmentDeadlineCommand { get; set; }
        public ICommand DeleteAssignmentCommand { get; set; }
        public ICommand AddAssignmentStepCommand { get; set; }
        public ICommand UpdateAssignmentStepCommand { get; set; }
        public ICommand DeleteAssignmentStepCommand { get; set; }
        public ICommand MyDayCategoryClickedCommand { get; set; }
        public ICommand ImportantCategoryClickedCommand { get; set; }
        public ICommand PlannedCategoryClickedCommand { get; set; }
        public ICommand BuiltInCategoryClickedCommand { get; set; }
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
            DeleteCategoryCommand = new RelayCommand(DeleteCategory, CanDeleteCategory);

            AddAssignmentCommand = new RelayCommand(AddAssignment, CanAddAssignment);
            UpdateAssignmentNameCommand = new RelayCommand(UpdateAssignmentName, CanUpdateAssignmentName);
            UpdateAssignmentDeadlineCommand = new RelayCommand(UpdateAssignmentDeadline, _ => true);
            DeleteAssignmentCommand = new RelayCommand(DeleteAssignment, CanDeleteAssignment);

            AddAssignmentStepCommand = new RelayCommand(AddAssignmentStep, CanAddAssignmentStep);
            UpdateAssignmentStepCommand = new RelayCommand(UpdateAssignmentStep, CanUpdateAssignmentStep);
            DeleteAssignmentStepCommand = new RelayCommand(DeleteAssignmentStep, CanDeleteAssignmentStep);

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
            var originalCategoryName = CurrentCategory!.Name;
            CurrentCategory.Name = CurrentCategoryName;
            var updateResult = _categoryRepo.UpdateCategory(CurrentCategory, _currentUser.Id);

            if (updateResult == UpdatingCategoryResult.Success)
            {
                CollectionViewSource.GetDefaultView(Categories).Refresh();
            }
            else
            {
                MessageBox.Show("Taka kategoria już istnieje");
                CurrentCategory.Name = originalCategoryName;
                CurrentCategoryName = originalCategoryName;
            }
        }

        //Delete Category:
        private void DeleteCategory(object obj)
        {
            GetConnectedAssignments(out var connectedAssignmentsIds);
            _categoryRepo.DeleteCategory(CurrentCategory!.Id, connectedAssignmentsIds);

            _toDoAssignments.Clear();
            CompletedAssignments.Clear();
            Categories.Remove(CurrentCategory);

            SearchPhrase = string.Empty;
            SearchPhraseChanged();

            MyDayCategoryClicked(this);
        }
        private List<int> GetConnectedAssignments(out List<int> connectedAssignmentsIds)
        {
            var connectedAssignments = new List<AssignmentDto>();
            connectedAssignments.AddRange(_toDoAssignments);
            connectedAssignments.AddRange(CompletedAssignments);
            connectedAssignmentsIds = connectedAssignments
                .Select(a => a.Id)
                .ToList();

            return connectedAssignmentsIds;
        }

        bool CanDeleteCategory(object obj) 
            => CurrentCategory != null 
                && CurrentCategory.Id != 1;



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

            _toDoAssignments.Add(newAssignmentWithIdDto);

            SearchPhraseChanged();

            CurrentAssignment = newAssignmentWithIdDto;
            NewAssignmentName = string.Empty;
        }

        private bool CanAddAssignment(object obj) 
            => !string.IsNullOrEmpty(NewAssignmentName.Trim())
                    && CurrentCategory != null;

        //Update Assignment:
        private void UpdateAssignmentName(object obj)
        {
            _assignmentRepo.UpdateAssignmentName(CurrentAssignmentName, CurrentAssignment!.Id);
            CurrentAssignment.Name = CurrentAssignmentName;           

            CollectionViewSource.GetDefaultView(FilteredToDoAssignments).Refresh();
            CollectionViewSource.GetDefaultView(CompletedAssignments).Refresh();
        }
        private bool CanUpdateAssignmentName(object obj)
        {
            bool result = !string.IsNullOrEmpty(CurrentAssignmentName.Trim());

            if (!result)
            {
                CurrentAssignmentName = CurrentAssignment!.Name;
            }

            return result;
        }

        private void UpdateAssignmentDeadline(object obj)
        {
            _assignmentRepo.UpdateAssignmentDeadline(AssignmentDeadline, CurrentAssignment!.Id);
            CurrentAssignment.Deadline = AssignmentDeadline;
        }

        private void UpdateAssignmentCheck()
        {
            _assignmentRepo.UpdateAssignmentCheck(IsAssignmentChecked, CurrentAssignment!.Id);
            CurrentAssignment.IsChecked = IsAssignmentChecked;

            if (IsAssignmentChecked)
            {
                CompletedAssignments.Add(CurrentAssignment);
                _toDoAssignments.Remove(CurrentAssignment);
            }
            else
            {
                _toDoAssignments.Add(CurrentAssignment);
                CompletedAssignments.Remove(CurrentAssignment);
            }
            SearchPhraseChanged();
            SetDefaultAssignmentsValues();
            AssignmentSteps.Clear();
        }

        private void UpdateAssignmentImportance()
        {
            _assignmentRepo.UpdateAssignmentImportance(IsAssignmentImportant, CurrentAssignment!.Id);
            CurrentAssignment.IsImportant = IsAssignmentImportant;
        }

        private void UpdateAssignmentSharing()
        {
            _assignmentRepo.UpdateAssignmentSharing(IsAssignmentShared, CurrentAssignment!.Id);
            CurrentAssignment.IsShared = IsAssignmentShared;
        }

        private void AssignmentDeadlineChanged()
            => AssignmentDeadline = _isDateEnabled
                    ? AssignmentDeadline
                    : null;

        private void IsAssignmentNameEnabledChanged() 
            => IsAssignmentNameEnabled = CurrentAssignment is not null;

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
                _toDoAssignments.Remove(CurrentAssignment);
                SearchPhraseChanged();
            }
            AssignmentSteps.Clear();
            SetDefaultAssignmentsValues();
            CurrentAssignment = null;
        }

        private bool CanDeleteAssignment(object obj) 
            => CurrentAssignment != null;


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
            CurrentAssignmentStep = assignmentStepWithIdDto;
            NewAssignmentStepName = string.Empty;
        }

        private bool CanAddAssignmentStep(object obj) 
            => CurrentAssignment != null
               && !string.IsNullOrEmpty(NewAssignmentStepName.Trim());

        //Update AssignmentStep:
        private void UpdateAssignmentStep(object obj)
        {
            CurrentAssignmentStep!.IsChecked = IsAssignmentStepChecked;
            _assignmentStepRepo.UpdateAssignmentStep(CurrentAssignmentStep);
        }

        private bool CanUpdateAssignmentStep(object obj) => CurrentAssignment != null
                                                            && CurrentAssignmentStep != null;
        //DeleteAssignmentStep:
        private void DeleteAssignmentStep(object obj)
        {
            IsAssignmentStepChecked = false;

            _assignmentStepRepo.DeleteAssignmentStep(CurrentAssignmentStep!.Id);
            AssignmentSteps.Remove(CurrentAssignmentStep);

            CurrentAssignmentStep = null;
        }

        private bool CanDeleteAssignmentStep(object obj) => CurrentAssignment != null
                                                            && CurrentAssignmentStep != null;


        //Category changed:
        private void DbCategoryChanged()
        {
            if (CurrentCategory is not null)
            {
                CurrentCategoryName = CurrentCategory.Name;
                _categoryMode = CategoryMode.Database;
                AssignmentSteps.Clear();
                SetDefaultAssignmentsValues();

                var loadedAssignments = _assignmentRepo.GetAssignments(_categoryMode, _currentUser.Id, CurrentCategory.Id);
                SetAssignmentsCollections(loadedAssignments);
            }
            UpdateIsEnabledCategoryName();
        }

        //AssignmentChanged:
        private void AssignmentChanged()
        {
            if (CurrentAssignment is not null)
            {
                if(_clickedAssignment is null)
                {
                    _clickedAssignment = CurrentAssignment;
                    CurrentAssignment = null;
                }

                AssignmentSteps = new(_assignmentStepRepo.GetAssignmentSteps(CurrentAssignment!.Id));

                _isCurrentlySetting = true;

                CurrentAssignmentName = CurrentAssignment.Name;
                IsAssignmentChecked = CurrentAssignment.IsChecked;
                IsAssignmentImportant = CurrentAssignment.IsImportant;
                IsAssignmentShared = CurrentAssignment.IsShared;
                AssignmentDeadline = CurrentAssignment.Deadline.HasValue
                    ? DateTime.Parse(CurrentAssignment.Deadline.Value.ToString())
                    : null;

                IsDateEnabled = CurrentAssignment.Deadline is not null;

                IsAssignmentStepChecked = false;

                _isCurrentlySetting = false;
                _clickedAssignment = null;
            }
            else if(_clickedAssignment is not null)
            {
                CurrentAssignment = _clickedAssignment;
            }


            UpdateIsEnabledSwitch();
            UpdateIsEnabledCheckBox();
            UpdateIsEnabledSwitchOthers();
        }

        private void AssignmentStepChanged()
        {
            if (CurrentAssignmentStep is not null)
            {
                _isCurrentlySetting = true;

                IsAssignmentStepChecked = CurrentAssignmentStep.IsChecked;

                _isCurrentlySetting = false;
            }

            UpdateIsEnabledCheckBoxStep();
        }


        //Other (built-in or dynamic categories):
        private void MyDayCategoryClicked(object obj)
        {
            if (CurrentCategory is not null) CurrentCategory = null;
            CurrentCategoryName = "Mój dzień";
            _categoryMode = CategoryMode.MyDay;
            AssignmentSteps.Clear();
            SetDefaultAssignmentsValues();

            var loadedAssignments = _assignmentRepo.GetAssignments(_categoryMode, _currentUser.Id);
            SetAssignmentsCollections(loadedAssignments);
            UpdateIsEnabledCategoryName();  
        }

        private void ImportantCategoryClicked(object obj)
        {
            if (CurrentCategory is not null) CurrentCategory = null;
            CurrentCategoryName = "Ważne";
            _categoryMode = CategoryMode.Important;
            AssignmentSteps.Clear();
            SetDefaultAssignmentsValues();

            var loadedAssignments = _assignmentRepo.GetAssignments(_categoryMode, _currentUser.Id);
            SetAssignmentsCollections(loadedAssignments);
            UpdateIsEnabledCategoryName();
        }

        private void PlannedCategoryClicked(object obj)
        {
            if (CurrentCategory is not null) CurrentCategory = null;
            CurrentCategoryName = "Zaplanowane";
            _categoryMode = CategoryMode.Planned;
            AssignmentSteps.Clear();
            SetDefaultAssignmentsValues();

            var loadedAssignments = _assignmentRepo.GetAssignments(_categoryMode, _currentUser.Id);
            SetAssignmentsCollections(loadedAssignments);
            UpdateIsEnabledCategoryName();
        }

        private void BuiltInCategoryClicked(object obj)
        {
            CurrentCategory = null;
            CurrentCategory = BuiltInCategory;
            DbCategoryChanged();
            UpdateIsEnabledCategoryName();
        }

        //Automatically set assignments collections:
        private void SetAssignmentsCollections(List<AssignmentDto> loadedAssignments)
        {
            _toDoAssignments.Clear();
            _toDoAssignments.AddRange(loadedAssignments.Where(a => a.IsChecked == false));
            SearchPhraseChanged();
            CompletedAssignments = new(loadedAssignments.Where(a => a.IsChecked == true));
        }

        private void SetDefaultAssignmentsValues()
        {
            _isCurrentlySetting = true;

            CurrentAssignmentName = string.Empty;
            IsAssignmentChecked = false;
            IsAssignmentImportant = false;
            IsAssignmentShared = false;
            IsDateEnabled = false;
            IsAssignmentStepChecked = false;
            SearchPhrase = string.Empty;

            _isCurrentlySetting = false;
        }


        //Search phrase:
        private void SearchPhraseChanged()
        {
            FilteredToDoAssignments = new(_toDoAssignments
                .Where(a => a.Name.ToLower().Trim().Contains(SearchPhrase.ToLower().Trim())));
        }

        //Log out:
        private void LogOut(object obj)
        {
            _userContextService.CurrentUser = null;

            NavigationService.NavigateTo<MainMenuViewModel>();
        }
        
        private bool _isEnabledSwitch;
        public bool IsEnabledSwitch
        {
            get => _isEnabledSwitch;
            set
            {
                if (_isEnabledSwitch != value)
                {
                    _isEnabledSwitch = value;
                    OnPropertyChanged();
                }
            }
        }
        private void UpdateIsEnabledSwitch()
        {
            IsEnabledSwitch = CurrentAssignment != null && CurrentCategory != null;
        }      
        
        private bool _isEnabledSwitchOthers;
        public bool IsEnabledSwitchOthers
        {
            get => _isEnabledSwitchOthers;
            set
            {
                if (_isEnabledSwitchOthers != value)
                {
                    _isEnabledSwitchOthers = value;
                    OnPropertyChanged();
                }
            }
        }
        private void UpdateIsEnabledSwitchOthers()
        {
            IsEnabledSwitchOthers = CurrentAssignment is not null && CurrentCategory is not null && CurrentCategoryName != "Pozostałe";
        }

        private bool _isEnabledCheckBox;
        public bool IsEnabledCheckBox
        {
            get => _isEnabledCheckBox;
            set
            {
                if (_isEnabledCheckBox != value)
                {
                    _isEnabledCheckBox = value;
                    OnPropertyChanged();
                }
            }
        }
        private void UpdateIsEnabledCheckBox()
        {
            IsEnabledCheckBox = CurrentAssignment != null;
        }

        private bool _isEnabledCheckBoxStep;
        public bool IsEnabledCheckBoxStep
        {
            get => _isEnabledCheckBoxStep;
            set
            {
                if (_isEnabledCheckBoxStep != value)
                {
                    _isEnabledCheckBoxStep = value;
                    OnPropertyChanged();
                }
            }
        }
        private void UpdateIsEnabledCheckBoxStep()
        {
            IsEnabledCheckBoxStep = CurrentAssignment != null && CurrentAssignmentStep !=null;
        }


        private bool _isEnabledDatePicker;
        public bool IsEnabledDatePicker
        {
            get => _isEnabledDatePicker;
            set
            {
                _isEnabledDatePicker = value;
                OnPropertyChanged();
            }
        }

        private void UpdateIsEnabledDatePicker()
        {
            IsEnabledDatePicker = IsDateEnabled && CurrentCategory != null;
        }


        private bool _isEnabledCategoryName;
        public bool IsEnabledCategoryName
        { 
            get => _isEnabledCategoryName;
            set
            {
                if (_isEnabledCategoryName != value)
                {
                    _isEnabledCategoryName = value;
                    OnPropertyChanged();
                }
            }
        }
        private void UpdateIsEnabledCategoryName()
        {
            IsEnabledCategoryName = CurrentCategoryName != "Mój dzień" && CurrentCategoryName != "Ważne" && CurrentCategoryName != "Zaplanowane" && CurrentCategoryName != "Pozostałe";
        }

    }
}
