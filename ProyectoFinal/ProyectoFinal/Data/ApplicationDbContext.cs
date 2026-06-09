using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace ProyectoFinal.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<CryptoCurrency> CryptoCurrencies { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Transaction>()
                .HasOne<User>()
                .WithMany()
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<CryptoCurrency>()
                .Property(c => c.CurrentPrice)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.CryptoAmount)
                .HasPrecision(18, 8);

            modelBuilder.Entity<Transaction>()
                .Property(t => t.Money)
                .HasPrecision(18, 2);
        }
    }
}