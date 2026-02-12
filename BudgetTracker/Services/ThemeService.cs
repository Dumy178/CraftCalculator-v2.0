using System.Windows;

namespace BudgetTracker.Services;

public class ThemeService
{
    public void ToggleTheme(bool useDark)
    {
        var app = Application.Current;
        if (app.Resources.MergedDictionaries.Count == 0) return;

        var source = useDark ? "/Themes/DarkTheme.xaml" : "/Themes/LightTheme.xaml";
        app.Resources.MergedDictionaries[0] = new ResourceDictionary { Source = new Uri(source, UriKind.Relative) };
    }
}
