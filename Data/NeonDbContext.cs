using Microsoft.EntityFrameworkCore;
using NeonTest.Models;

namespace NeonTest.Data;

public class NeonDbContext : DbContext
{
  public NeonDbContext(DbContextOptions<NeonDbContext> opts) : base (opts)
  { }

  public DbSet<User> User { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
      modelBuilder.Entity<User>().ToTable("user");

      modelBuilder.Entity<User>().HasKey(u => u.Id);

      modelBuilder.Entity<User>()
        .Property(u => u.Id)
        .HasColumnName("id");

      modelBuilder.Entity<User>()
        .Property(u => u.Username)
        .HasColumnName("username");

      modelBuilder.Entity<User>()
        .Property(u => u.Email)
        .HasColumnName("email");

      modelBuilder.Entity<User>()
        .Property(u => u.Password)
        .HasColumnName("password");
  }
}
