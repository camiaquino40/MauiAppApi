using MauiApiApp.Views;

namespace MauiApiApp;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("DetailPage", typeof(DetailPage));
        Routing.RegisterRoute("SensorPage", typeof(SensorPage));
    }
}
