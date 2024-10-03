namespace View
{
    public class MenuView
    {
        private readonly Dictionary<string, Action> _menuActions;
        private readonly string[] _menuItems;

        public MenuView(Dictionary<string, Action> menuActions)
        {
            _menuActions = menuActions;
            _menuItems = menuActions.Keys.ToArray();
        }

        public void ShowMenu()
        {
           
            for (int i = 0; i < _menuItems.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {_menuItems[i]}");
            }

            var choice = Console.ReadLine();
            HandleChoice(choice);
        }

        private void HandleChoice(string choice)
        {
            if (int.TryParse(choice, out int index) && index > 0 && index <= _menuItems.Length)
            {
                var selectedAction = _menuItems[index - 1];
                _menuActions[selectedAction]?.Invoke();
            }
            else
            {
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
            }
        }
    }
}