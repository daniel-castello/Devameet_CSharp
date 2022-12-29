﻿namespace Devameet_CSharp.Models
{
    public class Room
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MeetId { get; set; }
        public string UserName { get; set; }
        public string Avatar { get; set; }
        public string ClientId { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Orientation { get; set; }
        public bool Muted { set; get; }
    }
}
