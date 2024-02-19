using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SwimManager
{
    internal class SwimDB : DbContext
    {
        public SwimDB(string dbPath)
        {
            if (string.IsNullOrEmpty(dbPath))
                throw new ArgumentNullException(nameof(dbPath));


            DbPath = dbPath + ".db";
            Database.EnsureCreated();
        }

        public DbSet<Swimmer> Swimmers { get; set; }
        public DbSet<Result> Results { get; set; }
        public string DbPath { get; }

        public void MergeSwimmers(IEnumerable<Swimmer> new_swimmers)
        {
            var exists = Swimmers.Include(s => s.AllResults).ToList();
            Swimmer.MergeSwimmers(exists, new_swimmers);
            SaveChanges();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Result>().HasNoKey();
            modelBuilder.Entity<Swimmer>().HasMany(s => s.AllResults).WithOne();
        }
    }
}
