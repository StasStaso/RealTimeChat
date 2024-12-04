using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using RealTimeChat.Server.Data;
using RealTimeChat.Server.Data.Entities;
using RealTimeChat.Server.Hubs;
using RealTimeChat.Shared;
using RealTimeChat.Shared.DTOs;

namespace RealTimeChat.Server.Controllers
{
    public class MessagesController : BaseController
    {
        private readonly ApplicationDbContext _chatContext;
        private readonly IHubContext<RealTimeChatHub, IRealTimeChatClient> _hubContext;

        public MessagesController(ApplicationDbContext chatContext, 
            IHubContext<RealTimeChatHub, IRealTimeChatClient> hubContext)
        {
            _chatContext = chatContext;
            _hubContext = hubContext;
        }

        [HttpPost("")]
        public async Task<IActionResult> SendMessage(MessageDto messageDto,
            CancellationToken cancellationToken)
        {
            if (messageDto.ToUserId <= 0 || string.IsNullOrWhiteSpace(messageDto.Message))
            { 
                return BadRequest(); 
            }

            var message = new Message
            {
                FromId = base.UserId,
                ToId = messageDto.ToUserId,
                Content = messageDto.Message,
                SentOn = DateTime.Now
            };

            await _chatContext.Messages.AddAsync(message, cancellationToken);

            if (await _chatContext.SaveChangesAsync(cancellationToken) > 0)
            {
                var responseMessageDto = new MessageDto(message.ToId, message.FromId,
                    message.Content, message.SentOn);

                await _hubContext.Clients.User(messageDto.ToUserId.ToString())
                    .MessageRecieved(responseMessageDto);

                return Ok();
            }
            else 
            {
                return StatusCode(500, "Unable to send message");
            }
        }

        [HttpGet("{otherUserId:int}")]
        public async Task<IEnumerable<MessageDto>> GetMessage(int otherUserId,
            CancellationToken cancellationToken)
        {
            var messages = await _chatContext.Messages
                            .AsNoTracking()
                            .Where(m =>
                                (m.FromId == otherUserId && m.ToId == UserId)
                                || (m.ToId == otherUserId && m.FromId == UserId)
                            )
                            .Select(m => new MessageDto(m.ToId, m.FromId, m.Content, m.SentOn))
                            .ToListAsync(cancellationToken);

            return messages;
        }
    }
}
