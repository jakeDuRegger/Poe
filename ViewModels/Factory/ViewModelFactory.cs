using Microsoft.Extensions.DependencyInjection;
using Poe.Helpers.Dependency_Injection;

namespace Poe.ViewModels.Factory;

/// <summary>
/// Stores logic for initializing contractors of view models.
/// </summary>
public static class ViewModelFactory
{
    // Factory method for MainWindowViewModel
    public static MainWindowViewModel CreateMainWindowVM()
    {
        return new MainWindowViewModel();
    }

    // I can add more here.
    public static SearchResultsViewModel CreateSearchResultsVM()
    {
        throw new NotImplementedException();
    }

   
}
