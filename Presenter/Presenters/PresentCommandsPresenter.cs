using Models;

namespace Presenter;

public class PresentCommandsPresenter : IPresentCommandsPresenter
{
    public Task AddNewPresentAsync(Present present, CancellationToken token)
    {
        throw new NotImplementedException();
    }

    public Task DeletePresentAsync(Guid presentId)
    {
        throw new NotImplementedException();
    }

    public Task ReservePresentAsync(string presentId, string reserverId)
    {
        throw new NotImplementedException();
    }
}