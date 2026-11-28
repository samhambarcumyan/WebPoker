namespace WebPoker.PlayersRepository;
using WebPoker.DataBaseContext;
using WebPoker.Models;
using WebPoker.Repository;
using WebPoker.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
public class PlayerRepository : IRepositoryReader<PlayerResponse>, IPlayerCreator, IPlayerUpdater, IPlayerDeleter
{
    private readonly PokerDbContext _context;

    public PlayerRepository(PokerDbContext context)
    {
        _context = context;
    }

    public Task<List<PlayerResponse>> GetAllAsync()
    {
        return _context.Players
            .Select(player => new PlayerResponse(player.Id, player.Nickname, player.Balance))
            .ToListAsync();
    }

    public async Task<PlayerResponse> CreateAsync(Player player)
    {
        _context.Players.Add(player);
        await _context.SaveChangesAsync();
        return new PlayerResponse(player.Id, player.Nickname, player.Balance);
    }

    public async Task<Player?> FindForUpdateAsync(int id)
    {
        return await _context.Players.FindAsync(id);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var player = await _context.Players.FindAsync(id);
        if (player is null)
            return false;

        _context.Players.Remove(player);
        await _context.SaveChangesAsync();
        return true;
    }
}
