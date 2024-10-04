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
        private readonly UserPresenter _userPresenter;

        public WishlistPresenter()
        {
            _wishlistRepository = new WishlistRepository();
            _userPresenter = new UserPresenter();
        }

        // Метод для загрузки всех вишлистов пользователя
        public async Task<IReadOnlyCollection<Wishlist>> LoadUserWishlistsAsync(string userId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));
            }

            // Проверка отмены
            token.ThrowIfCancellationRequested();

            User user = await _userPresenter.LoadUserAsync(userId, token);
            if (user == null)
            {
                throw new ArgumentException("User does not exist.", nameof(userId));
            }

            var wishlists = await _wishlistRepository.GetUserWishlistsAsync(userId,token);
            return wishlists;
        }

        // Метод для добавления нового вишлиста


        // public Task<IReadOnlyCollection<Wishlist>> LoadUserWishlistsAsync(string userId)
        // {
        //     throw new NotImplementedException();
        // }

        public async Task AddNewWishlistAsync(string w_name, string w_description, string w_ownerId, string w_presentsNumber, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(w_name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(w_name));
            }

            if (string.IsNullOrWhiteSpace(w_description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(w_description));
            }

            if (string.IsNullOrWhiteSpace(w_ownerId))
            {
                throw new ArgumentException("OwnerId cannot be null or empty.", nameof(w_ownerId));
            }

            if (string.IsNullOrWhiteSpace(w_presentsNumber))
            {
                throw new ArgumentException("Presents number cannot be null or empty.", nameof(w_presentsNumber));
            }

            string w_id = Guid.NewGuid().ToString();
            var wishlist = new Wishlist(w_id, w_name, w_description, w_ownerId, w_presentsNumber);
            token.ThrowIfCancellationRequested();
            await _wishlistRepository.AddWishlistAsync(wishlist,token);
        }

        // Метод для удаления вишлиста по его ID
        public async Task DeleteWishlistAsync(Guid wishlistId, CancellationToken token)
        {
            if (wishlistId == Guid.Empty)
            {
                throw new ArgumentException("Wishlist ID cannot be empty.", nameof(wishlistId));
            }

            token.ThrowIfCancellationRequested();
            await _wishlistRepository.DeleteWishlistAsync(wishlistId.ToString(), token);
        }
    }
}
