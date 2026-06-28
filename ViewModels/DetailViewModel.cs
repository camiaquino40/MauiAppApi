using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApiApp.Models;
using MauiApiApp.Services;

namespace MauiApiApp.ViewModels
{

    [QueryProperty(nameof(PostId), "postId")]
    public partial class DetailViewModel : ObservableObject
    {
        private readonly IApiService _apiService;

        public DetailViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        // ─── propiedades observables ───────────────────────────────────────────


        [ObservableProperty]
        private int postId;


        [ObservableProperty]
        private Post? post;

        [ObservableProperty]
        private string statusMessage = "Cargando detalle...";

        [ObservableProperty]
        private bool isLoading;

        [RelayCommand]
        private async Task GoBack()
        {
            await Shell.Current.GoToAsync("..");
        }

        // ─── reaccion a PostId ──────────────────────────────────────

        partial void OnPostIdChanged(int value)
        {

            _ = LoadPostAsync(value);
        }

        // ─── logica privada ────────────────────────────────────────────────────

        private async Task LoadPostAsync(int id)
        {
            IsLoading     = true;
            StatusMessage = "Cargando detalle...";
            Post          = null;

            try
            {
                Post = await _apiService.GetPostByIdAsync(id);

                StatusMessage = Post is not null
                    ? string.Empty
                    : "⚠ No se encontró el post.";
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                int code = (int)ex.StatusCode!.Value;
                StatusMessage = $"⚠ Error HTTP {code} al cargar el detalle.";
            }
            catch (HttpRequestException)
            {
                StatusMessage = "⚠ Sin conexión. Verificá tu red.";
            }
            catch (Exception ex)
            {
                StatusMessage = $"⚠ Error inesperado: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
