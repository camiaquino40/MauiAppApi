using MauiApiApp.Models;

namespace MauiApiApp.Services
{
    public interface IApiService
    {
        Task<List<Post>> GetPostsAsync();
        Task<Post?> GetPostByIdAsync(int id);
    }
}
