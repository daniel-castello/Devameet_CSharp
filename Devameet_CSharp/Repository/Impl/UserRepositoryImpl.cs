using Devameet_CSharp.Models;

namespace Devameet_CSharp.Repository.Impl
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly DevameetContext _context;

        public UserRepositoryImpl(DevameetContext context)
        {
            _context = context;
        }

        public User GetUserById(int id)
        {
            return _context.Users.FirstOrDefault(u => u.Id == id);
        }

        public User GetUserByLoginPassword(string email, string password)
        {
            return _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
        }

        public void Save(User user)
        {
            _context.Add(user);
            _context.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            _context.Update(user);
            _context.SaveChanges();
        }

        public bool VerifyEmail(string email)
        {
            return _context.Users.Any(u => u.Email == email);
        }
    }
}
