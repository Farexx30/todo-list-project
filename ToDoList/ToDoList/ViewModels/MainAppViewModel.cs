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
        private List<AssignmentDto> _toDoAssignments = [];
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

                if(!_isCurrentlySetting) UpdateAssignment(this);
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

                if (!_isCurrentlySetting) UpdateAssignment(this);
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

                if (!_isCurrentlySetting) UpdateAssignment(this);
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
        public ICommand UpdateAssignmentCommand { get; set; }
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
            UpdateAssignmentCommand = new RelayCommand(UpdateAssignment, CanUpadteAssignmentStep);
            DeleteAssignmentCommand = new RelayCommand(DeleteAssignment, CanDeleteAssignment);

            AddAssignmentStepCommand = new RelayCommand(AddAssignmentStep, CanAddAssignmentStep);
            UpdateAssignmentStepCommand = new RelayCommand(UpdateAssignmentStep, CanUpdateAssignment);
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
            CurrentCategory!.Name = CurrentCategoryName;
            bool result = _categoryRepo.UpdateCategory(CurrentCategory, _currentUser.Id);

            if (result)
            {
                CollectionViewSource.GetDefaultView(Categories).Refresh();
            }
        }

        //Delete Category:
        private void DeleteCategory(object obj)
        {
            GetConnectedAssignments(out var connectedAssignmentsIds);
            _categoryRepo.DeleteCategory(CurrentCategory!.Id, connectedAssignmentsIds);

            Categories.Remove(CurrentCategory);
            FilteredToDoAssignments.Clear();
            _toDoAssignments.Clear();
            CompletedAssignments.Clear();
            //i czy CurrentCategory jest nullem juz ??
            CurrentCategoryName = "Mój dzień"; //bez tej linijki wyrzuca błąd przy zamykaniu programu, bo podobno currentcategoryname jest nullem
            MyDayCategoryClicked(obj);
        }

        bool CanDeleteCategory(object obj) => !string.IsNullOrEmpty(CurrentCategoryName)
                                                        && CurrentCategoryName != "Mój dzień"
                                                        && CurrentCategoryName != "Ważne"
                                                        && CurrentCategoryName != "Zaplanowane";
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

        private bool CanAddAssignment(object obj) => !string.IsNullOrEmpty(NewAssignmentName.Trim())
                                                        &&CurrentCategoryName!="Mój dzień"
                                                        &&CurrentCategoryName!="Ważne"
                                                        &&CurrentCategoryName!="Zaplanowane";

        //Update Assignment:
        private void UpdateAssignment(object obj)
        {
            if (CurrentAssignment is not null)
            {
                if (IsDateEnabled)
                {
                    CurrentAssignment!.Deadline = AssignmentDeadline is not null
                        ? AssignmentDeadline.Value
                        : null;
                }
                else
                {
                    CurrentAssignment!.Deadline = null;
                }
                CurrentAssignment.IsChecked = IsAssignmentChecked;
                CurrentAssignment.IsImportant = IsAssignmentImportant;
                CurrentAssignment.IsShared = IsAssignmentShared;

                _assignmentRepo.UpdateAssignment(CurrentAssignment);

                if (CurrentAssignment.IsChecked && !CompletedAssignments.Contains(CurrentAssignment))
                {
                    CompletedAssignments.Add(CurrentAssignment);
                    FilteredToDoAssignments.Remove(CurrentAssignment);
                    _toDoAssignments.Remove(CurrentAssignment);
                }
                else if (!CurrentAssignment.IsChecked && !_toDoAssignments.Contains(CurrentAssignment))
                {
                    FilteredToDoAssignments.Add(CurrentAssignment);
                    _toDoAssignments.Add(CurrentAssignment);
                    CompletedAssignments.Remove(CurrentAssignment);
                }

                CollectionViewSource.GetDefaultView(FilteredToDoAssignments).Refresh();
                CollectionViewSource.GetDefaultView(CompletedAssignments).Refresh();
            }           
        }

        private bool CanUpdateAssignment(object obj) => CurrentAssignment != null
                                                        &&!string.IsNullOrEmpty(CurrentAssignment.Name.Trim());

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
                FilteredToDoAssignments.Remove(CurrentAssignment);
                _toDoAssignments.Remove(CurrentAssignment);
            }
            AssignmentSteps.Clear();
            CurrentAssignment = null;
        }

        private bool CanDeleteAssignment(object obj) => CurrentAssignment != null;


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
        }

        private bool CanAddAssignmentStep(object obj) => CurrentAssignment != null
                                                           && !string.IsNullOrEmpty(NewAssignmentStepName.Trim());

        //Update AssignmentStep:
        private void UpdateAssignmentStep(object obj)
        {
            CurrentAssignmentStep!.IsChecked = IsAssignmentStepChecked;
            _assignmentStepRepo.UpdateAssignmentStep(CurrentAssignmentStep);
        }

        private bool CanUpadteAssignmentStep(object obj) => CurrentAssignment != null
                                                            && CurrentAssignmentStep != null;
        //DeleteAssignmentStep:
        private void DeleteAssignmentStep(object obj)
        {
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

                IsAssignmentChecked = CurrentAssignment.IsChecked;
                IsAssignmentImportant = CurrentAssignment.IsImportant;
                IsAssignmentShared = CurrentAssignment.IsShared;
                AssignmentDeadline = CurrentAssignment.Deadline.HasValue
                    ? DateTime.Parse(CurrentAssignment.Deadline.Value.ToString())
                    : null;

                if (CurrentAssignment.Deadline is not null)
                {
                    IsDateEnabled = true;
                }

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
            UpdateIsEnabledDatePicker();
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

        //LostFocuses:       
        //private void AssignmentNameLostFocus(object obj)
        //{
        //    MessageBox.Show("AssignmentNameLostFocus");

        //    //To do Xamla na LostFocus przy konkretnym TextBox.
        //    /*
        //                 <i:Interaction.Triggers>
        //        <i:EventTrigger EventName="LostFocus">
        //            <i:InvokeCommandAction Command="{Binding AssignmentNameLostFocusCommand}" />
        //        </i:EventTrigger>
        //    </i:Interaction.Triggers>
        //     */
        //}

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

            IsAssignmentChecked = false;
            IsAssignmentImportant = false;
            IsAssignmentShared = false;
            IsDateEnabled = false;
            AssignmentDeadline = null;
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
            IsEnabledSwitch = CurrentAssignment != null && CurrentCategoryName != "Mój dzień" && CurrentCategoryName != "Ważne" && CurrentCategoryName != "Zaplanowane";
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
            IsEnabledSwitchOthers = CurrentAssignment != null && CurrentCategoryName != "Mój dzień" && CurrentCategoryName != "Ważne" && CurrentCategoryName != "Zaplanowane" && CurrentCategoryName != "Pozostałe";
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
                if (_isEnabledDatePicker != value)
                {
                    _isEnabledDatePicker = value;
                    OnPropertyChanged();
                }
            }
        }
        private void UpdateIsEnabledDatePicker()
        {
            IsEnabledDatePicker = CurrentAssignment != null && IsDateEnabled;
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
