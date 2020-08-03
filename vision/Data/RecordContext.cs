using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vision.Models;

namespace Vision.Data {

    public class RecordContext : DbContext {
        public RecordContext(DbContextOptions<RecordContext> options)
            : base(options) { }

        public DbSet<Record> Record { get; set; }
    }
}
