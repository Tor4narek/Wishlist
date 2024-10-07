namespace Repository;

public interface IFileRepository<T>
{
    Task<List<T>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    Task DeleteAsync(Func<T, bool> predicate, CancellationToken cancellationToken);
    Task UpdateAsync(Predicate<T> predicate, T updatedEntity, CancellationToken cancellationToken);
}