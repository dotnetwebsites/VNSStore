using System;
using System.Collections.Generic;
using System.Text;
using VNSStoreMgmt.Areas.Identity.Data;
using VNSStoreMgmt.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VNSStoreMgmt.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<Enquiry>()
            //    .HasIndex(u => new { u.PhoneNumber })
            //    .IsUnique();

            builder.Entity<ProductMaster>()
               .HasIndex(u => new { u.ProductCode })
               .IsUnique();

            builder.Entity<ProductMaster>()
               .HasIndex(u => new { u.ProductBarcode })
               .IsUnique();

            builder.Entity<ProductMaster>()
               .HasIndex(u => new { u.SerialNo })
               .IsUnique();
        }

        public DbSet<MailLibrary> MailLibraries { get; set; }
        public DbSet<ProductMaster> ProductMasters { get; set; }
        public DbSet<Accountability> Accountabilities { get; set; }

        public DbSet<BarcodeList> BarcodeLists { get; set; }
        public DbSet<BarcodeMaster> BarcodeMasters { get; set; }
    }
}
