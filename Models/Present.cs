using System.Data.Common;

namespace Models;

public record Present(string Id, string Name, string Description, string WishlistId, bool IsReserved, string ReserverId);
