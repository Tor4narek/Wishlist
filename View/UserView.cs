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
                    var authenticatedUser = await _userPresenter.GetAuthenticatedUserAsync();

                    if (authenticatedUser == null)
                    {
                        Console.WriteLine("Нет аутентифицированного пользователя. Попробуйте снова.");
                        continue;
                    }
                    else
                    {
                        await ShowUser(authenticatedUser);
                    }
                }
            }
        }

        // Меню для пользователя
        public async Task ShowUser(User user)
        {
            try
            {
                var username = user.Name;
                _isLoggedIn = true;

                // Определяем действия для меню пользователя
                var menuActions = new Dictionary<int, Func<Task>>()
                {
                    { 1, async () => await _wishlistView.StartWishlist(user) },  // Асинхронный вызов метода для работы с вишлистами
                    { 2, async () => await SearchUser() },                       // Асинхронный вызов поиска пользователя
                    { 3, async () =>  SearchGift() },                       // Асинхронный вызов поиска подарка
                    { 4, async () => ExitProgram() }                             // Выход из программы
                };

                var menuLabels = new Dictionary<int, string>()
                {
                    { 1, "Посмотреть вишлисты" },
                    { 2, "Найти пользователя" },
                    { 3, "Найти подарок" },
                    { 4, "Выход" }
                };

                // Используем универсальный класс MenuView
                var menuView = new MenuView(menuActions, menuLabels);

                while (_isLoggedIn)
                {
                    Console.WriteLine($"Привет, {username}!");
                    await menuView.ExecuteMenuChoice();  // Показываем меню и выполняем действия
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

        // Поиск пользователя
        private async Task SearchUser()
        {
            Console.WriteLine("Поиск пользователя");
            string keyword;
            do
            {
                Console.Write("Введите email или имя для поиска: ");
                keyword = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(keyword))
                {
                    Console.WriteLine("Поле не может быть пустым. Пожалуйста, введите корректный Email или имя.");
                }
            }
            while (string.IsNullOrWhiteSpace(keyword));

            // Выполняем поиск пользователя
            var user = await _userPresenter.GetUserByEmailAsync(keyword);

            if (user != null)
            {
                Console.WriteLine("Результаты поиска:");
                Console.WriteLine($"Имя: {user.Name}, Email: {user.Email}");

                var searchMenuActions = new Dictionary<int, Func<Task>>()
                {
                    { 1, async () => await ShowUserWishes(user) },   // Показать вишлисты найденного пользователя
                    { 2, () => Task.CompletedTask }                  // Возврат в главное меню
                };

                var searchMenuLabels = new Dictionary<int, string>()
                {
                    { 1, "Посмотреть вишлисты пользователя" },
                    { 2, "Назад в главное меню" }
                };

                var menuView = new MenuView(searchMenuActions, searchMenuLabels);
                await menuView.ExecuteMenuChoice();  // Показываем меню действий после поиска пользователя
            }
            else
            {
                Console.WriteLine("Пользователи не найдены.");
            }
        }

        private async Task ShowUserWishes(User user)
        {
            try
            {
                await _wishlistView.ShowUserWishlistsAsync(user);

                Console.WriteLine("Нажмите любую клавишу, чтобы вернуться...");
                Console.ReadKey();  // Ждем нажатие клавиши
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при показе вишлистов: {ex.Message}");
            }
        }

        // Поиск подарка
        private void SearchGift()
        {
            Console.WriteLine("Логика поиска подарка...");
            // Добавьте здесь свою логику поиска
        }

        // Меню аутентификации
        public async Task AuthUser()
        {
            var menuActions = new Dictionary<int, Func<Task>>()
            {
                { 1, async () => await RegisterUser() },  // Регистрация
                { 2, async () => await LoginUser() },     // Вход
                { 3, () => Task.Run(() => System.Environment.Exit(0)) }  // Выход
            };

            var menuLabels = new Dictionary<int, string>()
            {
                { 1, "Зарегистрироваться" },
                { 2, "Войти" },
                { 3, "Выход" }
            };

            var menuView = new MenuView(menuActions, menuLabels);

            while (!_isLoggedIn)
            {
                Console.WriteLine("Выберите действие:");
                await menuView.ExecuteMenuChoice();  // Показываем меню аутентификации
            }
        }

        // Регистрация нового пользователя
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

        // Вход пользователя
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
                    _isLoggedIn = true;
                    break;
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
