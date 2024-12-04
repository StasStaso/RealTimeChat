using RealTimeChat.Shared.DTOs;

namespace RealTimeChat.Shared
{
    public interface IRealTimeChatClient
    {
        Task UserConnected(UserDto user);
        Task OnlineUsersList(IEnumerable<UserDto> users);
        Task UserIsOnline(int userId);

        Task MessageRecieved(MessageDto messageDto);
    }
}
