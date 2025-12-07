using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using TooLiRent.Core.Interfaces;
using TooLiRent.Infrastructure.Data;
using TooLiRent.Infrastructure.Repositories;
using TooLiRent.Services.DTOs.RentalDTOs;
using TooLiRent.Services.DTOs.ToolDTOs;
using TooLiRent.Services.Interfaces;
using TooLiRent.Services.Mapping;
using TooLiRent.Services.Services;
using TooLiRent.Services.Validation;



namespace TooliRentB
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Database connection

            builder.Services.AddDbContext<TooLiRentBDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddDbContext<AuthBDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
                x => x.MigrationsAssembly("TooLiRent.Infrastructure")));

            // -- Identity -- 

            builder.Services
                .AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<AuthBDbContext>()
                .AddDefaultTokenProviders();

            // -- JWT Authentication --

            var jwtSection = builder.Configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"];
            var jwtIssuer = jwtSection["Issuer"];
            var jwtAudience = jwtSection["Audience"];

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtIssuer,
                        ValidAudience = jwtAudience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtKey!))
                    };
                });

            // --- Dependency Injections Repositories and Services ---

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IToolRepository, ToolRepository>();
            builder.Services.AddScoped<IToolService, ToolService>();

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            builder.Services.AddScoped<IRentalRepository, RentalRepository>();
            builder.Services.AddScoped<IRentalService, RentalService>();

            builder.Services.AddScoped<IAdminSummaryService, AdminSummaryService>();

            builder.Services.AddScoped<
                TooLiRent.Services.Interfaces.IAuthServiceB,
                TooLiRent.Services.Services.AuthServiceB>();

            // --- AutoMapper ---

            builder.Services.AddAutoMapper(cfg => { }, typeof(ToolProfile).Assembly);

            // --- Validators ---

            builder.Services.AddScoped<IValidator<ToolCreateDto>, ToolCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<ToolUpdateDto>, ToolUpdateDtoValidator>();

            builder.Services.AddScoped<IValidator<RentalCreateDto>, RentalCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<RentalUpdateDto>, RentalUpdateDtoValidator>();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TooLiRent API",
                    Version = "v1"
                });

                // Definiera Bearer / JWT i Swagger
                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Skriv: Bearer {din token}",

                    Reference = new OpenApiReference
                    {
                        Id = "Bearer",
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { jwtSecurityScheme, Array.Empty<string>() }
    });
            });




            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var sp = scope.ServiceProvider;
                var roleMgr = sp.GetRequiredService<RoleManager<IdentityRole>>();
                var userMgr = sp.GetRequiredService<UserManager<IdentityUser>>();

                SeedIdentityAsync(roleMgr, userMgr).GetAwaiter().GetResult();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }

        private static async Task SeedIdentityAsync(
            RoleManager<IdentityRole> roleMgr,
            UserManager<IdentityUser> userMgr)
        {
            // Roller
            foreach (var role in new[] { "Admin", "Member" })
            {
                if (!await roleMgr.RoleExistsAsync(role))
                    await roleMgr.CreateAsync(new IdentityRole(role));
            }

            // Admin
            const string adminEmail = "admin@toolirent.local";
            var admin = await userMgr.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                var u = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                await userMgr.CreateAsync(u, "Admin123!");
                await userMgr.AddToRoleAsync(u, "Admin");
            }

        }
    }
}