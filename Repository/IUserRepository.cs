namespace Repository;
using System.Collections.Generic;
using System.Threading.Tasks;
using Models;

public interface IUserRepository
{
    Task<User> GetUserAsync(string userId);
    Task<User> GetUserByEmailAsync(string email); 
    Task AddUserAsync(User user);
    Task DeleteUserAsync(string userId);
    Task UpdateUserAsync(User user);
}
