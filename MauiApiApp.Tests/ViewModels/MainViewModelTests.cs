using MauiApiApp.Tests.Models;
using MauiApiApp.Tests.Services;
using Moq;
using Xunit;

namespace MauiApiApp.Tests.ViewModels
{
    public class MainViewModelTests
    {
        private static List<Post> PostsDePrueba() => new()
        {
            new() { Id = 1, UserId = 1, Title = "titulo uno", Body = "body" },
            new() { Id = 2, UserId = 1, Title = "titulo dos", Body = "body2" },
        };

        [Fact]
        public async Task GetPostsAsync_CuandoLaApiResponde_DevuelveLista()
        {
            var mock = new Mock<IApiService>();
            mock.Setup(s => s.GetPostsAsync()).ReturnsAsync(PostsDePrueba());

            var posts = await mock.Object.GetPostsAsync();

            Assert.NotEmpty(posts);
            Assert.Equal(2, posts.Count);
        }

        [Fact]
        public async Task GetPostsAsync_CuandoHayError_LanzaExcepcion()
        {
            var mock = new Mock<IApiService>();
            mock.Setup(s => s.GetPostsAsync())
                .ThrowsAsync(new HttpRequestException("sin conexion"));

            await Assert.ThrowsAsync<HttpRequestException>(
                () => mock.Object.GetPostsAsync());
        }

        [Fact]
        public async Task GetPostByIdAsync_DevuelveElPostCorrecto()
        {
            var mock = new Mock<IApiService>();
            mock.Setup(s => s.GetPostByIdAsync(1))
                .ReturnsAsync(new Post { Id = 1, Title = "titulo uno" });

            var result = await mock.Object.GetPostByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
        }

        [Fact]
        public async Task GetPostByIdAsync_CuandoNoExiste_DevuelveNull()
        {
            var mock = new Mock<IApiService>();
            mock.Setup(s => s.GetPostByIdAsync(999))
                .ReturnsAsync((Post?)null);

            var result = await mock.Object.GetPostByIdAsync(999);

            Assert.Null(result);
        }

        [Fact]
        public void Post_DisplayTitle_TieneElFormatoCorrecto()
        {
            var post = new Post { Id = 3, Title = "hola mundo" };
            Assert.Equal("#3 — hola mundo", post.DisplayTitle);
        }
    }
}
