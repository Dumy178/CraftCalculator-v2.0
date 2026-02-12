using BudgetTracker.Commands;
using BudgetTracker.Services;

namespace BudgetTracker.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ThemeService _themeService = new();

    [ObservableProperty] private bool isDarkMode;

    public RelayCommand ToggleThemeCommand { get; }

    public SettingsViewModel()
    {
        ToggleThemeCommand = new RelayCommand(_ => _themeService.ToggleTheme(IsDarkMode));
    }
}
