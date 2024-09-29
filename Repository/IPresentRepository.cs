using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models;
namespace Repository;

public interface IPresentRepository
{
    Task<IReadOnlyCollection<Present>>  GetPresentsAsync(string wishlistId);
    Task AddPresentAsync(Present present);
    Task DeletePresentAsync(string presentId);
    Task<IReadOnlyCollection<Present>> SearchPresentsByKeywordAsync(string keyword);
    Task ReservePresentAsync(string presentId, string reserverId);
    Task<IReadOnlyCollection<Present>> GetReservedPresentsAsync(string userId);
}