using Models;
using Presenter.Services;
using Repository;
using System;
using System.Threading.Tasks;

namespace Presenter
{
    public class UserPresenter : IUserPresenter
    {
        private  AuthenticationService _authService = new AuthenticationService();
        private UserRepository _userRepository = new UserRepository();

        // Конструктор с внедрением зависимостей (можно использовать DI)
       

        // Метод для создания нового пользователя
        public async Task CreateUserAsync(string name, string email, string password)
        {
            try
            {
                // Вызываем сервис для регистрации нового пользователя
                await _authService.RegisterUserAsync(name, email, password);
                Console.WriteLine("User created successfully!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while creating user: {ex.Message}");
            }
        }

        // Метод для аутентификации пользователя
        public async Task AuthenticateUserAsync(string email, string password)
        {
            try
            {
                await _authService.AuthenticateUserAsync(email, password);
                var user = _authService.GetAuthenticatedUser();
                Console.WriteLine($"User authenticated successfully! Welcome, {user.Name}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while authenticating user: {ex.Message}");
            }
        }

        // Метод для загрузки пользователя по ID
        public async Task LoadUserAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(userId);
                if (user != null)
                {
                    Console.WriteLine($"User loaded: {user.Name}, Email: {user.Email}");
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while loading user: {ex.Message}");
            }
        }
        
        // Метод для удаления пользователя
        public async Task DeleteUserAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(userId);
                if (user != null)
                {
                    await _userRepository.DeleteUserAsync(userId);
                    Console.WriteLine($"User {user.Name} has been deleted.");
                }
                else
                {
                    Console.WriteLine("User not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while deleting user: {ex.Message}");
            }
        }
    }
}
