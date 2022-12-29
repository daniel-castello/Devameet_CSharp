using Devameet_CSharp.Models;

namespace Devameet_CSharp.Repository.Impl
{
    public class MeetRepositoryImpl : IMeetRepository
    {
        private readonly DevameetContext _context;

        public MeetRepositoryImpl(DevameetContext context)
        {
            _context = context;
        }

        public void CreateMeet(Meet meet)
        {
            _context.Meets.Add(meet);
            _context.SaveChanges();

        }

        public void DeleteMeet(int meetId)
        {
            Meet meet = _context.Meets.First(m => m.Id == meetId);
            _context.Remove(meet);
        }

        public Meet GetMeetById(int idMeet)
        {
            return _context.Meets.Where(m => m.Id == idMeet).FirstOrDefault();
        }

        public List<Meet> GetMeetByUser(int id)
        {
            return _context.Meets.Where(m => m.UserId == id).ToList();
        }

        public void UpdateMeet(Meet meet)
        {
            _context.Meets.Update(meet);
            _context.SaveChanges();
        }
    }
}
