using MauiApiApp.ViewModels;

namespace MauiApiApp.Views;

public partial class DetailPage : ContentPage
{
    public DetailPage()
    {
        InitializeComponent();
        BindingContext = new DetailViewModel();
    }
}
