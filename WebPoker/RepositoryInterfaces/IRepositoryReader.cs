using Microsoft.AspNetCore.Mvc;

namespace WebPoker.Repository;

public interface IRepositoryReader<T>
{
    Task<List<T>> GetAllAsync();
}
