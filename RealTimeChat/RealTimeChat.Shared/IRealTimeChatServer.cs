using RealTimeChat.Shared.DTOs;

namespace RealTimeChat.Shared
{
    public interface IRealTimeChatServer
    {
        Task SetUserOnline(UserDto user);
    }
}
