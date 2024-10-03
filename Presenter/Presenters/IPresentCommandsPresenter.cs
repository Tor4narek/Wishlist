using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Presenter;

public interface IPresentCommandsPresenter
{
    Task AddNewPresentAsync(string Name, string Description, string ReserverId, string WishlistId,
        CancellationToken token);
    Task DeletePresentAsync(Guid presentId);
    Task ReservePresentAsync(Guid presentId, string reserverId);
}
