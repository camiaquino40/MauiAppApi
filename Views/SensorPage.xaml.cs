using MauiApiApp.ViewModels;

namespace MauiApiApp.Views;

public partial class SensorPage : ContentPage
{
    public SensorPage(SensorViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
