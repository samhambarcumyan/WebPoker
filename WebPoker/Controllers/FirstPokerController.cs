using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebPoker.Models;
using WebPoker.PatchRequests;
using WebPoker.PostRecord;
using WebPoker.Repository;
using WebPoker.RepositoryInterfaces;

namespace WebPoker.Controllers
{
    [Route("api/player")]
    [ApiController]
    public class FirstPokerController : ControllerBase
    {
        private readonly IRepositoryReader<PlayerResponse> _reader;
        private readonly IPlayerCreator _creator;
        private readonly IPlayerUpdater _updater;
        private readonly IPlayerDeleter _deleter;

        public FirstPokerController(
            IRepositoryReader<PlayerResponse> reader,
            IPlayerCreator creator,
            IPlayerUpdater updater,
            IPlayerDeleter deleter)
        {
            _reader = reader;
            _creator = creator;
            _updater = updater;
            _deleter = deleter;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlayers()
        {
            var players = await _reader.GetAllAsync();
            return Ok(players);
        }

        [HttpPost]
        public async Task<IActionResult> CreatePlayer([FromBody] Request request)
        {
            if (string.IsNullOrWhiteSpace(request.Nickname) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Invalid Nickname or Password");
            }

            var player = new Player
            {
                Nickname = request.Nickname.Trim()
            };

            var hasher = new PasswordHasher<Player>();
            player.Password = hasher.HashPassword(player, request.Password);

            var createdPlayer = await _creator.CreateAsync(player);
            return StatusCode(StatusCodes.Status201Created, createdPlayer);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePlayer(int id)
        {
            if (!await _deleter.DeleteAsync(id))
                return NotFound();

            return NoContent();
        }

        [HttpPatch("{id:int}/nickname")]
        public async Task<IActionResult> ChangeNickname(
            [FromRoute] int id,
            [FromBody] NicknamePatch request)
        {
            if (string.IsNullOrWhiteSpace(request.NewNickname))
                return BadRequest("Enter a nickname.");

            var player = await _updater.FindForUpdateAsync(id);
            if (player is null)
                return NotFound();

            player.Nickname = request.NewNickname.Trim();
            await _updater.SaveChangesAsync();

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

            var player = await _updater.FindForUpdateAsync(id);
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
            await _updater.SaveChangesAsync();

            return NoContent();
        }
    }
}