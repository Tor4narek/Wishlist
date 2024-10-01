using System.Data.Common;

namespace Models;

public record Present(Guid Id, string Name, string Description, string WishlistId, bool IsReserved, string ReserverId);
