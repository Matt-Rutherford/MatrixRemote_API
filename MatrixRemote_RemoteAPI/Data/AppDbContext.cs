using MatrixRemote_RemoteAPI.Models;
using MatrixRemote_RemoteAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;


namespace MatrixRemote_RemoteAPI.Data
{
    public class AppDbContext : DbContext
    {
        protected readonly IConfiguration _configuration;
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseNpgsql(_configuration.GetConnectionString("WebApiDatabase"));
        }

        public DbSet<Remote> Remotes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Remote>().HasData(
                new Remote()
                {
                    Id = 1,
                    Font = "font",
                    ImageUrl = "https://images.pexels.com/photos/998641/pexels-photo-998641.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500",
                    Message = "message from space"
                },
                new Remote() // New entry
                {
                    Id = 2, // Ensure the Id is unique
                    Font = "anotherFont",
                    ImageUrl = "https://images.pexels.com/photos/123456/pexels-photo-123456.jpeg?auto=compress&cs=tinysrgb&dpr=1&w=500",
                    Message = "another message from space"
                });
        }
    }
}
