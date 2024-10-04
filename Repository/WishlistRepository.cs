using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly FileRepository<Wishlist> _repository;

        public WishlistRepository()
        {
            _repository = new FileRepository<Wishlist>("../../data/Wishlists.json", "wishlists");
        }

        // Получение всех вишлистов пользователя по userId
        public async Task<IReadOnlyCollection<Wishlist>> GetUserWishlistsAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();  // Проверка на отмену
            var wishlists = await _repository.GetAllAsync(token); // Передаем токен в репозиторий
            return wishlists.Where(w => w.OwnerId == userId).ToList();
        }

        // Добавление нового вишлиста
        public async Task AddWishlistAsync(Wishlist wishlist, CancellationToken token)
        {
            if (wishlist == null)
            {
                throw new ArgumentNullException(nameof(wishlist), "Wishlist cannot be null.");
            }
            token.ThrowIfCancellationRequested();  // Проверка на отмену
            await _repository.AddAsync(wishlist, token);
        }

        // Удаление вишлиста по его ID
        public async Task DeleteWishlistAsync(string wishlistId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();  // Проверка на отмену
            await _repository.DeleteAsync(w => w.Id == wishlistId, token);
        }

        // Обновление существующего вишлиста
        public async Task UpdateWishlistAsync(Wishlist wishlist, CancellationToken token)
        {
            if (wishlist == null)
            {
                throw new ArgumentNullException(nameof(wishlist), "Wishlist cannot be null.");
            }
            token.ThrowIfCancellationRequested();  // Проверка на отмену
            await _repository.UpdateAsync(w => w.Id == wishlist.Id, wishlist, token);
        }
    }
}
