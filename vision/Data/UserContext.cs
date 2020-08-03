using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Vision.Models;

namespace Vision.Data {

    public class UserContext : DbContext {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options) { }

        public DbSet<User> User { get; set; }
    }
}
