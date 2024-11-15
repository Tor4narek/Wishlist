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
        private PresentView _presentView = new PresentView();
        private bool _isProgramRunning = true;
        private bool _isLoggedIn = false;

        public async Task Start()
        {
            var token = new CancellationToken();
            while (_isProgramRunning)
            {
                await AuthUser();
                if (_isProgramRunning && _isLoggedIn)
                {
                    var authenticatedUser = await _userPresenter.GetAuthenticatedUserAsync(token);

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
           
                var username = user.Name;
                _isLoggedIn = true;

                var menuActions = new Dictionary<int, Func<Task>>()
                {
                    { 1, async () => await _wishlistView.StartWishlist(user) },
                    { 2, async () => await SearchUser() },
                    { 3, async () =>  await SearchPresents() },
                    { 4, async () => ExitProgram() }
                };

                var menuLabels = new Dictionary<int, string>()
                {
                    { 1, "Посмотреть вишлисты" },
                    { 2, "Найти пользователя" },
                    { 3, "Найти подарок" },
                    { 4, "Выход" }
                };

                var menuView = new MenuView(menuActions, menuLabels);

                while (_isLoggedIn)
                {
                    Console.WriteLine($"Привет, {username}!");
                    await menuView.ExecuteMenuChoice();
                }
        }
        

        // Метод выхода
        private async Task ExitProgram()
        {
            Console.WriteLine("Выход из программы...");
            await _userPresenter.LogoutAsync();
            _isLoggedIn = false;
        }

        // Поиск пользователя
        private async Task SearchUser()
        {
            CancellationToken token = new CancellationToken();
            Console.WriteLine("Поиск пользователя (нажмите 'Esc' для отмены)");

            string keyword = await ReadInputWithEsc("Введите email или имя для поиска: ");
            if (keyword == null) return;

            var user = await _userPresenter.GetUserByEmailAsync(keyword, token);

            if (user != null)
            {
                Console.WriteLine($"Имя: {user.Name}, Email: {user.Email}");

                var searchMenuActions = new Dictionary<int, Func<Task>>()
                {
                    { 1, async () => await ShowUserWishes(user) },
                    { 2, () => Task.CompletedTask }
                };

                var searchMenuLabels = new Dictionary<int, string>()
                {
                    { 1, "Посмотреть вишлисты пользователя" },
                    { 2, "Назад в главное меню" }
                };

                var menuView = new MenuView(searchMenuActions, searchMenuLabels);
                await menuView.ExecuteMenuChoice();
            }
            else
            {
                Console.WriteLine("Пользователи не найдены.");
            }
        }

        private async Task SearchPresents()
        {
            CancellationToken token = new CancellationToken();
            User user = await _userPresenter.GetAuthenticatedUserAsync(token);
            await _presentView.ShowSearchedPresents(token, user);
            
        }

        private async Task ShowUserWishes(User user)
        {
            await _wishlistView.ShowUserWishlistsAsync(user);
            await _wishlistView.UpdateWishlist(user, false);
            
        }

        // Меню аутентификации
        public async Task AuthUser()
        {
            var menuActions = new Dictionary<int, Func<Task>>()
            {
                { 1, async () => await RegisterUser() },
                { 2, async () => await LoginUser() },
                { 3, () => Task.Run(() => System.Environment.Exit(0)) }
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
                await menuView.ExecuteMenuChoice();
            }
        }

        // Регистрация нового пользователя
        private async Task RegisterUser()
        {
            CancellationToken token = new CancellationToken();
            while (true)
            {
                Console.WriteLine("Регистрация нового пользователя (нажмите 'Esc' для отмены)...");

                string name = await ReadInputWithEsc("Введите имя: ");
                if (name == null) return;

                string email = await ReadInputWithEsc("Введите электронную почту: ");
                if (email == null) return;

                string password = await ReadInputWithEsc("Введите пароль: ");
                if (password == null) return;
                try
                {
                    await _userPresenter.CreateUserAsync(name, email, password, token);
                    Console.WriteLine("Пользователь успешно зарегистрирован.");
                    break;
                }
                catch (AuthenticationServiceException e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                   
            }
        }

        // Вход пользователя
        private async Task LoginUser()
        {
            CancellationToken token = new CancellationToken();
            while (true)
            {
                Console.WriteLine("Аутентификация пользователя (нажмите 'Esc' для отмены)...");

                string email = await ReadInputWithEsc("Введите электронную почту: ");
                if (email == null) return;

                string password = await ReadInputWithEsc("Введите пароль: ");
                if (password == null) return;

                try
                {
                    await _userPresenter.AuthenticateUserAsync(email, password, token);
                    Console.WriteLine("Пользователь успешно аутентифицирован.");
                    _isLoggedIn = true;
                    break;
                }
                catch (AuthenticationServiceException ex)
                {
                    Console.WriteLine($"Ошибка при аутентификации: {ex.Message}. Попробуйте снова.");
                }
            }
        }

        // Общий метод для чтения ввода с отменой через Esc
        private async Task<string> ReadInputWithEsc(string prompt)
        {
            Console.Write(prompt);
            StringBuilder input = new StringBuilder();

            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);

                    // Если нажата клавиша Tab, возвращаем null для отмены ввода
                    if (key.Key == ConsoleKey.Tab)
                    {
                        Console.WriteLine("\nВвод отменен, возвращение в меню...");
                        return null;
                    }
                    // Если нажата клавиша Enter, завершаем ввод
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine(); // Переход на новую строку после завершения ввода
                        break;
                    }
                    // Обработка удаления символов при нажатии Backspace
                    else if (key.Key == ConsoleKey.Backspace && input.Length > 0)
                    {
                        input.Remove(input.Length - 1, 1);
                        Console.Write("\b \b"); // Удаление символа из консоли
                    }
                    // Добавление символа в строку, если это не управляющий символ
                    else if (!char.IsControl(key.KeyChar))
                    {
                        input.Append(key.KeyChar);
                        Console.Write(key.KeyChar); // Отображение символа в консоли
                    }
                }

                // Немного времени для уменьшения нагрузки на CPU
                await Task.Delay(50);
            }

            return input.ToString();
        }
        
        public void UpdateUserList()
        {
            throw new NotImplementedException();
        }
    }
}
