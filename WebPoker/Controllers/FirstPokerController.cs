using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebPoker.DataBaseContext;
using WebPoker.Models;
using WebPoker.PostRecord;
using Microsoft.AspNetCore.Identity;
using WebPoker.PatchRequests;
namespace WebPoker.Controllers
{
    [Route("api/player")]
    [ApiController]
    public class FirstPokerController : ControllerBase
    {
        public FirstPokerController(PokerDbContext context)
        {
            _context = context;
        }
        private readonly PokerDbContext _context;
        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            var player = await _context.Players
                .Select(player => new { player.Id, player.Nickname, player.Balance })
                .ToListAsync();
            return Ok(player);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] Request request)
        {
            if (string.IsNullOrWhiteSpace(request.Nickname) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Invalid Nickname or Password");
            var player = new Player()
            {
                Nickname = request.Nickname.Trim()
            };

            var hasher = new PasswordHasher<Player>();
            player.Password = hasher.HashPassword(player, request.Password);
            _context.Players.Add(player);
            await _context.SaveChangesAsync();
            return StatusCode(StatusCodes.Status201Created, new
            {
                player.Id,
                player.Nickname,
                player.Balance
            });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePlayer(int Id)
        {
            var player = await _context.Players.FindAsync(Id);
            if (player == null)
                return NotFound();

            _context.Players.Remove(player);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPatch("{id:int}/nickname")]
        public async Task<IActionResult> ChangeNickname(
            [FromRoute] int id,
            [FromBody] NicknamePatch request)
        {
            if (string.IsNullOrWhiteSpace(request.NewNickname))
                return BadRequest("Enter a nickname.");

            var player = await _context.Players.FindAsync(id);
            if (player == null)
                return NotFound();

            player.Nickname = request.NewNickname.Trim();
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id:int}/password")]
        public async Task<IActionResult> ChangePassword(
            [FromRoute] int id,
            [FromBody] PasswordPatch request)
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("Enter both the current and new passwords.");
            }

            var player = await _context.Players.FindAsync(id);
            if (player is null)
                return NotFound();

            var hasher = new PasswordHasher<Player>();
            var storedHash = player.Password;

            if (string.IsNullOrEmpty(storedHash) ||
                hasher.VerifyHashedPassword(player, storedHash, request.CurrentPassword)
                    == PasswordVerificationResult.Failed)
            {
                return BadRequest("Invalid password");
            }

            player.Password = hasher.HashPassword(player, request.NewPassword);
            await _context.SaveChangesAsync();

            return NoContent();
        }


    }
}