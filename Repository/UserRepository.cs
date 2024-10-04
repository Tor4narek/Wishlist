using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Models;

namespace Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly FileRepository<User> _repository;

        public UserRepository()
        {
            _repository = new FileRepository<User>("../../data/Users.json", "users");
        }

        public async Task<User> GetUserAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var users = await _repository.GetAllAsync(token);
            return users.FirstOrDefault(u => u.Id == userId);
        }

        public async Task<IReadOnlyCollection<User>> SearchUsersByKeywordAsync(string keyword, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var users = await _repository.GetAllAsync(token);

            var filteredUsers = users.Where(u => 
                u.Name.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
                u.Email.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return filteredUsers;
        }

        public async Task<User> GetUserByEmailAsync(string email, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var users = await _repository.GetAllAsync(token);
            return users.FirstOrDefault(u => u.Email == email);
        }

        public async Task AddUserAsync(User user, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _repository.AddAsync(user, token);
        }

        public async Task DeleteUserAsync(string userId, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _repository.DeleteAsync(u => u.Id == userId, token);
        }

        public async Task UpdateUserAsync(User updatedUser, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            await _repository.UpdateAsync(u => u.Id == updatedUser.Id, updatedUser, token);
        }
    }
}
