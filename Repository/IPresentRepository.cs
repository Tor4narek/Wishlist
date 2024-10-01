using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
namespace Repository;

public interface IPresentRepository
{
    Task<IReadOnlyCollection<Present>>  GetPresentsAsync(string wishlistId);
    Task AddPresentAsync(Present present, CancellationToken token);
    Task DeletePresentAsync(Guid presentId);
    Task<IReadOnlyCollection<Present>> SearchPresentsByKeywordAsync(string keyword);
    Task ReservePresentAsync(Guid presentId, string reserverId);
    Task<IReadOnlyCollection<Present>> GetReservedPresentsAsync(string userId);
}