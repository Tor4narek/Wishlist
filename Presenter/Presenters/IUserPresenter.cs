using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Presenter;

public interface IUserPresenter
{
    Task<User> LoadUserAsync(string userId);
    Task CreateUserAsync(string name, string email, string password);
    Task DeleteUserAsync(string userId);
    Task<User> AuthenticateUserAsync(string email, string password);
    Task<User> GetAuthenticatedUserAsync();
    Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword);
}