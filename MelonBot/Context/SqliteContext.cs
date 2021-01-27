using MelonBot.Bots.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace MelonBot.Bots.Context
{
    public class SqliteContext : DbContext
    {
        public DbSet<Profile> Profile { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=SqliteDB.db");

    }
}
