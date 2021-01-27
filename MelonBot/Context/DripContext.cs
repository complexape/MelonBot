using MelonBot.Bots.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelonBot.Bots.Context
{
    public class DripContext : DbContext
    {
        public DbSet<Drip> Drip { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=DripDB.db");

    }
}
