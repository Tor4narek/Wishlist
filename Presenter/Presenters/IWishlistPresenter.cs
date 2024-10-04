using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

namespace Presenter
{
    public interface IWishlistPresenter
    {
        Task<IReadOnlyCollection<Wishlist>> LoadUserWishlistsAsync(string userId, CancellationToken token);

        Task AddNewWishlistAsync(string w_name, string w_description, string w_ownerId, string w_presentsNumber, CancellationToken token);
        
        Task DeleteWishlistAsync(Guid wishlistId, CancellationToken token);
    }
}