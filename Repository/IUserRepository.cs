namespace Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

public interface IUserRepository
{
    Task<User> GetUserAsync(string userId);
    Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword);
    Task AddUserAsync(User user);
    Task<User> GetUserByEmailAsync(string email);
    Task DeleteUserAsync(string userId);
    Task UpdateUserAsync(User user);
}
