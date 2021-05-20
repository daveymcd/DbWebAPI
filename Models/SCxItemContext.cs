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
    /// </summary>
    public class SCxItemContext: DbContext
    {
        /// <summary>
        ///     DbWebAPI.Models.SCxItemContext - Set Database context for SCxItem
        /// </summary>
        public SCxItemContext(DbContextOptions<SCxItemContext> options)
            : base(options)
        {
        }
        /// <summary>
        ///     DbWebAPI.Models.SCxItems - Document List (IList of SCxItem)
        /// </summary>
        public DbSet<SCxItem> SCxItems { get; set; }
    }
}
