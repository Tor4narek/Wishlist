using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models;
namespace Presenter;

public interface IWishlistPresenter
{
    Task<IReadOnlyCollection<Wishlist>> LoadUserWishlistsAsync(string userId);
    Task AddNewWishlistAsync(Wishlist wishlist, CancellationToken token);
    Task DeleteWishlistAsync(Guid wishlistId);
}