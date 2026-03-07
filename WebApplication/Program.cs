using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using ConsoleApp1.Models;
//using Common;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AutoMapper;
//using Service.Dto;
//using Service.Interface;
//using Service.Services;
using System.Text;
using Service.Dto;
using Service.Interface;
using Service.Services;
using System;
using Microsoft.EntityFrameworkCore;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//סווגר עם אבטחה
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "securityLessonWebApi", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Please enter your bearer token",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
});

//הגדרת התלויות 
//addscoped -לכל גולש יוצר את המופע
//addTrensient -בכל בקשה 
//addsingelton -אחד לכולם
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<IService<UserDto>, UserService>();
builder.Services.AddScoped<IRepository<Producer>, ProducerRepository>();
builder.Services.AddScoped<IService<ProducerDto>, ProducerService>();
builder.Services.AddScoped<IRepository<Hall>, HallRepository>();
builder.Services.AddScoped<IService<HallDto>, HallService>();
builder.Services.AddHostedService<ExpireCartService>();

builder.Services.AddScoped<IRepository<HallSeat>, HallSeatRepository>();
builder.Services.AddScoped<IService<HallSeatDto>, HallSeatService>();
builder.Services.AddScoped<IRepository<Event>, EventRepository>();
builder.Services.AddScoped<IService<EventDto>, EventService>();
builder.Services.AddScoped<IRepository<OrderDetail>, OrderDetailRepository>();
builder.Services.AddScoped<OrderDetailIService, OrderDetailService>();

//builder.Services.AddAutoMapper(typeof(MyMapper));

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<MyMapper>();
});

//builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//builder.Services.AddScoped<IContext,Database>();
//חיבור ל sql 
builder.Services.AddScoped<IContext, TandO>();
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
