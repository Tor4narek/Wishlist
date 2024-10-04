using Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter
{
    public interface IPresentCommandsPresenter
    {
        Task AddNewPresentAsync(string name, string description, string reserverId, string wishlistId, CancellationToken token);
        Task DeletePresentAsync(Guid presentId, CancellationToken token);
        Task ReservePresentAsync(Guid presentId, string reserverId, CancellationToken token);
    }
}