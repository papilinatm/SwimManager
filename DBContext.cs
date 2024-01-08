using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace SwimManager
{
    internal class SwimDB: DbContext
    {

        public DbSet<Swimmer> Swimmers { get; set; }
        public DbSet<Result>  Results { get; set; }

        public string DbPath { get; } = "swimmers.db";

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Result>().HasNoKey();
            modelBuilder.Entity<Swimmer>().HasMany(s => s.AllResults).WithOne();
        }
    }
}
