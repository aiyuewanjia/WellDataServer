using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace WellDataService.Models
{
    public class WellDBContext : IdentityDbContext<ApplicationUser>
    {
        public WellDBContext(DbContextOptions<WellDBContext> options) : base(options)
        {

        }

        public DbSet<Device> devices { get; set; }

        public DbSet<DeviceData> deviceDatas { get; set; }
        
    }
}
