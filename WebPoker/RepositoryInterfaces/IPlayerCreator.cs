using WebPoker.Models;

namespace WebPoker.RepositoryInterfaces;

public interface IPlayerCreator
{
    Task<PlayerResponse> CreateAsync(Player player);
}
