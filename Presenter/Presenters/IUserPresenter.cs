using Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Presenter;

public interface IUserPresenter
{
    Task LoadUserAsync(string userId);
    Task CreateUserAsync(string name, string email, string password);
    Task DeleteUserAsync(string userId);
}