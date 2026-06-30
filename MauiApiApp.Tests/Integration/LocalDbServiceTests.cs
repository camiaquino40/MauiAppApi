using MauiApiApp.Tests.Models;
using SQLite;
using Xunit;

namespace MauiApiApp.Tests.Integration
{
    // uso :memory: para no escribir en disco durante los tests
    public class InMemoryLocalDbService
    {
        private readonly SQLiteAsyncConnection _db;

        public InMemoryLocalDbService()
        {
            // nombre único por instancia para evitar que los tests paralelos compartan la db
            var path = $"file:testdb{Guid.NewGuid():N}?mode=memory&cache=shared";
            _db = new SQLiteAsyncConnection(path);
            _db.CreateTableAsync<FavoritePost>().Wait();
        }

        public Task<List<FavoritePost>> GetFavoritesAsync()
            => _db.Table<FavoritePost>().ToListAsync();

        public Task SaveFavoriteAsync(FavoritePost fav)
            => _db.InsertOrReplaceAsync(fav);

        public Task RemoveFavoriteAsync(int postId)
            => _db.DeleteAsync<FavoritePost>(postId);

        public async Task<bool> IsFavoriteAsync(int postId)
        {
            var item = await _db.FindAsync<FavoritePost>(postId);
            return item is not null;
        }
    }

    public class LocalDbServiceTests
    {
        [Fact]
        public async Task GuardarFavorito_DespuesSeEncuentra()
        {
            var db = new InMemoryLocalDbService();
            var fav = new FavoritePost { PostId = 1, Title = "post de prueba", SavedAt = DateTime.Now };

            await db.SaveFavoriteAsync(fav);

            Assert.True(await db.IsFavoriteAsync(1));
        }

        [Fact]
        public async Task EliminarFavorito_YaNoAparece()
        {
            var db = new InMemoryLocalDbService();
            await db.SaveFavoriteAsync(new FavoritePost { PostId = 2, Title = "otro post", SavedAt = DateTime.Now });

            await db.RemoveFavoriteAsync(2);

            Assert.False(await db.IsFavoriteAsync(2));
        }

        [Fact]
        public async Task ListaInicial_EstaVacia()
        {
            var db = new InMemoryLocalDbService();

            var lista = await db.GetFavoritesAsync();

            Assert.Empty(lista);
        }

        [Fact]
        public async Task GuardarVarios_TodosQuedan()
        {
            var db = new InMemoryLocalDbService();

            await db.SaveFavoriteAsync(new FavoritePost { PostId = 10, Title = "uno", SavedAt = DateTime.Now });
            await db.SaveFavoriteAsync(new FavoritePost { PostId = 11, Title = "dos", SavedAt = DateTime.Now });
            await db.SaveFavoriteAsync(new FavoritePost { PostId = 12, Title = "tres", SavedAt = DateTime.Now });

            var lista = await db.GetFavoritesAsync();
            Assert.Equal(3, lista.Count);
        }

        [Fact]
        public async Task GuardarDosVeces_ActualizaEnVezDeduplicar()
        {
            var db = new InMemoryLocalDbService();
            await db.SaveFavoriteAsync(new FavoritePost { PostId = 5, Title = "titulo viejo", SavedAt = DateTime.Now });

            await db.SaveFavoriteAsync(new FavoritePost { PostId = 5, Title = "titulo nuevo", SavedAt = DateTime.Now });

            var lista = await db.GetFavoritesAsync();
            Assert.Single(lista);
            Assert.Equal("titulo nuevo", lista[0].Title);
        }
    }
}
