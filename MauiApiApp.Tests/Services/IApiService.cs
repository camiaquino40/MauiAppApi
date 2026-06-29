using MauiApiApp.Tests.Models;

namespace MauiApiApp.Tests.Services
{
    public interface IApiService
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post?> GetPostByIdAsync(int id);
    }
}
