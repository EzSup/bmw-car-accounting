using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
namespace BMWCarAccountingApp.WPF.Navigation
{
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private Frame _frame;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void SetFrame(Frame frame)
        {
            _frame = frame;
        }

        public void NavigateTo<TPage, TViewModel>() where TPage : Page, new() where TViewModel : class
        {
            var page = new TPage();
            var viewModel = _serviceProvider.GetService<TViewModel>();
            if (viewModel == null)
            {
                throw new InvalidOperationException($"ViewModel of type {typeof(TViewModel).Name} is not registered in DI.");
            }
            page.DataContext = viewModel;
            _frame?.Navigate(page);
        }
    }
}
