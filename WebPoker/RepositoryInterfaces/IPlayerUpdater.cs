using WebPoker.Models;
namespace WebPoker.RepositoryInterfaces;
public interface IPlayerUpdater
{
    Task<Player?> FindForUpdateAsync(int id);
    Task SaveChangesAsync();
}
