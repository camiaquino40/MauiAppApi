using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApiApp.Models;
using MauiApiApp.Services;

namespace MauiApiApp.ViewModels
{

    public partial class MainViewModel : ObservableObject
    {
        // ─── servicio ──────────────────────────────────────────────────────────
        private readonly IApiService _apiService;

        public MainViewModel(IApiService apiService)
        {
            _apiService = apiService;
        }

        // ─── propiedades observables ───────────────────────────────────────────

        [ObservableProperty]
        private ObservableCollection<Post> posts = new();

        [ObservableProperty]
        private string statusMessage = "Presioná 'Cargar' para obtener los posts.";

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private Color statusColor = Colors.Gray;

        // ─── comando ───────────────────────────────────────────────────────────

        [RelayCommand]
        private async Task LoadPostsAsync()
        {
            IsLoading     = true;
            StatusMessage = "Cargando posts...";
            StatusColor   = Colors.Gray;
            Posts.Clear();

            try
            {
                List<Post> result = await _apiService.GetPostsAsync();

                foreach (Post post in result)
                    Posts.Add(post);

                StatusMessage = $"✔ {Posts.Count} posts cargados correctamente.";
                StatusColor   = Color.FromArgb("#27ae60"); // verde
            }
            catch (HttpRequestException ex) when (ex.StatusCode.HasValue)
            {
                // error HTTP
                int code = (int)ex.StatusCode!.Value;
                StatusMessage = code switch
                {
                    400 => "⚠ Error 400: solicitud incorrecta.",
                    401 => "⚠ Error 401: no autorizado.",
                    403 => "⚠ Error 403: acceso prohibido.",
                    404 => "⚠ Error 404: recurso no encontrado.",
                    >= 500 => $"⚠ Error {code}: falla en el servidor.",
                    _       => $"⚠ Error HTTP {code}."
                };
                StatusColor = Color.FromArgb("#c0392b"); // rojo
            }
            catch (HttpRequestException)
            {
                StatusMessage = "⚠ Sin conexión. Verificá tu red e intentá de nuevo.";
                StatusColor   = Color.FromArgb("#c0392b");
            }
            catch (TaskCanceledException)
            {
                StatusMessage = "⚠ Tiempo de espera agotado. La API no respondió.";
                StatusColor   = Color.FromArgb("#c0392b");
            }
            catch (JsonException)
            {

                StatusMessage = "⚠ Error al procesar la respuesta del servidor.";
                StatusColor   = Color.FromArgb("#c0392b");
            }
            catch (Exception ex)
            {

                StatusMessage = $"⚠ Error inesperado: {ex.Message}";
                StatusColor   = Color.FromArgb("#c0392b");
            }
            finally
            {

                IsLoading = false;
            }
        }

        // ─── navegacion ────────────────────────────────────────────────────────

        [RelayCommand]
        private async Task GoToDetailAsync(Post post)
        {
            if (post is null) return;

            await Shell.Current.GoToAsync(
                $"DetailPage?postId={post.Id}");
        }
    }
}
