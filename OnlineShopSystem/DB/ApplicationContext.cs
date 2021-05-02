using Microsoft.EntityFrameworkCore;

namespace OnlineShopSystem.DB
{
    public class ApplicationContext : DbContext
    {
        //public DbSet<Credit> CreditSet { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=OnlineShopSystem;Username=postgres;Password=admin;SearchPath=dbo");
        }
    }
}
