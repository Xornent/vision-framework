using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Models;

namespace Vision.Data {

    public class CategoryContext : DbContext {
        public CategoryContext(DbContextOptions<CategoryContext> options) 
            : base(options) { }

        public DbSet<Category> Category { get; set; }
    }
}
