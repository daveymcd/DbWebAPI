using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbWebAPI.Models
{
    /// <summary>
    ///     DbWebAPI.Models.SCxItemContext - DbContext
    ///     
    ///     Setup the SQL Server database context for the API.
    ///     Called from DbWebApi.Startup.cs
    /// </summary>
    public class SCxItemContext: DbContext
    {
        /// <summary>
        ///     DbWebAPI.Models.SCxItemContext - Set Database context for SCxItem
        /// </summary>
        public SCxItemContext(DbContextOptions<SCxItemContext> options) : base(options)
        {
        }
        /// <summary>DbWebAPI.Models.SCxItems - Document List (IList of SCxItem)</summary>
        public DbSet<SCxItem> SCxItems { get; set; }
        ///// <summary>All Documents in DTO subset</summary>
        //public DbSet<SCxItemDto> SCxItemsDto { get; set; }
        /// <summary>SC1: Deliveries In Documents</summary>
        public DbSet<SC1Item> SC1Items { get; set; }
        /// <summary>SC2: Cooler Checks Documents</summary>
        public DbSet<SC2Item> SC2Items { get; set; }
        /// <summary>SC3: Cooking Log Documents</summary>
        public DbSet<SC3Item> SC3Items { get; set; }
        /// <summary>SC4: Hot Holding Documents</summary>
        public DbSet<SC4Item> SC4Items { get; set; }
        /// <summary>SC9: Deliveries Out Documents</summary>
        public DbSet<SC9Item> SC9Items { get; set; }

        /// <summary>
        ///     DbWebAPI.Models.OnModelCreating (override)
        ///     
        ///     Invoked by EnsureCreated() call in DbWebAPI.Data.DbInitializer.Initialize()
        ///     Equate SCx Item to its Database table.
        /// </summary>
        /// <param name="modelBuilder">Context Model</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SCxItem>().ToTable("SCx");
            modelBuilder.Entity<SC1Item>().ToTable("SC1");
            modelBuilder.Entity<SC2Item>().ToTable("SC2");
            modelBuilder.Entity<SC3Item>().ToTable("SC3");
            modelBuilder.Entity<SC4Item>().ToTable("SC4");
            modelBuilder.Entity<SC9Item>().ToTable("SC9");
        }
    }
}
