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
                while (_isLoggedIn)
                {
                    Console.WriteLine($"Привет! {username}");
            
                    var menuActions = new Dictionary<int, Func<Task>>()
                    {
                        { 1, async () => await _wishlistView.StartWishlist(user) },  // Асинхронный вызов метода для работы с вишлистами
                        { 2, async () => await SearchUser() },                       // Асинхронный вызов поиска пользователя
                        { 3, async () =>  SearchGift() },                       // Асинхронный вызов поиска подарка
                        { 4, async () => ExitProgram() }                             // Выход из программы
                    };

                    ShowMenu(menuActions);
                    int choice = GetUserInput();

                    if (menuActions.ContainsKey(choice))
                    {
                        await menuActions[choice](); // Выполняем выбранное действие
                    }
                    else
                    {
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }

        private void ShowMenu(Dictionary<int, Func<Task>> menuActions)
        {
            Console.WriteLine("1. Посмотреть вишлисты");
            Console.WriteLine("2. Найти пользователя");
            Console.WriteLine("3. Найти подарок");
            Console.WriteLine("4. Выход");
        }

        private int GetUserInput()
        {
            int choice;
            do
            {
                Console.Write("Выберите действие: ");
            }
            while (!int.TryParse(Console.ReadLine(), out choice));
    
            return choice;
        }


        // Метод выхода
        private void ExitProgram()
        {
            Console.WriteLine("Выход из программы...");
            _userPresenter.LogoutAsync().Wait();  // Очищаем данные пользователя
            _isLoggedIn = false;
        }

        // Заглушка для метода поиска пользователя
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

            // Проверяем, есть ли результаты
            if (user != null)
            {
                Console.WriteLine("Результаты поиска:");
                Console.WriteLine($"Имя: {user.Name}, Email: {user.Email}");
            }
            else
            {
                Console.WriteLine("Пользователи не найдены.");
            }

            Console.WriteLine("Нажмите любую клавишу, чтобы вернуться в главное меню...");
            Console.ReadKey(); // Ожидаем нажатие клавиши, чтобы не перескакивать сразу на следующее меню
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
