using Models;
using Repository;

namespace Presenter;

public class PresentCommandsPresenter : IPresentCommandsPresenter
{
    PresentRepository _presentRepository;

    public PresentCommandsPresenter()
    {
        _presentRepository = new PresentRepository();
    }
    public async Task AddNewPresentAsync(string Name, string Description, string ReserverId,string WishlistId, CancellationToken token)
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            throw new ArgumentNullException(nameof(Name));
        }

        if (string.IsNullOrWhiteSpace(Description))
        {
            throw new ArgumentNullException(nameof(Description));
        }

        if (string.IsNullOrWhiteSpace(ReserverId))
        {
            throw new ArgumentNullException(nameof(ReserverId));
        }

        if (string.IsNullOrWhiteSpace(WishlistId))
        {
            throw new ArgumentNullException(nameof(WishlistId));
        }
        Guid id = Guid.NewGuid();
        Present present = new Present(id, Name, Description,WishlistId ,false, ReserverId);
        token.ThrowIfCancellationRequested();
        await _presentRepository.AddPresentAsync(present, token);
        
    }

    public Task DeletePresentAsync(Guid presentId)
    {
        throw new NotImplementedException();
    }

    public Task ReservePresentAsync(Guid presentId, string reserverId)
    {
        throw new NotImplementedException();
    }
}