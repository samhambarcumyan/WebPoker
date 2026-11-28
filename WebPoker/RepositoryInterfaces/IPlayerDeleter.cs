namespace WebPoker.RepositoryInterfaces;
public interface IPlayerDeleter
{
    Task<bool> DeleteAsync(int id);
}
