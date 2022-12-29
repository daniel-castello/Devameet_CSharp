using Devameet_CSharp.Models;

namespace Devameet_CSharp.Repository
{
    public interface IMeetRepository
    {
        List<Meet> GetMeetByUser(int id);
        void DeleteMeet(int meetId);
        void CreateMeet(Meet meet);
        Meet GetMeetById(int idMeet);
        void UpdateMeet(Meet meet);
    }
}
