using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class MenuView
{
    private readonly Dictionary<int, Func<Task>> _menuActions;
    private readonly Dictionary<int, string> _menuLabels;

    // Конструктор принимает словарь с действиями и метками для пунктов меню
    public MenuView(Dictionary<int, Func<Task>> menuActions, Dictionary<int, string> menuLabels)
    {
        _menuActions = menuActions ?? throw new ArgumentNullException(nameof(menuActions));
        _menuLabels = menuLabels ?? throw new ArgumentNullException(nameof(menuLabels));
        
        // Проверяем, что у всех действий есть соответствующие метки
        if (_menuActions.Count != _menuLabels.Count)
        {
            throw new ArgumentException("Количество действий должно совпадать с количеством меток меню.");
        }
    }

    // Метод для отображения меню
    public void ShowMenu()
    {
        Console.WriteLine("Меню:");
        foreach (var label in _menuLabels)
        {
            Console.WriteLine($"{label.Key}. {label.Value}");
        }
    }

    // Метод для получения ввода пользователя
    public int GetUserInput()
    {
        int choice;
        do
        {
            Console.Write("Выберите действие: ");
        }
        while (!int.TryParse(Console.ReadLine(), out choice));

        return choice;
    }

    // Метод для выполнения действия
    public async Task ExecuteMenuChoice()
    {
        while (true)
        {
            ShowMenu();
            var choice = GetUserInput();

            if (_menuActions.ContainsKey(choice))
            {
                await _menuActions[choice]();  // Выполняем выбранное действие
                break;  // Если действие успешно выполнено, выходим из цикла
            }
            else
            {
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
            }
        }
    }
}