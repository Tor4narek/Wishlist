using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
namespace Repository;

public interface IWishlistRepository
{
    Task<IReadOnlyCollection<Wishlist>> GetUserWishlistsAsync(string userId);
    Task AddWishlistAsync(Wishlist wishlist);
    Task DeleteWishlistAsync(string wishlistId);
    Task UpdateWishlistAsync(Wishlist wishlist);
}