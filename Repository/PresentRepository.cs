using Models;

namespace Repository;

public class PresentRepository : IPresentRepository
{
    private readonly FileRepository<Present> _repository;

    public PresentRepository()
    {
        _repository = new FileRepository<Present>("../../data/Presents.json", "presents.json");
    }
    public async Task<IReadOnlyCollection<Present>> GetPresentsAsync(string wishlistId)
    {
        var presents = await _repository.GetAllAsync();
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
        await _repository.AddAsync(present);
        
    }

    public async Task DeletePresentAsync(Guid presentId)
    {
       await _repository.DeleteAsync(p => p.Id == presentId);
    }

    public async Task<IReadOnlyCollection<Present>> SearchPresentsByKeywordAsync(string keyword)
    {
        // Загружаем все подарки
        List<Present> presents = await _repository.GetAllAsync();
    
        // Фильтруем подарки по наличию ключевого слова в названии или описании (без учета регистра)
        var filteredPresents = presents.Where(p => 
                p.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                p.Description.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return filteredPresents;
    }


    public async Task ReservePresentAsync(Guid presentId, string reserverId)
    {
        // Загрузка подарков
        List<Present> presents = await _repository.GetAllAsync();

        // Поиск подарка с этим id
        var present = presents.FirstOrDefault(p => p.Id == presentId);

        // If present does not exist, throw an exception
        if (present == null)
            throw new Exception("Подарок не найден");

        // Check if the present is already reserved
        if (present.IsReserved)
            throw new Exception("Подарок уже зарезервирован");

        // Create a new Present with updated reservation details
        var updatedPresent = present with
        {
            IsReserved = true,
            ReserverId = reserverId
        };

        // Update the present in the repository
        await _repository.UpdateAsync(p => p.Id == presentId, updatedPresent);
    }


    public async Task<IReadOnlyCollection<Present>> GetReservedPresentsAsync(string userId)
    {
        List<Present> presents = await _repository.GetAllAsync();
        return presents.Where(p => p.IsReserved && p.ReserverId == userId).ToList();
    }
}