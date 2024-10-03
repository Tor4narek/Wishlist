using Models;
using Presenter;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace View
{
    public class PresentView : IPresentView
    {
        private PresentQueryPresenter _presentQueryPresenter;
        private PresentCommandsPresenter _presentCommandsPresenter;

        public PresentView()
        {
            _presentQueryPresenter = new PresentQueryPresenter();
            _presentCommandsPresenter = new PresentCommandsPresenter();
        }

        public async Task StartPresents(User user, Wishlist wishlist, bool update)
        {
            string userId = user.Id;
            string wishlistId = wishlist.Id;
            string wishlistName = wishlist.Name;
            bool continueRunning = true;

            while (continueRunning)
            {
                Console.Clear();  // Очищаем экран для обновления информации
                Console.WriteLine($"Подарки в вишлисте: {wishlistName}");

                // Показываем существующие подарки в вишлисте
                await ShowUserPresents(wishlistId);
                Dictionary<int,Func<Task>> menuActions = null;
                Dictionary<int, string> menuLabels = null;
                if (update)
                {
                    // Определяем действия меню
                     menuActions = new Dictionary<int, Func<Task>>()
                    {
                        { 1, async () => await AddPresent(userId, wishlistId) }, // Добавление нового подарка
                        { 2, async () => await UpdatePresentList() }, // Изменение подарка
                        { 3, () => Task.FromResult(continueRunning = false) } // Возврат назад
                    };

                    menuLabels = new Dictionary<int, string>()
                    {
                        { 1, "Добавить подарок" },
                        { 2, "Изменить подарок" },
                        { 3, "Назад" }
                    };
                }
                else
                {
                    // Определяем действия меню
                    menuActions = new Dictionary<int, Func<Task>>()
                    {
                       
                        { 1, async () => await ReservePresent(wishlistId,user) }, // Изменение подарка
                        { 2, () => Task.FromResult(continueRunning = false) } // Возврат назад
                    };

                     menuLabels = new Dictionary<int, string>()
                    {
                        { 1, "Зарезервировать подарок" },
                        { 2, "Назад" }
                    };
                }

                // Используем универсальное меню
                var menuView = new MenuView(menuActions, menuLabels);
                await menuView.ExecuteMenuChoice();

                // Небольшая задержка перед обновлением интерфейса для лучшего UX
                if (continueRunning)
                {
                    Console.WriteLine("\nНажмите любую клавишу, чтобы продолжить...");
                    Console.ReadKey();
                }
            }

            Console.WriteLine("Загрузка...");
        }

        public async Task ShowUserPresents(string wishlistId)
        {
            try
            {
                IReadOnlyCollection<Present> presents = await _presentQueryPresenter.LoadWishlistPresentsAsync(wishlistId);

                if (presents == null || presents.Count == 0)
                {
                    Console.WriteLine("В этом вишлисте пока нет подарков.");
                    return;
                }

                foreach (var present in presents)
                {
                    Console.WriteLine($"Имя: {present.Name}, Описание: {present.Description}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при загрузке подарков: {ex.Message}");
            }
        }

        public async Task AddPresent(string userId, string wishlistId)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = cancellationTokenSource.Token;

            try
            {
                Console.WriteLine("Создание подарка");

                string p_name;
                do
                {
                    Console.Write("Введите название подарка: ");
                    p_name = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(p_name))
                    {
                        Console.WriteLine("Название подарка не может быть пустым. Пожалуйста, введите корректное название.");
                    }
                }
                while (string.IsNullOrWhiteSpace(p_name));

                string p_description;
                do
                {
                    Console.Write("Введите комментарий: ");
                    p_description = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(p_description))
                    {
                        Console.WriteLine("Описание не может быть пустым. Пожалуйста, введите комментарий.");
                    }
                }
                while (string.IsNullOrWhiteSpace(p_description));

                await _presentCommandsPresenter.AddNewPresentAsync(p_name, p_description, userId, wishlistId, token);
                Console.WriteLine("Подарок успешно создан.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка: {e.Message}");
            }
        }

        public void ShowSearchedPresents(List<Present> presents)
        {
            throw new NotImplementedException();
        }

        public void ShowReservedPresents(List<Present> reservedPresents)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePresentList()
        {
            throw new NotImplementedException();
        }

        private async Task ReservePresent(string wishlistId, User user)
        {
            try
            {
                // Загружаем вишлисты пользователя
                IReadOnlyCollection<Present> presents = await _presentQueryPresenter.LoadWishlistPresentsAsync(wishlistId);

                // Проверяем наличие вишлистов
                if (presents == null || presents.Count == 0)
                {
                    Console.WriteLine("У вас нет доступных вишлистов для изменения.");
                    return;
                }

                // Выводим список вишлистов
                await ShowUserPresents(wishlistId);

                // Спрашиваем у пользователя, какой вишлист он хочет обновить
                int selectedPresentIndex;
                do
                {
                    Console.Write("\nВведите номер подарка, который хотите забронировать : ");
                    string input = Console.ReadLine();

                    // Проверка ввода номера вишлиста
                    if (!int.TryParse(input, out selectedPresentIndex) || selectedPresentIndex < 1 || selectedPresentIndex > presents.Count)
                    {
                        Console.WriteLine("Неверный ввод. Пожалуйста, введите корректный номер подарка.");
                    }
                }
                while (selectedPresentIndex < 1 || selectedPresentIndex > presents.Count);

                // Получаем выбранный вишлист по индексу
                var selectedPresent = presents.ElementAt(selectedPresentIndex - 1);

                await _presentCommandsPresenter.ReservePresentAsync(selectedPresent.Id,user.Id);
                Console.WriteLine("Подарок забронирован");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Произошла ошибка при бронировании товара: {e.Message}");
            }
        }
    }
}
