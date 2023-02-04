using Devameet_CSharp.Dtos;
using Devameet_CSharp.Models;

namespace Devameet_CSharp.Repository
{
    public interface IRoomRepository
    {
        Task<Meet> GetRoom(string link);
        Task<ICollection<PositionDto>> ListUsersPosition(string link);
        Task<Room?> GetUserPosition(string clientId);
        
        Task DeleteUserPosition(string clientId);
        Task UpdateUserPosition(int userId, string link, string clientId, UpdatePositionDto dto);
        Task UpdateUserMute(ToggleMuteDto dto);
    }
}
