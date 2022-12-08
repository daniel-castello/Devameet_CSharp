using Devameet_CSharp.Models;

namespace Devameet_CSharp.Repository
{
    public interface IUserRepository
    {
        User GetUserById(int id);
        User GetUserByLoginPassword(string email, string password);
    }
}
