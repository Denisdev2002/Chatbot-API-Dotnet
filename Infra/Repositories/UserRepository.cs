//using Domain.Entities;
//using Infra.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Infra.Repositories
//{
//    public class UserRepository : IUserRepository
//    {
//        private readonly ContextDatabase _contextDatabase;

//        public UserRepository(ContextDatabase contextDatabase)
//        {
//            _contextDatabase = contextDatabase;
//        }

//        public async Task<List<User>> GetUsersAsync()
//        {
//            return await _contextDatabase.Users.ToListAsync();
//        }

//        public async Task InsertUserAsync(User user)
//        {
//            _contextDatabase.Users.Add(user);
//            await _contextDatabase.SaveChangesAsync();
//        }

//        public async Task UpdateUserAsync(User user)
//        {
//            try
//            {
//                _contextDatabase.Users.Update(user);
//                await _contextDatabase.SaveChangesAsync();
//            }
//            catch (Exception ex)
//            {
//                Console.WriteLine($"Erro ao salvar mudanças na entidade: {ex.Message}");
//                Console.WriteLine($"StackTrace: {ex.StackTrace}");
//                throw;
//            }
//        }

//        public async Task DeleteUserAsync(User user)
//        {
//            _contextDatabase.Users.Remove(user);
//            await _contextDatabase.SaveChangesAsync();
//        }

//        public async Task<User> GetUserByIdAsync(string userId)
//        {
//            return await _contextDatabase.Users.FirstOrDefaultAsync(x => x.Id == userId);
//        }

//        public async Task<User> GetUserByEmailAsync(string email)
//        {
//            return await _contextDatabase.Users.FirstOrDefaultAsync(x => x.Email == email);
//        }
//    }
//}