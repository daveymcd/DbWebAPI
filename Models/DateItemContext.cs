using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DbWebAPI.Models
{   
    public class ArcItemContext : DbContext
    {
        public ArcItemContext(DbContextOptions<ArcItemContext> options)
            : base(options)
        {
        }

        public DbSet<ArcItem> ArcItems { get; set; }
    }
}
