namespace Devameet_CSharp.Models
{
    public class UserRoom
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Orientation { get; set; }
    }
}
