using System.Net.Http.Json;
using MauiApiApp.Models;

namespace MauiApiApp.Services
{

    public class ApiService : IApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        /// <exception cref="HttpRequestException">si hay problema de conexion o error HTTP</exception>
        public async Task<List<Post>> GetPostsAsync()
        {
            HttpResponseMessage response = await _httpClient.GetAsync("posts");


            response.EnsureSuccessStatusCode();

            List<Post>? posts = await response.Content.ReadFromJsonAsync<List<Post>>();
            return posts ?? new List<Post>();
        }

        public async Task<Post?> GetPostByIdAsync(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"posts/{id}");
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Post>();
        }
    }
}
