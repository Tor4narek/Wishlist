using Models;
using Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter
{
    public class WishlistPresenter : IWishlistPresenter
    {
        private readonly WishlistRepository _wishlistRepository;

        public WishlistPresenter()
        {
            _wishlistRepository = new WishlistRepository();
        }

        // Метод для загрузки всех вишлистов пользователя
        public async Task<IReadOnlyCollection<Wishlist>> LoadUserWishlistsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            var wishlists = await _wishlistRepository.GetUserWishlistsAsync(userId);
            return wishlists;
        }

        // Метод для добавления нового вишлиста
        public async Task AddNewWishlistAsync(Wishlist wishlist, CancellationToken token)
        {
            if (wishlist == null)
            {
                throw new ArgumentNullException(nameof(wishlist), "Wishlist cannot be null.");
            }

            // Используем токен отмены для управления задачами
            token.ThrowIfCancellationRequested();

            await _wishlistRepository.AddWishlistAsync(wishlist);
        }

        // Метод для удаления вишлиста по его ID
        public async Task DeleteWishlistAsync(Guid wishlistId)
        {
            if (wishlistId == Guid.Empty)
            {
                throw new ArgumentException("Wishlist ID cannot be empty.", nameof(wishlistId));
            }

            await _wishlistRepository.DeleteWishlistAsync(wishlistId.ToString());
        }
    }
}