using ints.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ints.Services
{
    public interface INavigationService
    {
        BaseViewModel CurrentViewModel { get; }

        void NavigateTo(BaseViewModel vm);

        void NavigateTo<T>() where T : BaseViewModel;   // <-- ДОБАВИТЬ

        event Action? CurrentChanged;
    }

    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _sp;

        public NavigationService(IServiceProvider sp)
        {
            _sp = sp;
        }

        private BaseViewModel _current = null!;
        public BaseViewModel CurrentViewModel => _current;

        public event Action? CurrentChanged;

        public void NavigateTo(BaseViewModel vm)
        {
            _current = vm;
            CurrentChanged?.Invoke();
        }

        public void NavigateTo<T>() where T : BaseViewModel
        {
            var vm = App.Services.GetRequiredService<T>();
            NavigateTo(vm);
        }
    }
}
