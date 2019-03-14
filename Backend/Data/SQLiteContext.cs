using Backend.Model.DB;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Data
{
    public class SQLiteContext : DbContext
    {
        public DbSet<mUser> sUser { get; set; }
        public DbSet<mDuty> sDuty { get; set; }
    }
}
