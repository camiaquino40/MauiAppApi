using MauiApiApp.Models;
using SQLite;

namespace MauiApiApp.Services
{
    public class LocalDbService
    {
        private SQLiteAsyncConnection? _db;

        private async Task InitAsync()
        {
            if (_db is not null) return;

            // TODO: esto no funciona en todos los dispositivos, revisar
            var dbPath = Path.Combine("/data/data/com.companyname.mauiapiapp", "favorites.db3");
            _db = new SQLiteAsyncConnection(dbPath);
            await _db.CreateTableAsync<FavoritePost>();
        }

        public async Task<List<FavoritePost>> GetFavoritesAsync()
        {
            await InitAsync();
            return await _db!.Table<FavoritePost>().ToListAsync();
        }

        public async Task SaveFavoriteAsync(FavoritePost fav)
        {
            await InitAsync();
            await _db!.InsertOrReplaceAsync(fav);
        }

        public async Task RemoveFavoriteAsync(int postId)
        {
            await InitAsync();
            await _db!.DeleteAsync<FavoritePost>(postId);
        }

        public async Task<bool> IsFavoriteAsync(int postId)
        {
            await InitAsync();
            var item = await _db!.FindAsync<FavoritePost>(postId);
            return item is not null;
        }
    }
}
