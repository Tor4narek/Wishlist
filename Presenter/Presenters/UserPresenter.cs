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
        // Метод для создания нового пользователя
        public async Task CreateUserAsync(string name, string email, string password)
        {
            try
            {
                // Вызываем сервис для регистрации нового пользователя
                await _authService.RegisterUserAsync(name, email, password);
            }
            catch (Exception ex)
            {
               throw new Exception($"Error while creating user: {ex.Message}");
            }
        }

        // Метод для аутентификации пользователя
        public async Task<User> AuthenticateUserAsync(string email, string password)
        {
            try
            {
                await _authService.AuthenticateUserAsync(email, password);
                var user = await _authService.GetAuthenticatedUserAsync();
                return user;
            }
            catch (Exception ex)
            {
               throw new Exception($"Error while authenticating user: {ex.Message}");
            }
        }

        // Метод для загрузки пользователя по ID
        public async Task<User> LoadUserAsync(string userId)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(userId);
                if (user != null)
                {
                    return user;
                }
                else
                {
                     throw new Exception("User not found.");
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while loading user: {ex.Message}");
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
                }
                else
                {
                   throw new Exception("User not found.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while deleting user: {ex.Message}");
            }
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
            try
            {
               var user = await _authService.GetAuthenticatedUserAsync();
               return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error receiving the user: {ex.Message}");
            }
        }

        public async Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword)
        {
            // Проводим проверку входного параметра
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return null; // Возвращаем пустой список, если ключевое слово пустое
            }

            // Получаем пользователей по ключевому слову
            var users = await _userRepository.SearchUsersByKeywordAsync(keyword);
            return users;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null; // Возвращаем пустой список, если ключевое слово пустое
            }
            var user = await _userRepository.GetUserByEmailAsync(email);
            return user;
        }

        public async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
        }
        
    }
}
