using Microsoft.AspNetCore.SignalR;
using RealTimeChat.Shared;
using RealTimeChat.Shared.DTOs;

namespace RealTimeChat.Server.Hubs
{
    public class RealTimeChatHub : Hub<IRealTimeChatClient>, IRealTimeChatServer
    {
        private static IDictionary<int, UserDto> _onlineUsers = new Dictionary<int, UserDto>();

        public RealTimeChatHub()
        {
            
        }

        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        public async Task SetUserOnline(UserDto user) 
        {
            await Clients.Caller.OnlineUsersList(_onlineUsers.Values);
            if (!_onlineUsers.ContainsKey(user.Id)) 
            {
                _onlineUsers.Add(user.Id, user);
                await Clients.Others.UserIsOnline(user.Id);
            }
        }
    }
}
