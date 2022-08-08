using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ObsidianToVuePress.Domain;

namespace ObsidianToVuePress.Context
{
    public class ObsidianFileInfoContext : DbContext
    {
        public DbSet<ObsidianFileInfo> Files { get; set; }
        public string DbPath { get; set; }

        public ObsidianFileInfoContext(string dbPath = "obsidianDb.db") => DbPath = dbPath;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite($"Data Source={DbPath}");
        }
    }
}
