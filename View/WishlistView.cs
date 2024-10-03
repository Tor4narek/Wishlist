using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Presenter;
namespace View;

public class WishlistView : IWishlistView
{
    private WishlistPresenter _wishlistPresenter = new WishlistPresenter();
    private PresentView _presentView = new PresentView();

    public async Task StartWishlist(User user)
    {
        string userId = user.Id;
        bool continueRunning = true;

        while (continueRunning)
        {
            Console.Clear();  // Очищаем экран для обновления информации
            Console.WriteLine($"\nПривет! {user.Name}\n");

            // Console.WriteLine("Ваши текущие вишлисты:\n");

            // Показываем существующие вишлисты пользователя
            await ShowUserWishlistsAsync(user);

            Console.WriteLine("\nЧто вы хотите сделать?");
            Console.WriteLine("1. Создать новый вишлист");
            Console.WriteLine("2. Изменить вишлист");
            Console.WriteLine("3. Назад");

            Console.Write("Введите ваш выбор (1, 2 или 3): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Создаем новый вишлист
                    await AddWishlistAsync(userId);
                    break;

                case "2":
                    // Функция обновления вишлиста (можно реализовать отдельно)
                   await UpdateWishlist(user);
                    break;

                case "3":
                    // Завершаем работу программы
                    continueRunning = false;
                    break;

                default:
                    Console.WriteLine("Неверный выбор. Пожалуйста, выберите 1, 2 или 3.");
                    break;
            }

            // Небольшая задержка перед обновлением интерфейса для лучшего UX
            if (continueRunning)
            {
                Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                Console.ReadKey();
            }
        }

        Console.WriteLine("Программа завершена.");
    }

    public async Task ShowUserWishlistsAsync(User user)
    {
        try
        {
            // Загружаем вишлисты пользователя
            IReadOnlyCollection<Wishlist> wishlists = await _wishlistPresenter.LoadUserWishlistsAsync(user.Id);

            // Проверяем наличие вишлистов
            if (wishlists == null || wishlists.Count == 0)
            {
                Console.WriteLine("У вас нет доступных вишлистов.");
                return;
            }

            // Выводим все вишлисты пользователя
            Console.WriteLine("Вишлисты:\n");
            int index = 1;
            foreach (var wishlist in wishlists)
            {
                Console.WriteLine($"{index}. Имя: {wishlist.Name}, Описание: {wishlist.Description}, {wishlist.PresentsNumber} подарков");
                index++;
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Произошла ошибка при загрузке вишлистов: {e.Message}");
        }
    }


    public async Task AddWishlistAsync(string w_ownerId)
    {
        try
        {
            Console.WriteLine("Создание вишлиста");

            string w_name;
            do
            {
                Console.Write("Введите название вишлиста: ");
                w_name = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(w_name))
                {
                    Console.WriteLine("Название вишлиста не может быть пустым. Пожалуйста, введите корректное название.");
                }
            }
            while (string.IsNullOrWhiteSpace(w_name));

            string w_description;
            do
            {
                Console.Write("Введите комментарий: ");
                w_description = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(w_description))
                {
                    Console.WriteLine("Описание не может быть пустым. Пожалуйста, введите комментарий.");
                }
            }
            while (string.IsNullOrWhiteSpace(w_description));

            await _wishlistPresenter.AddNewWishlistAsync(w_name, w_description, w_ownerId, "0");
            Console.WriteLine("Вишлист успешно создан.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Произошла ошибка: {e.Message}");
        }
    }

    public async Task UpdateWishlist(User user)
    {
        try
        {
            // Загружаем вишлисты пользователя
            IReadOnlyCollection<Wishlist> wishlists = await _wishlistPresenter.LoadUserWishlistsAsync(user.Id);

            // Проверяем наличие вишлистов
            if (wishlists == null || wishlists.Count == 0)
            {
                Console.WriteLine("У вас нет доступных вишлистов для изменения.");
                return; // Если вишлистов нет, выходим
            }

            // Выводим список вишлистов
            await ShowUserWishlistsAsync(user);

            // Спрашиваем у пользователя, какой вишлист он хочет обновить
            int selectedWishlistIndex;
            do
            {
                Console.Write("\nВведите номер вишлиста, который хотите обновить: ");
                string input = Console.ReadLine();

                // Проверка ввода номера вишлиста
                if (!int.TryParse(input, out selectedWishlistIndex) || selectedWishlistIndex < 1 || selectedWishlistIndex > wishlists.Count)
                {
                    Console.WriteLine("Неверный ввод. Пожалуйста, введите корректный номер вишлиста.");
                }
            }
            while (selectedWishlistIndex < 1 || selectedWishlistIndex > wishlists.Count);

            // Получаем выбранный вишлист по индексу
            var selectedWishlist = wishlists.ElementAt(selectedWishlistIndex - 1);

            // Вызываем функцию для работы с подарками в выбранном вишлисте
            await _presentView.StartPresents(user, selectedWishlist);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Произошла ошибка при обновлении вишлиста: {e.Message}");
        }
    }





}