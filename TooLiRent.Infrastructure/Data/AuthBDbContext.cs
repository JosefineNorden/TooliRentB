using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TooLiRent.Infrastructure.Data
{
    public class AuthBDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AuthBDbContext(DbContextOptions<AuthBDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
            base.OnModelCreating(modelbuilder);

            // Seed roles
            var adminRole = new IdentityRole("Admin")
            {
                Id = "1",
                NormalizedName = "ADMIN",
                ConcurrencyStamp = "STATIC-ADMIN-ROLE-CONCURRENCYSTAMP"

            };
            var memberRole = new IdentityRole("Member")
            {
                Id = "2",
                NormalizedName = "MEMBER",
                ConcurrencyStamp = "STATIC-MEMBER-ROLE-CONCURRENCYSTAMP"
            };
            modelbuilder.Entity<IdentityRole>().HasData(adminRole, memberRole);

        }
    }
}
