using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vision.Data {

    public class PageContext : DbContext {
        public PageContext(DbContextOptions<PageContext> options)
            : base(options) { }

        public DbSet<Vision.Models.Page> Page { get; set; }
    }
}
