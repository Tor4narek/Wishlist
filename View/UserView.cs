using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presenter.Services;
using Presenter;

namespace View
{
    public class UserView : IUserView
    {
        private UserPresenter _userPresenter = new UserPresenter();
        private WishlistView _wishlistView = new WishlistView();
        private bool _isProgramRunning = true;
        private bool _isLoggedIn = false;

        public async Task Start()
        {
            while (_isProgramRunning)
            {
                await AuthUser();
                if (_isProgramRunning && _isLoggedIn)
                {
                    // Попытка получить аутентифицированного пользователя
                    var authenticatedUser = await _userPresenter.GetAuthenticatedUserAsync();

                    // Если пользователь не аутентифицирован, предлагаем повторить авторизацию
                    if (authenticatedUser == null)
                    {
                        Console.WriteLine("Нет аутентифицированного пользователя. Попробуйте снова.");
                        continue; // Возвращаемся к AuthUser для новой попытки
                    }
                    else
                    {
                        // Если пользователь аутентифицирован, продолжаем работу
                        await ShowUser(authenticatedUser);
                    }
                }
            }
        }

        public async Task ShowUser(User user)
        {
            try
            {
                var username = user.Name;
                _isLoggedIn = true;

                // Определяем действия для меню
                var menuActions = new Dictionary<string, Action>
                {
                    { "Посмотреть вишлисты", () => _wishlistView.StartWishlist(user).Wait() },
                    { "Найти пользователя", () => SearchUser() },
                    { "Найти подарок", () => SearchGift() },
                    { "Выход", () => ExitProgram() }
                };

                var menuView = new MenuView(menuActions);

                while (_isLoggedIn)
                {
                    Console.WriteLine($"Привет! {username}");
                    menuView.ShowMenu();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }

        // Метод выхода
        private void ExitProgram()
        {
            Console.WriteLine("Выход из программы...");
            _userPresenter.LogoutAsync().Wait();  // Очищаем данные пользователя
            _isLoggedIn = false;
        }

        // Заглушка для метода поиска пользователя
        private void SearchUser()
        {
            Console.WriteLine("Логика поиска пользователя...");
            // Добавьте здесь свою логику поиска
        }

        // Заглушка для метода поиска подарка
        private void SearchGift()
        {
            Console.WriteLine("Логика поиска подарка...");
            // Добавьте здесь свою логику поиска
        }

        public async Task AuthUser()
        {
            // Определяем действия для меню
            var menuActions = new Dictionary<string, Action>
            {
                { "Зарегистрироваться", () => RegisterUser().Wait() },  // Регистрация нового пользователя
                { "Войти", () => 
                    {
                        LoginUser().Wait();  // Аутентификация пользователя
                        var authenticatedUser = _userPresenter.GetAuthenticatedUserAsync().Result;
                        if (authenticatedUser != null)
                        {
                            ShowUser(authenticatedUser).Wait();  // Если аутентификация успешна, показываем меню пользователя
                        }
                    } 
                },     
                { "Выход", () => System.Environment.Exit(0) }                        // Выход из программы
            };

            var menuView = new MenuView(menuActions);

            // Отображаем меню до тех пор, пока программа работает
            while (_isProgramRunning)
            {
                Console.WriteLine("Выберите действие:");
                menuView.ShowMenu();
            }
        }

        private async Task RegisterUser()
        {
            while (true)
            {
                Console.WriteLine("Регистрация нового пользователя...");

                string name;
                do
                {
                    Console.Write("Введите имя: ");
                    name = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        Console.WriteLine("Имя не может быть пустым. Пожалуйста, введите корректное имя.");
                    }
                }
                while (string.IsNullOrWhiteSpace(name));

                string email;
                do
                {
                    Console.Write("Введите электронную почту: ");
                    email = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        Console.WriteLine("Электронная почта не может быть пустой. Пожалуйста, введите корректный адрес.");
                    }
                }
                while (string.IsNullOrWhiteSpace(email));

                string password;
                do
                {
                    Console.Write("Введите пароль: ");
                    password = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Пароль не может быть пустым. Пожалуйста, введите пароль.");
                    }
                }
                while (string.IsNullOrWhiteSpace(password));

                try
                {
                    await _userPresenter.CreateUserAsync(name, email, password);
                    Console.WriteLine("Пользователь успешно зарегистрирован.");
                    break; // Выход из цикла после успешной регистрации
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при регистрации: {ex.Message}. Попробуйте снова.");
                }
            }
        }

        private async Task LoginUser()
        {
            while (true)
            {
                Console.WriteLine("Аутентификация пользователя...");

                string email;
                do
                {
                    Console.Write("Введите электронную почту: ");
                    email = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        Console.WriteLine("Электронная почта не может быть пустой. Пожалуйста, введите корректный адрес.");
                    }
                }
                while (string.IsNullOrWhiteSpace(email));

                string password;
                do
                {
                    Console.Write("Введите пароль: ");
                    password = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(password))
                    {
                        Console.WriteLine("Пароль не может быть пустым. Пожалуйста, введите пароль.");
                    }
                }
                while (string.IsNullOrWhiteSpace(password));

                try
                {
                    await _userPresenter.AuthenticateUserAsync(email, password);
                    Console.WriteLine("Пользователь успешно аутентифицирован.");
                    break; // Выход из цикла после успешной аутентификации
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при аутентификации: {ex.Message}. Попробуйте снова.");
                }
            }
        }

        public void UpdateUserList()
        {
            throw new NotImplementedException();
        }
    }
}
