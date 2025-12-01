using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System;
using TooliRent.Core.Interfaces;
using TooliRent.Infrastructure.Data;
using TooLiRent.Infrastructure.Repositories;
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

            // --- Dependency Injections Repositories and Services ---

            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<IToolRepository, ToolRepository>();
            builder.Services.AddScoped<IToolService, ToolService>();

            builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();

            // --- AutoMapper ---

            builder.Services.AddAutoMapper(cfg => { }, typeof(ToolProfile).Assembly);

            // --- Validators ---

            builder.Services.AddScoped<IValidator<ToolCreateDto>, ToolCreateDtoValidator>();
            builder.Services.AddScoped<IValidator<ToolUpdateDto>, ToolUpdateDtoValidator>();



            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
