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
        private bool _isProgramRunning = true;

        public async Task Start()
        {
            while (_isProgramRunning)
            {
                await AuthUser();
                if (_isProgramRunning)
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
                        await ShowUser();
                    }
                }


            }
        }

        public async Task ShowUser()
        {
            try
            {
                var username = await _userPresenter.GetAuthenticatedUserAsync();

                if (username == null)
                {
                    Console.WriteLine("Пользователь не аутентифицирован. Возвращение в главное меню.");
                    return;
                }

                while (true)
                {
                    Console.WriteLine($"Привет! {username.Name}");
                    Console.WriteLine("1. Посмотреть вишлисты");
                    Console.WriteLine("2. Найти пользователя");
                    Console.WriteLine("3. Найти подарок");
                    Console.WriteLine("4. Выход");

                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            // Логика просмотра вишлистов
                            break;

                        case "2":
                            // Логика поиска пользователя
                            break;

                        case "3":
                            // Логика поиска подарка
                            break;

                        case "4":
                            Console.WriteLine("Выход из программы...");
                            await _userPresenter.LogoutAsync();  // Очищаем данные пользователя
                            return;  // Выход из метода ShowUser, программа вернется в цикл Start
                        default:
                            Console.WriteLine("Неверный выбор. Пожалуйста, выберите 1, 2 или 3.");
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }

        public async Task AuthUser()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("Выберите действие:");
                    Console.WriteLine("1. Зарегистрироваться");
                    Console.WriteLine("2. Войти");
                    Console.WriteLine("3. Выход");

                    var choice = Console.ReadLine();

                    switch (choice)
                    {
                        case "1":
                            await RegisterUser();
                            break;

                        case "2":
                            await LoginUser();
                            return;

                        case "3":
                            Console.WriteLine("Выход из программы...");
                            _isProgramRunning = false;  // Завершение программы
                            return;

                        default:
                            Console.WriteLine("Неверный выбор. Пожалуйста, выберите 1, 2 или 3.");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Произошла ошибка: {e.Message}");
                }
            }
        }

        private async Task RegisterUser()
        {
            while (true)
            {
                Console.WriteLine("Регистрация нового пользователя...");

                Console.Write("Введите имя: ");
                string name = Console.ReadLine();

                Console.Write("Введите электронную почту: ");
                string email = Console.ReadLine();

                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();

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

                Console.Write("Введите электронную почту: ");
                string email = Console.ReadLine();

                Console.Write("Введите пароль: ");
                string password = Console.ReadLine();

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
