using MauiApiApp.Services;
using MauiApiApp.ViewModels;
using MauiApiApp.Views;

namespace MauiApiApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/");
            client.Timeout = TimeSpan.FromSeconds(10);
        });

        builder.Services.AddTransient<MainViewModel>();
        builder.Services.AddTransient<DetailViewModel>();

        builder.Services.AddTransient<MainPage>();
        builder.Services.AddTransient<DetailPage>();

        return builder.Build();
    }
}
