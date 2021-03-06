﻿using Microsoft.EntityFrameworkCore;
using NGK3.Models;

namespace WebApi.Context
{
    public class WeatherContext : DbContext
    {

        public DbSet<Place> Place { get; set; }
        public DbSet<WeatherReading> WeatherReading { get; set; }
        public DbSet<User> User { get; set; }
        public WeatherContext()
        {
            
        }

        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {
            
        }

        


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=WeatherReadings;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeatherReading>(entity =>
            {
                entity.HasOne(w => w.Place)
                    .WithMany(p => p.WeatherReadings)
                    .HasForeignKey(w => w.PlaceId);
            });

            modelBuilder.Entity<Place>(entity =>
            {
                entity.HasIndex(p => p.Name).IsUnique();
            });
        }

        

    }
}