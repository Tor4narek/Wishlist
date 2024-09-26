using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace Presenter.Services;

public interface IAuthenticationService
{
    Task RegisterUserAsync(string name, string email, string password);
    Task AuthenticateUserAsync(string email, string password);
}