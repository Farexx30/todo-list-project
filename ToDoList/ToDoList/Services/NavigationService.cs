using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToDoList.Services
{
    public interface INavigationService
    {
        BaseViewModel? CurrentViewModel { get; }
        void NavigateTo<T>() where T : BaseViewModel;
    }

    public class NavigationService(Func<Type, BaseViewModel> viewModelFactory) : BaseViewModel, INavigationService
    {
         private readonly Func<Type, BaseViewModel> _viewModelFactory = viewModelFactory;
         private BaseViewModel? _currentViewModel;
         public BaseViewModel? CurrentViewModel
         {
             get => _currentViewModel;
             private set
             {
                 _currentViewModel = value;
                 OnPropertyChanged();
             }
         }

         public void NavigateTo<T>() where T : BaseViewModel
         {
             BaseViewModel newViewModel = _viewModelFactory.Invoke(typeof(T));
                
             CurrentViewModel = newViewModel;
         }
    }
}
