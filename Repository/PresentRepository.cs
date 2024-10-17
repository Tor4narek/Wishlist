using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Repository
{
    public class PresentRepository : IPresentRepository
    {
        private readonly IFileRepository<Present> _repository;

        public PresentRepository()
        {
            _repository = new FileRepository<Present>("../../data/Presents.json", "presents.json");
        }
        public PresentRepository(IFileRepository<Present> repository)
        {
            _repository = repository;
        }

        public async Task<IReadOnlyCollection<Present>> GetPresentsAsync(string wishlistId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var presents = await _repository.GetAllAsync(token);
            var filteredPresents = presents.Where(p => p.WishlistId == wishlistId).ToList();
            return filteredPresents;
        }

        public async Task AddPresentAsync(Present present, CancellationToken token)
        {
            if (present == null)
            {
                throw new ArgumentNullException(nameof(present));
            }
            token.ThrowIfCancellationRequested();
            await _repository.AddAsync(present, token);
        }

        public async Task DeletePresentAsync(Guid presentId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _repository.DeleteAsync(p => p.Id == presentId, token);
        }

        public async Task<IReadOnlyCollection<Present>> SearchPresentsByKeywordAsync(string keyword, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Present> presents = await _repository.GetAllAsync(token);

            var filteredPresents = presents.Where(p => 
                    !p.IsReserved &&  // Условие, чтобы показывать только незарезервированные подарки
                    (p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                     p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            return filteredPresents;
        }


        public async Task ReservePresentAsync(Guid presentId, string reserverId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Present> presents = await _repository.GetAllAsync(token);

            var present = presents.FirstOrDefault(p => p.Id == presentId);

            if (present == null)
                throw new Exception("Подарок не найден");

            if (present.IsReserved)
                throw new Exception("Подарок уже зарезервирован");

            var updatedPresent = present with
            {
                IsReserved = true,
                ReserverId = reserverId
            };

            await _repository.UpdateAsync(p => p.Id == presentId, updatedPresent, token);
        }

        public async Task<IReadOnlyCollection<Present>> GetReservedPresentsAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Present> presents = await _repository.GetAllAsync(token);
            return presents.Where(p => p.IsReserved && p.ReserverId == userId).ToList();
        }
    }
}
