﻿using DatingApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<AppUser> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Message>()
                    .HasOne(x => x.Recipient)
                    .WithMany(x => x.MessagesReceived)
                    .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                  .HasOne(x => x.Sender)
                  .WithMany(x => x.MessagesSent)
                  .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
