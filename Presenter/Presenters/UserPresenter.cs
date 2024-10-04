using Models;
using Presenter.Services;
using Repository;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Presenter
{
    public class UserPresenter : IUserPresenter
    {
        private readonly AuthenticationService _authService = new AuthenticationService();
        private readonly UserRepository _userRepository = new UserRepository();

        // Метод для создания нового пользователя
        public async Task CreateUserAsync(string name, string email, string password, CancellationToken token)
        {
            try
            {
                // Вызываем сервис для регистрации нового пользователя
                await _authService.RegisterUserAsync(name, email, password, token);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while creating user: {ex.Message}");
            }
        }

        // Метод для аутентификации пользователя
        public async Task<User> AuthenticateUserAsync(string email, string password, CancellationToken token)
        {
            try
            {
                await _authService.AuthenticateUserAsync(email, password, token);
                var user = await _authService.GetAuthenticatedUserAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error while authenticating user: {ex.Message}");
            }
        }

        // Метод для загрузки пользователя по ID
        public async Task<User> LoadUserAsync(string userId, CancellationToken token)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(userId,token);
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
        public async Task DeleteUserAsync(string userId, CancellationToken token)
        {
            try
            {
                var user = await _userRepository.GetUserAsync(userId,token);
                if (user != null)
                {
                    await _userRepository.DeleteUserAsync(userId,token);
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

        public async Task<User> GetAuthenticatedUserAsync(CancellationToken token)
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

        public async Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword, CancellationToken token)
        {
            // Проводим проверку входного параметра
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return Array.Empty<User>(); // Возвращаем пустой массив, если ключевое слово пустое
            }

            // Получаем пользователей по ключевому слову
            var users = await _userRepository.SearchUsersByKeywordAsync(keyword,token);
            return users;
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return null; // Возвращаем null, если ключевое слово пустое
            }
            var user = await _userRepository.GetUserByEmailAsync(email,token);
            return user;
        }

        public async Task LogoutAsync()
        {
            await _authService.LogoutAsync();
        }
    }
}
