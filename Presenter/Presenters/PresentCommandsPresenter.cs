using Models;
using Repository;

namespace Presenter
{
    public class PresentCommandsPresenter : IPresentCommandsPresenter
    {
        private readonly PresentRepository _presentRepository;

        public PresentCommandsPresenter()
        {
            _presentRepository = new PresentRepository();
        }

        // Добавление нового подарка
        public async Task AddNewPresentAsync(string name, string description, string reserverId, string wishlistId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (string.IsNullOrWhiteSpace(reserverId))
            {
                throw new ArgumentNullException(nameof(reserverId));
            }

            if (string.IsNullOrWhiteSpace(wishlistId))
            {
                throw new ArgumentNullException(nameof(wishlistId));
            }

            Guid id = Guid.NewGuid();
            Present present = new Present(id, name, description, wishlistId, false, reserverId);
            
            token.ThrowIfCancellationRequested();  // Проверка на отмену
            await _presentRepository.AddPresentAsync(present, token);
        }

        // Удаление подарка по ID
        public async Task DeletePresentAsync(Guid presentId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();  // Проверка на отмену
            await _presentRepository.DeletePresentAsync(presentId, token);
        }

        // Резервирование подарка
        public async Task ReservePresentAsync(Guid presentId, string reserverId, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(reserverId))
            {
                throw new ArgumentNullException(nameof(reserverId));
            }

            token.ThrowIfCancellationRequested();  // Проверка на отмену
            await _presentRepository.ReservePresentAsync(presentId, reserverId, token);
        }
    }
}
