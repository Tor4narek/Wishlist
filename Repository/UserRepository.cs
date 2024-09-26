using System.Collections.Generic;
using System.Threading.Tasks;
using Models;
namespace Repository;

public class UserRepository : IUserRepository
{
    private readonly FileRepository<User> _repository;

    public UserRepository()
    {
        _repository = new FileRepository<User>("../../data/Users.json", "users");
    }

    public async Task<User> GetUserAsync(string userId)
    {
        var users = await _repository.GetAllAsync();
        return users.FirstOrDefault(u => u.Id == userId);
    }

    // Новый метод для поиска пользователя по email
    public async Task<User> GetUserByEmailAsync(string email)
    {
        var users = await _repository.GetAllAsync();
        return users.FirstOrDefault(u => u.Email == email);;
    }

    public async Task AddUserAsync(User user)
    {
        await _repository.AddAsync(user);
    }

    public async Task DeleteUserAsync(string userId)
    {
        await _repository.DeleteAsync(u => u.Id == userId);
    }

    public async Task UpdateUserAsync(User updatedUser)
    {
        await _repository.UpdateAsync(u => u.Id == updatedUser.Id, updatedUser);
    }
}

