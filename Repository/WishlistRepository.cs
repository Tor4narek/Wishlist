using Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IReadOnlyCollection<Wishlist>> GetUserWishlistsAsync(string userId)
        {
            var wishlists = await _repository.GetAllAsync();
            return wishlists.Where(w => w.OwnerId == userId).ToList(); 
        }

        // Добавление нового вишлиста
        public async Task AddWishlistAsync(Wishlist wishlist)
        {
            if (wishlist == null)
            {
                throw new ArgumentNullException(nameof(wishlist), "Wishlist cannot be null.");
            }
            await _repository.AddAsync(wishlist);
        }

        // Удаление вишлиста по его ID
        public async Task DeleteWishlistAsync(string wishlistId)
        {
            await _repository.DeleteAsync(w => w.Id == wishlistId); 
        }

        // Обновление существующего вишлиста
        public async Task UpdateWishlistAsync(Wishlist wishlist)
        {
            if (wishlist == null)
            {
                throw new ArgumentNullException(nameof(wishlist), "Wishlist cannot be null.");
            }
            await _repository.UpdateAsync(w => w.Id == wishlist.Id, wishlist);
        }
    }
}