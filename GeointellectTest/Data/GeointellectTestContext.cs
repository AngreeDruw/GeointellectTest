#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GeointellectTest.Models;

namespace GeointellectTest.Data
{
    public class GeointellectTestContext : DbContext
    {
        public GeointellectTestContext (DbContextOptions<GeointellectTestContext> options) : base(options)
        {
        }

        public DbSet<GeointellectTest.Models.PropertyModel> Property { get; set; }
    }
}
