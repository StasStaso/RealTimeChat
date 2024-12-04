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
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _chatContext;
        private readonly TokenService _tokenService;
        private IHubContext<RealTimeChatHub, IRealTimeChatClient> _hubContext;

        public AccountController(ApplicationDbContext chatContext,TokenService tokenService, 
            IHubContext<RealTimeChatHub, IRealTimeChatClient> hubContext)
        {
            _chatContext = chatContext;
            _tokenService = tokenService;
            _hubContext = hubContext;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto, CancellationToken cancellationToken) 
        {
            var user = await _chatContext.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username 
                && u.Password == dto.Password, cancellationToken);

            if (user is null)
            {
                return BadRequest("Incorrect credentials");
            }

            return Ok(GenerateToken(user));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto, CancellationToken cancellationToken) 
        {
            var usernameExists = await _chatContext.Users
                .AsNoTracking()
                .AnyAsync(u => u.Username == dto.Username, cancellationToken);

            if (usernameExists)
            {
                return BadRequest($"[{nameof(dto.Username)}] already exists");
            }

            var user = new User
            {
                Username = dto.Username,
                AddedOn = DateTime.Now,
                Name = dto.Name,
                Password = dto.Password,
            };

            await _chatContext.Users.AddAsync(user, cancellationToken);
            await _chatContext.SaveChangesAsync();

            await _hubContext.Clients.All.UserConnected(new UserDto(user.Id, user.Name));

            return Ok(GenerateToken(user));
        }

        private AuthResponseDto GenerateToken(User user)
        {
            var token = _tokenService.GenerateJWT(user);
            return new AuthResponseDto(new UserDto(user.Id, user.Name), token);
        }
    }
}
