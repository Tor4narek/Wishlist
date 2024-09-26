using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Presenter;

public interface IPresentCommandsPresenter
{
    Task AddNewPresentAsync(Present present, CancellationToken token);
    Task DeletePresentAsync(Guid presentId);
    Task ReservePresentAsync(string presentId, string reserverId);
}
