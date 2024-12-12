using Microsoft.EntityFrameworkCore;
using Store.Application.Services;
using Store.Infrastructure.Data;
using Store.Infrastructure.Repositories;
using Store.Domain.Interfaces;
using Microsoft.OpenApi.Models;

namespace Store.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adds the DbContext configured to use the InMemory database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("StoreDb"));

            // Registers services and repositories in the dependency injection container
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<IProductRepository, ProductRepository>();
            builder.Services.AddScoped<OrderService>();

            // Adds support for Swagger for API documentation
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Store API",
                    Version = "v1",
                    Description = "API for Orders and Products Management",
                    Contact = new OpenApiContact
                    {
                        Name = "Gabriel Silva",
                        Email = "gabriel.gbss@gmail.com",
                    }
                });
            });

            // Adds controllers
            builder.Services.AddControllers();

            var app = builder.Build();

            // Middleware configuration for the development environment
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // General middleware configuration
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            // Runs the application
            app.Run();
        }
    }
}
