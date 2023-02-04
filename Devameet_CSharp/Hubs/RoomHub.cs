using Devameet_CSharp.Dtos;
using Devameet_CSharp.Repository;
using Microsoft.AspNetCore.SignalR;

namespace Devameet_CSharp.Hubs;

public class RoomHub : Hub
{
    private readonly IRoomRepository _roomRepository;
    
    private string ClientId => Context.ConnectionId;
    
    public RoomHub(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Client: {ClientId} connected!");
        await base.OnConnectedAsync();
    }
    
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        Console.WriteLine("Disconnecting client...");
        var userSocket = await _roomRepository.GetUserPosition(ClientId);
        
        if (userSocket == null)
            return;

        var link = userSocket.Meet.Link;
        await _roomRepository.DeleteUserPosition(ClientId);
        
        await Clients.Others.SendAsync($"remove-user", new { SocketId=ClientId });
        
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task Join(JoinDto dto)
    {
        var link = dto.Link;
        var userId = Int32.Parse(dto.UserId);
        
        
        Console.WriteLine("Joining room...");
        var userSocket = await _roomRepository.GetUserPosition(ClientId);
        if (userSocket != null)
        {
            Console.WriteLine("User already in room");
        }
        else
        {
            Console.WriteLine("User ID: " + userId + "ClientId: " + ClientId + " Link: " + link);
            await Groups.AddToGroupAsync(ClientId, link);
        
            var updatePositionDto = new UpdatePositionDto();
            updatePositionDto.X = 2;
            updatePositionDto.Y = 2;
            updatePositionDto.Orientation = "bottom";
            
            await _roomRepository.UpdateUserPosition(userId, link, ClientId, updatePositionDto);
            var users = await _roomRepository.ListUsersPosition(link);
            
            Console.WriteLine("Sending to client.... Users: " + users.Count + "");
            await Clients.Group(link).SendAsync($"update-user-list", new { Users=users });
            await Clients.OthersInGroup(link).SendAsync($"add-user", new {  User=ClientId });
            Console.WriteLine("Sent to client!");
        }
    }
}