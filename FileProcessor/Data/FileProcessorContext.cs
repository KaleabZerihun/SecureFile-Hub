using FileProcessor.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace FileProcessor.Data
{
    public class FileProcessorContext : IdentityDbContext<IdentityUser>
    {
        //where we do database interaction

        public FileProcessorContext(DbContextOptions<FileProcessorContext> options) : base(options) { }
        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // One-to-Many: One User has Many Files
            modelBuilder.Entity<Item>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade); // When a user is deleted, delete their files
        }
    }
}
