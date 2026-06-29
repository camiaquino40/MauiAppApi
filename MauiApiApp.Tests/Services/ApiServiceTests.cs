using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MauiApiApp.Tests.Models;
using Xunit;

namespace MauiApiApp.Tests.Services
{
    // handler falso para simular respuestas sin hacer llamadas reales
    public class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public FakeHttpMessageHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_response);
    }

    // implementacion concreta del servicio para poder testearla
    public class ConcreteApiService : IApiService
    {
        private readonly HttpClient _http;

        public ConcreteApiService(HttpClient http) => _http = http;

        public async Task<List<Post>> GetPostsAsync()
        {
            var r = await _http.GetAsync("posts");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<List<Post>>() ?? new();
        }

        public async Task<Post?> GetPostByIdAsync(int id)
        {
            var r = await _http.GetAsync($"posts/{id}");
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadFromJsonAsync<Post?>();
        }
    }

    public class ApiServiceTests
    {
        private HttpClient CrearClienteFalso(object contenido)
        {
            var json = JsonSerializer.Serialize(contenido);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
            return new HttpClient(new FakeHttpMessageHandler(response))
            {
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
            };
        }

        [Fact]
        public async Task GetPostsAsync_RetornaListaNoVacia()
        {
            var posts = new List<Post>
            {
                new() { Id = 1, UserId = 1, Title = "primer post", Body = "cuerpo" },
                new() { Id = 2, UserId = 1, Title = "segundo post", Body = "cuerpo2" }
            };

            var service = new ConcreteApiService(CrearClienteFalso(posts));
            var resultado = await service.GetPostsAsync();

            Assert.NotNull(resultado);
            Assert.Equal(2, resultado.Count);
            Assert.Equal("primer post", resultado[0].Title);
        }

        [Fact]
        public async Task GetPostByIdAsync_RetornaPostCorrecto()
        {
            var post = new Post { Id = 5, UserId = 2, Title = "un post", Body = "body" };

            var service = new ConcreteApiService(CrearClienteFalso(post));
            var resultado = await service.GetPostByIdAsync(5);

            Assert.NotNull(resultado);
            Assert.Equal(5, resultado!.Id);
        }

        [Fact]
        public async Task GetPostsAsync_CuandoElServidorFalla_LanzaExcepcion()
        {
            var response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            var http = new HttpClient(new FakeHttpMessageHandler(response))
            {
                BaseAddress = new Uri("https://jsonplaceholder.typicode.com/")
            };
            var service = new ConcreteApiService(http);

            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetPostsAsync());
        }
    }
}
