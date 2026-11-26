using Microsoft.EntityFrameworkCore;
using WebPoker.Models;
namespace WebPoker.DataBaseContext
{
    public class PokerDbContext :DbContext
    {
        public DbSet<Player> Players => Set<Player>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=LENOVO\\MSSQLSERVER02;Database=PokerDb;Trusted_Connection=True;TrustServerCertificate=True");
        }
    }
}
