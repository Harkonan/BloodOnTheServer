using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string SessionId { get; set; }
        public ICollection<Player> Players { get; set; }
        public DateTime LastUsed { get; set; }
        public int Seats { get; set; }
    }

    public class Player
    {

        public Guid PlayerID { get; set; }
        public int PlayerSeat { get; set; }
        public Session Session { get; set; }
    }
}
