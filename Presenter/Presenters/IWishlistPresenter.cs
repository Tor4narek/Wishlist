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

    Task AddNewWishlistAsync(string w_name, string w_description, string w_ownerId, string w_presentsNumber);
    Task DeleteWishlistAsync(Guid wishlistId);
}