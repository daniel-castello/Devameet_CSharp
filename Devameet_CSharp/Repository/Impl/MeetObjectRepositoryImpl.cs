using Devameet_CSharp.Models;

namespace Devameet_CSharp.Repository.Impl
{
    public class MeetObjectRepositoryImpl : IMeetObjectRepository
    {
        private readonly DevameetContext _context;

        public MeetObjectRepositoryImpl(DevameetContext context)
        {
            _context = context;
        }
        public void CreateObjectsMeet(List<MeetObjects> meetObjects, int meetId)
        {

            List<MeetObjects> meetObjectsDelete = _context.MeetObjects.Where(m => m.MeetId == meetId).ToList();

            foreach (MeetObjects meetObject in meetObjectsDelete)
            {
                _context.MeetObjects.Remove(meetObject);
                _context.SaveChanges();
            }

            foreach(MeetObjects meetobject in meetObjects)
            {
                _context.MeetObjects.Add(meetobject);
                _context.SaveChanges();
            }
        }
    }
}
