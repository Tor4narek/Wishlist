using Models;
using Repository;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Presenter.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private UserRepository _userRepository = new UserRepository();
        private User _authenticatedUser = null;

        public async Task RegisterUserAsync(string name, string email, string password)
        {
            // Валидация данных
            ValidateUserData(name, email, password);

            // Проверка, существует ли пользователь с таким email
            var userEmail = await _userRepository.GetUserByEmailAsync(email);
            if (userEmail == null)
            {
                string id = Guid.NewGuid().ToString();
                await _userRepository.AddUserAsync(new User(id, name, email, HashPassword(password)));
   
            }
            else
            {
                throw new Exception("The user already exists with this email");
            }
        }

        public async Task AuthenticateUserAsync(string email, string password)
        {
            // Валидация email и пароля
            ValidateEmailAndPassword(email, password);

            var user = await _userRepository.GetUserByEmailAsync(email);
            if (user != null)
            {
                if (user.PasswordHash == HashPassword(password))
                {
                    _authenticatedUser = user;
     
                }
                else
                {
                    throw new Exception("The password does not match");
                }
            }
            else
            {
                throw new Exception("The user does not exist");
            }
        }

        public async Task<User> GetAuthenticatedUserAsync()
        {
            if (_authenticatedUser != null)
            {
                return _authenticatedUser;
            }
            else
            {
                throw new Exception("No user is currently authenticated");
            }
        }
        public async Task LogoutAsync()
        {
            _authenticatedUser = null;
            
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        // Метод валидации для имени, email и пароля
        private void ValidateUserData(string name, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new Exception("Name cannot be empty");
            }

            if (!IsValidEmail(email))
            {
                throw new Exception("Invalid email format");
            }

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new Exception("Password must be at least 6 characters long");
            }
        }

        // Метод валидации для email и пароля (при аутентификации)
        private void ValidateEmailAndPassword(string email, string password)
        {
            if (!IsValidEmail(email))
            {
                throw new Exception("Invalid email format");
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                throw new Exception("Password cannot be empty");
            }
        }

        // Проверка валидности email с помощью регулярного выражения
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;

            try
            {
                var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled);
                return emailRegex.IsMatch(email);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
