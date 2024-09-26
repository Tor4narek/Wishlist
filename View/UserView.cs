using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presenter.Services;
using Presenter;
namespace View;

public class UserView : IUserView
{
    private UserPresenter _userPresenter = new UserPresenter(); 
    public async Task ShowUser()
    {
        while (true)
        {
            Console.WriteLine($"Привет!");
            Console.WriteLine("1. Посмотреть вишлисты");
            Console.WriteLine("2. Найти пользователя");
            Console.WriteLine("3. Найти подарок");
            Console.WriteLine("4. Выход");
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                   
                    break;

                case "2":
                   
                    break;
                case "3":
                   
                    break;

                case "4":
                    Console.WriteLine("Выход из программы...");
                    return;

                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, выберите 1, 2 или 3.");
                    break;
            }
        }

    }

    public async Task AuthUser()
    {
        
        while (true)
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
                    return;

                case "2":
                    await LoginUser();
                    return;

                case "3":
                    Console.WriteLine("Выход из программы...");
                    return;

                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, выберите 1, 2 или 3.");
                    break;
            }
        }
        
    }
    private async Task RegisterUser()
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
            await _userPresenter.CreateUserAsync(name,email,password);
            Console.WriteLine("Пользователь успешно зарегистрирован.");
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при регистрации: {ex.Message}");
        }
    }

    private async Task LoginUser()
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
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при аутентификации: {ex.Message}");
        }
    }

    public void UpdateUserList()
    {
        throw new NotImplementedException();
    }
}