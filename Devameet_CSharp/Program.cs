using Devameet_CSharp;
using Devameet_CSharp.Models;
using Devameet_CSharp.Repository;
using Devameet_CSharp.Repository.Impl;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Devameet_CSharp.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionstring = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<DevameetContext>(option => option.UseSqlServer(connectionstring));

builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
builder.Services.AddScoped<IMeetRepository, MeetRepositoryImpl>();
builder.Services.AddScoped<IRoomRepository, RoomRepositoryImpl>();
builder.Services.AddScoped<IMeetObjectRepository, MeetObjectRepositoryImpl>();

builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientPermission", policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:3000")
            .AllowCredentials();
    });
});

var chaveCriptografia = Encoding.ASCII.GetBytes(JWTKey.SecretKey);
builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(autenticacao =>
{
    autenticacao.RequireHttpsMetadata = false;
    autenticacao.SaveToken = true;
    autenticacao.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(chaveCriptografia),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// #region Websocket

// SocketIOServer server = new SocketIOServer(new SocketIOServerOption(9001));
//
// server.OnConnection((socket) =>
// {
//     Console.WriteLine("Client connected!");
//
//     socket.On("join", (data) => // IdUser, link
//     {
//
//         foreach (JToken token in data)
//         {
//             JoinSocketDto requestJson = token.ToObject<JoinSocketDto>();
//
//             Console.Write(requestJson.IdUser.ToString() + " - " + requestJson.Link); 
//
//         }
//
//         JoinSocketDto teste = new JoinSocketDto();
//
//         socket.Emit("lista de usuarios ativos", teste); // emite a lista de usu�rios conectados na room inclusive voc�
//
//         
//     });
//
//     socket.On("move", (data) => // IdUser, link ,x,y ,orientation
//     {
//         foreach (JToken token in data)
//         {
//             Console.Write(token + " ");
//         }
//         socket.Emit("lista de usuarios ativos", data); // emite a lista de usu�rios atualizados na room inclusive voc�
//     });
//
//     socket.On("handleDisconnect", (data) => // clientId 
//     {
//         foreach (JToken token in data)
//         {
//             Console.Write(token + " ");
//         }
//         // disconnecta o cliente da sala
//         socket.Dispose();
//     });
//
//     socket.On("toggl-mute-use", (data) => //IdUser, link, muted
//     {
//         foreach (JToken token in data)
//         {
//             Console.Write(token + " ");
//         }
//         socket.Emit("lista de usuarios ativos", data); // emite a lista de usu�rios conectados na room inclusive voc�
//     });
//
//     socket.On("call-user", (data) => //clientId, ClientIdToSend, RTCOffer
//     {
//         foreach (JToken token in data)
//         {
//             Console.Write(token + " ");
//         }
//         socket.Emit("lista de usuarios ativos", data); // emite a call-made para o client especifico com a offer e o seu id
//     });
//
//     socket.On("make-aswner", (data) => //clientId, ClientIdToSend, RTCAsnwer
//     {
//         foreach (JToken token in data)
//         {
//             Console.Write(token + " "); 
//         }
//         socket.Emit("lista de usuarios ativos", data); // emite a call-made para o client especifico com a offer e o seu id
//     });
//
//
//     socket.On(SocketIOEvent.DISCONNECT, () =>
//     {
//         Console.WriteLine("Client disconnected!");
//     });
//
// });
//
// #endregion Websocket

// server.Start();

// Console.WriteLine("Input /exit to exit program.");
// string line;

// while (!(line = Console.ReadLine())?.Trim()?.ToLower()?.Equals("/exit") ?? false)
// {
//     server.Emit("echo", line);
// }

var app = builder.Build();

//app.UseHttpsRedirection();

app.UseCors("ClientPermission");

app.UseAuthentication();

app.UseAuthorization();

app.UseWebSockets();



app.MapControllers();

app.MapHub<RoomHub>("/roomHub");

app.Run();
