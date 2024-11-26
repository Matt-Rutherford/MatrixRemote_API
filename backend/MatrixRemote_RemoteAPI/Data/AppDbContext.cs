using MatrixRemote_RemoteAPI.Models;
using MatrixRemote_RemoteAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static System.Net.WebRequestMethods;


using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MatrixRemote_RemoteAPI.Models;
using static MatrixRemote_RemoteAPI.Models.MatrixEvent;

namespace MatrixRemote_RemoteAPI.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet for MatrixEvent
        public DbSet<MatrixEvent> Events { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed roles for Identity
            SeedRoles(modelBuilder);

            // Configure MatrixEvent entity
            modelBuilder.Entity<MatrixEvent>(entity =>
            {
                // Set up a primary key and additional constraints
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Content).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Type).HasConversion<string>(); // Store EventType as a string
                entity.OwnsOne(e => e.Location); // If GeoLocation is a complex type
                entity.OwnsOne(e => e.Color);    // If RgbColor is a complex type
            });

            // Seed data for MatrixEvent
            modelBuilder.Entity<MatrixEvent>().HasData(
                new MatrixEvent
                {
                    Id = Guid.NewGuid(),
                    Content = "Hello from Earth!",
                    Timestamp = DateTime.UtcNow,
                    Type = EventType.Text,
                    Location = new GeoLocation { Latitude = 37.7749, Longitude = -122.4194 },
                    Color = new RgbColor { R = 255, G = 0, B = 0 }
                },
                new MatrixEvent
                {
                    Id = Guid.NewGuid(),
                    Content = "Image from the stars",
                    Timestamp = DateTime.UtcNow,
                    Type = EventType.Image,
                    Location = new GeoLocation { Latitude = 40.7128, Longitude = -74.0060 },
                    Color = null // No color for images
                }
            );
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Name = "Admin",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedName = "ADMIN"
                },
                new IdentityRole
                {
                    Name = "User",
                    ConcurrencyStamp = Guid.NewGuid().ToString(),
                    NormalizedName = "USER"
                }
            );
        }
    }
}

