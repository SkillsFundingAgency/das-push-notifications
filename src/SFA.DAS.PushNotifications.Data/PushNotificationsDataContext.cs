﻿using Microsoft.EntityFrameworkCore;
using SFA.DAS.PushNotifications.Model.Entities;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Data
{
    [ExcludeFromCodeCoverage]
    public class PushNotificationsDataContext : DbContext, IPushNotificationsDataContext
    {
        public PushNotificationsDataContext(DbContextOptions<PushNotificationsDataContext> options) : base(options) { }
        public DbSet<ApplicationClient> ApplicationClients => Set<ApplicationClient>();
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationClient>()
           .HasIndex(e => e.Endpoint)
           .IsUnique();
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(PushNotificationsDataContext).Assembly);
        }
    }
}