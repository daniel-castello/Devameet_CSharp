using Devameet_CSharp.Dtos;
using Devameet_CSharp.Models;
using Microsoft.EntityFrameworkCore;

namespace Devameet_CSharp.Repository.Impl
{
    public class RoomRepositoryImpl : IRoomRepository
    {
        private readonly DevameetContext _context;

        public RoomRepositoryImpl(DevameetContext context)
        {
            _context = context;
        }
        
        public async Task<Meet> GetRoom(string link)
        {
            // TODO: Include Meet Objects
            return await _context.Meets.Where(m => m.Link == link).FirstAsync();
        }

        public async Task<ICollection<PositionDto>> ListUsersPosition(string link)
        {
            var meet = await _context.Meets.Where(m => m.Link == link).FirstAsync();
            var rooms = await _context.Rooms.Where(r => r.MeetId == meet.Id).ToListAsync();
            return rooms.Select(r => new PositionDto
            {
                X = r.X,
                Y = r.Y,
                Orientation = r.Orientation,
                Id = r.Id,
                Name = r.UserName,
                Avatar = r.Avatar,
                Muted = r.Muted,
                Meet = r.Meet.Link,
                User = r.UserId.ToString(),
                ClientId = r.ClientId
            }).ToList();
        }

        public Task<Room?> GetUserPosition(string clientId)
        {
            return _context.Rooms.Where(r => r.ClientId == clientId)
                .Include(room => room.Meet)
                .FirstOrDefaultAsync();
        }

        public async Task DeleteUserPosition(string clientId)
        {
            var room = await _context.Rooms.Where(r => r.ClientId == clientId).ToListAsync();
            _context.Rooms.RemoveRange(room);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserPosition(int userId, string link, string clientId, UpdatePositionDto dto)
        {
            var meet = await _context.Meets.Where(m => m.Link == link).FirstAsync();
            var user = await _context.Users.Where(u => u.Id == userId).FirstAsync();

            var usersInRoom = await _context.Rooms.Where(r => r.MeetId == meet.Id).ToListAsync();
            
            if (usersInRoom.Count > 20)
                throw new Exception("Meet is full");

            if (usersInRoom.Any(r => r.ClientId == clientId || r.UserId == userId))
            {
                var position = await _context.Rooms.Where(r => r.ClientId == clientId || r.UserId == userId).FirstAsync();
                position.X = dto.X;
                position.Y = dto.Y;
                position.Orientation = dto.Orientation;
            }
            else
            {
                var room = new Room();
                room.X = dto.X;
                room.Y = dto.Y;
                room.Orientation = dto.Orientation;
                room.ClientId = clientId;
                room.UserId = user.Id;
                room.MeetId = meet.Id;
                room.UserName = user.Name;
                room.Avatar = user.Avatar;
                
                await _context.Rooms.AddAsync(room);
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserMute(ToggleMuteDto dto)
        {
            
            var meet = await _context.Meets.Where(m => m.Link == dto.Link).FirstAsync();
            var user = await _context.Users.Where(u => u.Id == dto.UserId).FirstAsync();
            var room = await _context.Rooms.Where(r => r.MeetId == meet.Id && r.UserId == user.Id).FirstAsync();
            room.Muted = dto.Mute;
            await _context.SaveChangesAsync();
        }
    }
}
