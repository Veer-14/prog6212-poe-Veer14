using Microsoft.EntityFrameworkCore;
using PROG6212_POE.Models;

namespace PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<User> Users { get; set; }
        public DbSet<Claim> Claims { get; set; }
        public DbSet<ClaimAttachment> ClaimAttachments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Claim>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserID)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClaimAttachment>()
                .HasOne(ca => ca.Claim)
                .WithMany(c => c.Attachments)
                .HasForeignKey(ca => ca.ClaimID)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
