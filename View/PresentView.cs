using Models;
using Presenter;

namespace View;

public class PresentView : IPresentView
{
    PresentQueryPresenter _presentQueryPresenter;
    PresentCommandsPresenter _presentCommandsPresenter;

    public PresentView()
    {
        _presentQueryPresenter = new PresentQueryPresenter();
        _presentCommandsPresenter = new PresentCommandsPresenter();
    }
    public async Task StartPresents(User user, Wishlist wishlist)
    {
        string userId = user.Id;
        string wishlistId = wishlist.Id;
        string WishlistName = wishlist.Name;
        bool continueRunning = true;

        while (continueRunning)
        {
            Console.Clear();  // Очищаем экран для обновления информации
            
            Console.WriteLine($"Подарки в вишлисте: {WishlistName}");

            // Показываем существующие вишлисты пользователя
            await ShowUserPresents(wishlistId);

            Console.WriteLine("\nЧто вы хотите сделать?");
            Console.WriteLine("1. Добавить подарок");
            Console.WriteLine("2. Изменить подарок");
            Console.WriteLine("3. Назад");

            Console.Write("Введите ваш выбор (1, 2 или 3): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    // Создаем новый подарок
                    await AddPresent(userId, wishlistId);
                    break;

                case "2":
                    // Функция обновления подарка
                    UpdatePresentList();
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
                Console.Write("Введите коментарий: ");
                p_description = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(p_description))
                {
                    Console.WriteLine("Описание не может быть пустым. Пожалуйста, введите комментарий.");
                }
            }
            while (string.IsNullOrWhiteSpace(p_description));
                await _presentCommandsPresenter.AddNewPresentAsync(p_name, p_description,userId,wishlistId,token);
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

    public void UpdatePresentList()
    {
        throw new NotImplementedException();
    }
}