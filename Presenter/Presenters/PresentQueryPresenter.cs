using Models;
using Repository;

namespace Presenter;

public class PresentQueryPresenter : IPresentQueryPresenter
{
    private PresentRepository _presentRepository;

    public PresentQueryPresenter()
    {
        _presentRepository = new PresentRepository();
    }
    public async Task<IReadOnlyCollection<Present>> LoadWishlistPresentsAsync(string wishlistId)
    {
        if (string.IsNullOrEmpty(wishlistId))
        {
            throw new ArgumentNullException(nameof(wishlistId));
        }
        var presents =  await _presentRepository.GetPresentsAsync(wishlistId);
        return presents;
    }

    public Task SearchPresentsByKeywordAsync(string keyword)
    {
        throw new NotImplementedException();
    }

    public Task LoadReservedPresentsAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
