using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Data
{
    public class XcelAppDbContext : IdentityDbContext<XcelAppUser>
    {
        public XcelAppDbContext(DbContextOptions<XcelAppDbContext> options) : base(options)
        {

        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);

        //    builder.Entity<XcelAppUser>(entity =>
        //    {
        //        // Mark UserID as Identity column
        //        entity.Property(e => e.UserID).ValueGeneratedOnAdd();
        //    });
        //}
    }
}
