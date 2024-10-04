using Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter
{
    public interface IUserPresenter
    {
        Task<User> LoadUserAsync(string userId, CancellationToken token);
        Task CreateUserAsync(string name, string email, string password, CancellationToken token);
        Task DeleteUserAsync(string userId, CancellationToken token);
        Task<User> AuthenticateUserAsync(string email, string password, CancellationToken token);
        Task<User> GetAuthenticatedUserAsync(CancellationToken token);
        Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword, CancellationToken token);
    }
}