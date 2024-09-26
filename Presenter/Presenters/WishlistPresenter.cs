using Models;

namespace Presenter;

public class WishlistPresenter :IWishlistPresenter
{
    public Task LoadUserWishlistsAsync(string userId)
    {
        throw new NotImplementedException();
    }

    public Task AddNewWishlistAsync(Wishlist wishlist, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task DeleteWishlistAsync(Guid wishlistId)
    {
        throw new NotImplementedException();
    }
}