using System;
using System.Threading.Tasks;
using Models;
using Repository;
using Presenter;
using Presenter.Services;
using View;
namespace Whishlist
{


    public class Program
    {
        public static async Task Main(string[] args)
        {
            var authService = new AuthenticationService();
            try
            {
                await authService.RegisterUserAsync("Egor", "johnexample.com", "password123");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                
            }
            // Регистрация пользователя
              

            // Аутентификация пользователя
            try
            {
                await authService.AuthenticateUserAsync("john@example.com", "password123");
                var authenticatedUser = authService.GetAuthenticatedUser();
                Console.WriteLine("Authenticated as: " + authenticatedUser.Name);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            // Дальнейшие действия с аутентифицированным пользователем
        }
    }

}