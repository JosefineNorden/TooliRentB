using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using TooLiRent.Core.Enums;
using TooLiRent.Core.Models;

namespace TooLiRent.Infrastructure.Data
{
    public class TooLiRentBDbContext : DbContext
    {
        public TooLiRentBDbContext(DbContextOptions<TooLiRentBDbContext> options) : base(options)
        {
        }

        public DbSet<Tool> Tools { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        public DbSet<RentalDetail> RentalDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            /// --- Relationer ---


            modelBuilder.Entity<RentalDetail>()
                .HasOne(rd => rd.Tool)
                .WithMany()
                .HasForeignKey(rd => rd.ToolId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<RentalDetail>()
                .HasOne(rd => rd.Rental)
                .WithMany(r => r.RentalDetails)
                .HasForeignKey(rd => rd.RentalId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Tool>()
                .HasOne(t => t.Category)
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Rental>()
                .HasOne(r => r.Customer)
                .WithMany()
                .HasForeignKey(r => r.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);




            // --- Categories ---
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Drilling", Description = "Tools for drilling", CreatedAt = new DateTime(2025, 05, 01), UpdatedAt = new DateTime(2025, 05, 02) },
                new Category { Id = 2, Name = "Cutting", Description = "Tools for cutting", CreatedAt = new DateTime(2025, 05, 01), UpdatedAt = new DateTime(2025, 05, 02) },
                new Category { Id = 3, Name = "Grinding", Description = "Tools for grinding", CreatedAt = new DateTime(2025, 05, 01), UpdatedAt = new DateTime(2025, 05, 02) },
                new Category { Id = 4, Name = "Sanding", Description = "Tools for sanding", CreatedAt = new DateTime(2025, 05, 01), UpdatedAt = new DateTime(2025, 05, 02) },
                new Category { Id = 5, Name = "Detailing", Description = "Tools for Detailing", CreatedAt = new DateTime(2025, 05, 01), UpdatedAt = new DateTime(2025, 05, 02) },
                new Category { Id = 6, Name = "Air Tools", Description = "Air Compressor tools", CreatedAt = new DateTime(2025, 05, 01), UpdatedAt = new DateTime(2025, 05, 02) },
                new Category { Id = 7, Name = "Painting", Description = "Tools for painting", CreatedAt = new DateTime(2025, 05, 01), UpdatedAt = new DateTime(2025, 05, 02) }
            );

            // --- Tools ---
            modelBuilder.Entity<Tool>().HasData(
                new Tool
                {
                    Id = 1,
                    Name = "Cordless Drill",
                    Price = 25,
                    Description = "A powerful cordless drill for all your DIY needs.",
                    Stock = 10,
                    CatalogNumber = "DRL-1001",
                    CreatedAt = new DateTime(2025, 05, 01),
                    UpdatedAt = new DateTime(2025, 05, 02),
                    Status = ToolStatus.Available,
                    CategoryId = 1
                },
                new Tool
                {
                    Id = 2,
                    Name = "Circular Saw",
                    Price = 30,
                    Description = "High-precision circular saw for wood and metal.",
                    Stock = 5,
                    CatalogNumber = "SAW-2002",
                    CreatedAt = new DateTime(2025, 02, 14),
                    UpdatedAt = new DateTime(2025, 02, 25),
                    Status = ToolStatus.Available,
                    CategoryId = 2
                },
                new Tool
                {
                    Id = 3,
                    Name = "Hammer Drill",
                    Price = 28,
                    Description = "Heavy-duty hammer drill for concrete and masonry.",
                    Stock = 7,
                    CatalogNumber = "HDR-3003",
                    CreatedAt = new DateTime(2025, 02, 01),
                    UpdatedAt = new DateTime(2025, 02, 10),
                    Status = ToolStatus.Available,
                    CategoryId = 1
                },
                new Tool
                {
                    Id = 4,
                    Name = "Angle Grinder",
                    Price = 22,
                    Description = "Versatile angle grinder for cutting and grinding.",
                    Stock = 8,
                    CatalogNumber = "AGR-4004",
                    CreatedAt = new DateTime(2025, 02, 01),
                    UpdatedAt = new DateTime(2025, 02, 19),
                    Status = ToolStatus.Available,
                    CategoryId = 3
                },
                new Tool
                {
                    Id = 5,
                    Name = "Jigsaw",
                    Price = 18,
                    Description = "Compact jigsaw for intricate cutting tasks.",
                    Stock = 6,
                    CatalogNumber = "JSG-5005",
                    CreatedAt = new DateTime(2025, 01, 01),
                    UpdatedAt = new DateTime(2025, 01, 16),
                    Status = ToolStatus.Available,
                    CategoryId = 2
                },
                new Tool
                {
                    Id = 6,
                    Name = "Power Sander",
                    Price = 20,
                    Description = "Electric sander for smooth finishing.",
                    Stock = 9,
                    CatalogNumber = "SND-6006",
                    CreatedAt = new DateTime(2024, 08, 10),
                    UpdatedAt = new DateTime(2024, 08, 15),
                    Status = ToolStatus.Available,
                    CategoryId = 4
                },
                new Tool
                {
                    Id = 7,
                    Name = "Rotary Tool",
                    Price = 15,
                    Description = "Multi-purpose rotary tool for detailed work.",
                    Stock = 12,
                    CatalogNumber = "RTY-7007",
                    CreatedAt = new DateTime(2025, 01, 02),
                    UpdatedAt = new DateTime(2025, 01, 10),
                    Status = ToolStatus.Available,
                    CategoryId = 5
                },
                new Tool
                {
                    Id = 8,
                    Name = "Table Saw",
                    Price = 40,
                    Description = "Large table saw for professional carpentry.",
                    Stock = 3,
                    CatalogNumber = "TBS-8008",
                    CreatedAt = new DateTime(2025, 01, 01),
                    UpdatedAt = new DateTime(2025, 01, 05),
                    Status = ToolStatus.Available,
                    CategoryId = 2
                },
                new Tool
                {
                    Id = 9,
                    Name = "Air Compressor",
                    Price = 35,
                    Description = "Portable air compressor for pneumatic tools.",
                    Stock = 4,
                    CatalogNumber = "AIR-9009",
                    CreatedAt = new DateTime(2024, 04, 01),
                    UpdatedAt = new DateTime(2024, 10, 01),
                    Status = ToolStatus.Available,
                    CategoryId = 6
                },
                new Tool
                {
                    Id = 10,
                    Name = "Paint Sprayer",
                    Price = 27,
                    Description = "Efficient paint sprayer for large surfaces.",
                    Stock = 5,
                    CatalogNumber = "PNT-1010",
                    CreatedAt = new DateTime(2024, 05, 01),
                    UpdatedAt = new DateTime(2024, 07, 01),
                    Status = ToolStatus.Available,
                    CategoryId = 7
                },
                new Tool
                {
                    Id = 12,
                    Name = "Air Compressor",
                    Price = 35,
                    Description = "Portable air compressor for pneumatic tools.",
                    Stock = 1,
                    CatalogNumber = "AIR-9010",
                    CreatedAt = new DateTime(2024, 04, 01),
                    UpdatedAt = new DateTime(2024, 10, 01),

                    Status = ToolStatus.Broken,
                    CategoryId = 6
                });

            /// --- Customers ---

            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    Name = "Anna Andersson",
                    Email = "anna@example.com",
                    PhoneNumber = "0701234561",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 01),
                    UpdatedAt = new DateTime(2024, 07, 01)
                },
                new Customer
                {
                    Id = 2,
                    Name = "Björn Berg",
                    Email = "bjorn@example.com",
                    PhoneNumber = "0701234562",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 02),
                    UpdatedAt = new DateTime(2024, 07, 02)
                },
                new Customer
                {
                    Id = 3,
                    Name = "Cecilia Carlsson",
                    Email = "cecilia@example.com",
                    PhoneNumber = "0701234563",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 03),
                    UpdatedAt = new DateTime(2024, 07, 03)
                },
                new Customer
                {
                    Id = 4,
                    Name = "David Dahl",
                    Email = "david@example.com",
                    PhoneNumber = "0701234564",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 04),
                    UpdatedAt = new DateTime(2024, 07, 04)
                },
                new Customer
                {
                    Id = 5,
                    Name = "Eva Ek",
                    Email = "eva@example.com",
                    PhoneNumber = "0701234565",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 05),
                    UpdatedAt = new DateTime(2024, 07, 05)
                },
                new Customer
                {
                    Id = 6,
                    Name = "Filip Fors",
                    Email = "filip@example.com",
                    PhoneNumber = "0701234566",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 06),
                    UpdatedAt = new DateTime(2024, 07, 06)
                },
                new Customer
                {
                    Id = 7,
                    Name = "Greta Gustavsson",
                    Email = "greta@example.com",
                    PhoneNumber = "0701234567",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 07),
                    UpdatedAt = new DateTime(2024, 07, 07)
                },
                new Customer
                {
                    Id = 8,
                    Name = "Henrik Hall",
                    Email = "henrik@example.com",
                    PhoneNumber = "0701234568",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 08),
                    UpdatedAt = new DateTime(2024, 07, 08)
                },
                new Customer
                {
                    Id = 9,
                    Name = "Ida Isaksson",
                    Email = "ida@example.com",
                    PhoneNumber = "0701234569",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 09),
                    UpdatedAt = new DateTime(2024, 07, 09)
                },
                new Customer
                {
                    Id = 10,
                    Name = "Johan Jönsson",
                    Email = "johan@example.com",
                    PhoneNumber = "0701234570",
                    Status = CustomerStatus.Active,
                    CreatedAt = new DateTime(2024, 05, 10),
                    UpdatedAt = new DateTime(2024, 07, 10)
                });

            /// --- Rentals ---

            modelBuilder.Entity<Rental>().HasData(
                new Rental
                {
                    Id = 1,
                    CustomerId = 1,
                    StartDate = new DateTime(2025, 09, 01),
                    EndDate = new DateTime(2025, 09, 05),
                    CreatedAt = new DateTime(2025, 09, 01),
                    UpdatedAt = new DateTime(2025, 09, 01),
                    IsReturned = false
                },
                new Rental
                {
                    Id = 2,
                    CustomerId = 2,
                    StartDate = new DateTime(2025, 09, 03),
                    EndDate = new DateTime(2025, 09, 06),
                    CreatedAt = new DateTime(2025, 09, 03),
                    UpdatedAt = new DateTime(2025, 09, 03),
                    IsReturned = false
                },
                new Rental
                {
                    Id = 3,
                    CustomerId = 3,
                    StartDate = new DateTime(2025, 09, 07),
                    EndDate = new DateTime(2025, 09, 10),
                    CreatedAt = new DateTime(2025, 09, 07),
                    UpdatedAt = new DateTime(2025, 09, 07),
                    IsReturned = false
                },
                new Rental
                {
                    Id = 4,
                    CustomerId = 4,
                    StartDate = new DateTime(2025, 09, 08),
                    EndDate = new DateTime(2025, 09, 12),
                    CreatedAt = new DateTime(2025, 09, 08),
                    UpdatedAt = new DateTime(2025, 09, 08),
                    IsReturned = false
                },
                new Rental
                {
                    Id = 5,
                    CustomerId = 5,
                    StartDate = new DateTime(2025, 09, 10),
                    EndDate = new DateTime(2025, 09, 15),
                    CreatedAt = new DateTime(2025, 09, 10),
                    UpdatedAt = new DateTime(2025, 09, 10),
                    IsReturned = false
                }
            );


            /// --- RentalDetails ---

            modelBuilder.Entity<RentalDetail>().HasData(
                new RentalDetail
                {
                    Id = 1,
                    RentalId = 1,
                    ToolId = 1,
                    Quantity = 2,
                    CreatedAt = new DateTime(2024, 06, 01),
                    UpdatedAt = new DateTime(2024, 06, 02)
                },
                new RentalDetail
                {
                    Id = 2,
                    RentalId = 1,
                    ToolId = 3,
                    Quantity = 1,
                    CreatedAt = new DateTime(2024, 06, 01),
                    UpdatedAt = new DateTime(2024, 06, 02)
                },
                new RentalDetail
                {
                    Id = 3,
                    RentalId = 2,
                    ToolId = 2,
                    Quantity = 1,
                    CreatedAt = new DateTime(2024, 06, 05),
                    UpdatedAt = new DateTime(2024, 06, 06)
                },
                new RentalDetail
                {
                    Id = 4,
                    RentalId = 3,
                    ToolId = 5,
                    Quantity = 3,
                    CreatedAt = new DateTime(2024, 06, 10),
                    UpdatedAt = new DateTime(2024, 06, 11)
                },
                new RentalDetail
                {
                    Id = 5,
                    RentalId = 3,
                    ToolId = 4,
                    Quantity = 1,
                    CreatedAt = new DateTime(2024, 06, 10),
                    UpdatedAt = new DateTime(2024, 06, 11)
                }
            );
        }
    }
}
