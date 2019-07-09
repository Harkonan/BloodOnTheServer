using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BloodOnTheWeb.Models
{
    public class SessionContext : DbContext
    {
        public SessionContext(DbContextOptions<SessionContext> options)
            : base(options)
        { }

        public DbSet<Session> Sessions { get; set; }
        public DbSet<Player> Players  { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Session>().ToTable("Sessions");
            modelBuilder.Entity<Player>().ToTable("Players");
        }

    }

    public class Session{
        public Guid SessionId { get; set; }
        public ICollection<Player> Players { get; set; }
    }

    public class Player
    {
        public int PlayerID { get; set; }
        public Session Session { get; set; }
    }
}
