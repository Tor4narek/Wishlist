using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly DatabaseRepository<Wishlist> _repository;

        // Конструктор для инициализации WishlistRepository с экземпляром DatabaseRepository
        public WishlistRepository(DatabaseRepository<Wishlist> repository)
        {
            _repository = repository;
        }

        // Конструктор с жестко заданной строкой подключения и именем таблицы
        public WishlistRepository()
        {
            _repository = new DatabaseRepository<Wishlist>(
                "Host=localhost;Port=5432;Username=admin;Password=12345;Database=wishlistdb",
                "Wishlists");
        }

        // Получение всех вишлистов пользователя по userId
        public async Task<IReadOnlyCollection<Wishlist>> GetUserWishlistsAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested(); // Проверка на отмену

            // Запрос для получения всех вишлистов определенного пользователя
            var query = @"
                SELECT Id, Name, Description, OwnerId, PresentsNumber 
                FROM Wishlists 
                WHERE OwnerId = @UserId";

            using var connection = new Npgsql.NpgsqlConnection(_repository._connectionString);
            await connection.OpenAsync(token);

            // Выполнение запроса и возврат списка вишлистов пользователя
            var wishlists = await connection.QueryAsync<Wishlist>(query, new { UserId = userId });
            return wishlists.ToList();
        }

        // Добавление нового вишлиста
        public async Task AddWishlistAsync(Wishlist wishlist, CancellationToken token)
        {
            token.ThrowIfCancellationRequested(); // Проверка на отмену

            if (wishlist == null) throw new ArgumentNullException(nameof(wishlist));

            // Запрос для вставки нового вишлиста
            var query = @"
                INSERT INTO Wishlists (Id, Name, Description, OwnerId, PresentsNumber) 
                VALUES (@Id, @Name, @Description, @OwnerId, @PresentsNumber)";

            using var connection = new Npgsql.NpgsqlConnection(_repository._connectionString);
            await connection.OpenAsync(token);

            // Выполнение запроса на добавление
            await connection.ExecuteAsync(query, wishlist);
        }

        // Удаление вишлиста по его ID
        public async Task DeleteWishlistAsync(string wishlistId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested(); // Проверка на отмену

            if (string.IsNullOrWhiteSpace(wishlistId))
                throw new ArgumentException("WishlistId cannot be null or empty.");

            // Запрос на удаление вишлиста по ID
            var query = "DELETE FROM Wishlists WHERE Id = @WishlistId";

            using var connection = new Npgsql.NpgsqlConnection(_repository._connectionString);
            await connection.OpenAsync(token);

            // Выполнение запроса на удаление
            await connection.ExecuteAsync(query, new { WishlistId = wishlistId });
        }

        // Обновление существующего вишлиста
        public async Task UpdateWishlistAsync(Wishlist wishlist, CancellationToken token)
        {
            token.ThrowIfCancellationRequested(); // Проверка на отмену

            if (wishlist == null) throw new ArgumentNullException(nameof(wishlist));

            // Запрос для обновления вишлиста
            var query = @"
                UPDATE Wishlists 
                SET Name = @Name, Description = @Description, PresentsNumber = @PresentsNumber 
                WHERE Id = @Id";

            using var connection = new Npgsql.NpgsqlConnection(_repository._connectionString);
            await connection.OpenAsync(token);

            // Выполнение запроса на обновление
            await connection.ExecuteAsync(query, wishlist);
        }
    }
}
