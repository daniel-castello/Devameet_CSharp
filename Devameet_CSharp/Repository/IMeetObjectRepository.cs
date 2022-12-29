using Devameet_CSharp.Models;

namespace Devameet_CSharp.Repository
{
    public interface IMeetObjectRepository
    {
        void CreateObjectsMeet(List<MeetObjects> meetObjects, int meetId);
    }
}
