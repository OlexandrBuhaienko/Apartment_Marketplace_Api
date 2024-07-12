using Apartment_Marketplace_API.Data;
using Apartment_Marketplace_API.Middleware;
using Apartment_Marketplace_API.Models.Domain;
using Apartment_Marketplace_API.Models.DTO;
using Apartment_Marketplace_API.Repositories.Implementation;
using Apartment_Marketplace_API.Repositories.Interface;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace Apartment_Marketplace_API
{
    public class Program
    {
        
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
                .AddNegotiate();

            //builder.Services.AddAuthorization(options =>
            //{
            //    // By default, all incoming requests will be authorized according to the default policy.
            //    options.FallbackPolicy = options.DefaultPolicy;
            //});

            builder.Services.AddDbContext<ApiDbContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            builder.Services.AddScoped<IApartmentRepository, ApartmentRepository>();
            builder.Services.AddExceptionHandler<ApartmentExceptionHandler>();
            var app = builder.Build();
            //app.UseStatusCodePagesWithReExecute("/Errors/{0}");
            //app.UseExceptionHandler();
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionHandler = context.RequestServices.GetRequiredService<ApartmentExceptionHandler>();
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (exceptionFeature?.Error != null)
                    {
                        await exceptionHandler.TryHandleAsync(context, exceptionFeature.Error, context.RequestAborted);
                    }
                });
            });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            //app.UseAuthorization();
            IApartmentRepository _apartmentRepository = app.Services.CreateScope().ServiceProvider
                .GetRequiredService<IApartmentRepository>();

            //POST : https://localhost:7081/api/apartments
            //Route for creating new apartment 
            app.MapPost("/apartments", async ([FromBody] CreateApartmentRequestDto request) =>
            {
                //Map DTO to Domain model
                var apartment = new Apartment
                {
                    Name = request.Name,
                    Rooms = request.Rooms,
                    Price = request.Price,
                    Description = request.Description
                };
                try
                {
                    await _apartmentRepository.CreateAsync(apartment);
                }
                catch (BadHttpRequestException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                
                catch (JsonException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                //Domain model to DTO 
                var response = new ApartmentDto
                {
                    Name = request.Name,
                    Rooms = request.Rooms,
                    Price = request.Price,
                    Description = request.Description
                };
                return Results.Ok(response);
            })
            .WithName("AddApartments")
            .WithOpenApi();
            //.RequireAuthorization();


            //GET : https://localhost:7081/api/apartments
            app.MapGet("/apartments", async (int? rooms, string price) =>
            {
                var apartments = await _apartmentRepository.GetAllAsync();

                if (rooms.HasValue)
                {
                    apartments = apartments.Where(a => a.Rooms == rooms.Value).ToList();
                }

                if (price?.ToLower() == "asc")
                {
                    apartments = apartments.OrderBy(a => a.Price).ToList();
                }
                else if( price?.ToLower() == "desc")
                {
                    apartments = apartments.OrderByDescending(a => a.Price).ToList();
                }

                var response = apartments.Select(a => new ApartmentDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    Rooms = a.Rooms,
                    Price = a.Price,
                    Description = a.Description
                }).ToList();

                //Map Domain model to the DTO 
                //var response = new List<ApartmentDto>();
                //foreach (var appartment in apartments)
                //{
                //    response.Add(new ApartmentDto
                //    {
                //        Id = apartments.Id,
                //        Name = apartments.Name,
                //        Rooms = apartments.Rooms,
                //        Price = apartments.Price,
                //        Description = apartments.Description,
                //    });
                //}
                return Results.Ok(response);
            })
            .WithName("GetAllApartments")
            .WithOpenApi();
            // .RequireAuthorization();

            //GET : https://localhost:7081/api/apartments/{id}

            app.MapGet("/apartments/{id}", async ([FromRoute] Guid id) =>
            {
                var existingApartment = await _apartmentRepository.GetById(id);
                if (existingApartment is null)
                {
                    return Results.NotFound();
                }

                //Creating DTO of the apartment by ID 
                var response = new ApartmentDto
                {
                    Id = existingApartment.Id,
                    Name = existingApartment.Name,
                    Rooms = existingApartment.Rooms,
                    Price = existingApartment.Price,
                    Description = existingApartment.Description
                };
                //Returning the response with DTO
                return Results.Ok(response);
            })
            .WithName("GetApartmentsById")
            .WithOpenApi();
            //.RequireAuthorization();


            //PUT : https://localhost:7081/api/apartments/{id}
            app.MapPut("/apartments/{id}", async ([FromRoute] Guid id, UpdateApartmentRequestDto request) =>
            {
                //Convert DTO to Domain model
                var apartment = new Apartment
                {
                    Id = id,
                    Name = request.Name,
                    Rooms = request.Rooms,
                    Price = request.Price,
                    Description = request.Description
                };
                try
                {
                    apartment = await _apartmentRepository.UpdateAsync(apartment);
                }
                catch (BadHttpRequestException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch(ArgumentException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }

                if (apartment == null)
                {
                    return Results.NotFound();
                }

                //Convert Domain model to DTO

                var response = new ApartmentDto
                {
                    Id = apartment.Id,
                    Name = apartment.Name,
                    Rooms = apartment.Rooms,
                    Price = apartment.Price,
                    Description = apartment.Description
                };
                return Results.Ok(response);
            })
            .WithName("UpdateApartmentsById")
            .WithOpenApi();
            //.RequireAuthorization();


            //DELETE : https://localhost:7081/api/apartments/{id}
            app.MapDelete("/apartments/{id}", async ([FromRoute] Guid id) =>
            {
                var apartment = await _apartmentRepository.DeleteAsync(id);
                if (apartment == null)
                {
                    return Results.NotFound();
                }
                // Convert Domain model to DTO
                var response = new ApartmentDto
                {
                    Id = apartment.Id,
                    Name = apartment.Name,
                    Rooms = apartment.Rooms,
                    Price = apartment.Price,
                    Description = apartment.Description
                };
                return Results.Ok(response);
            })
            .WithName("DeleteApartmentsById")
            .WithOpenApi();
            //.RequireAuthorization();

            app.Run();
        }
    }
}
