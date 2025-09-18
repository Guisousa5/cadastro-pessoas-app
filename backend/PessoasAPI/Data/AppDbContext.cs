using Microsoft.EntityFrameworkCore;
using PessoasAPI.Models;

namespace PessoasAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        
        public DbSet<Pessoa> Pessoas { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pessoa>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Cpf).IsRequired().HasMaxLength(11);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Cpf).IsUnique();
            });
        }
    }
}