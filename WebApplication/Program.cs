using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Repository;
using Repository.Entities;
using Repository.Interfaces;
using Repository.Repositories;
using AutoMapper;
using Service.Dto;
using Service.Interface;
using Service.Services;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using ConsoleApp1.Models;

var builder = WebApplication.CreateBuilder(args);

// --- 1. הוספת שירותים למערכת (DI Container) ---
builder.Services.AddControllers();

// הגדרת Swagger עם תמיכה ב-JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "נא להזין את הטוקן בלבד (ללא המילה Bearer)"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

// --- תיקון ה-CORS המעודכן ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        // וודאי שזו הכתובת המדויקת של ה-React (כולל הפורט 5175)
        policy.WithOrigins("http://localhost:5177")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// הגדרת JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

// הגדרת תלותיות (Dependency Injection)
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IContext, TandO>();
builder.Services.AddScoped<IRepository<User>, UserRepository>();
builder.Services.AddScoped<UserIService, UserService>();
builder.Services.AddScoped<IRepository<Producer>, ProducerRepository>();
builder.Services.AddScoped<IService<ProducerDto>, ProducerService>();
builder.Services.AddScoped<IRepository<Hall>, HallRepository>();
builder.Services.AddScoped<IService<HallDto>, HallService>();
builder.Services.AddScoped<IRepository<HallSeat>, HallSeatRepository>();
builder.Services.AddScoped<HallSeatIRepository, HallSeatRepository>();
builder.Services.AddScoped<IService<HallSeatDto>, HallSeatService>();
builder.Services.AddScoped<HallSeatIService, HallSeatService>();
builder.Services.AddScoped<EventIRepository, EventRepository>();
builder.Services.AddScoped<EventIService, EventService>();
builder.Services.AddScoped<IRepository<OrderDetail>, OrderDetailRepository>();
builder.Services.AddScoped<OrderDetailIRepository, OrderDetailRepository>();
builder.Services.AddScoped<OrderDetailIService, OrderDetailService>();
builder.Services.AddHostedService<ExpireCartService>();

// הגדרת AutoMapper
builder.Services.AddAutoMapper(cfg =>

{

    cfg.AddProfile<MyMapper>();

});

var app = builder.Build();

// --- 2. הגדרת ה-Pipeline (סדר הפעולות קריטי!) ---

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("AllowReactApp");
app.UseStaticFiles();

app.UseRouting();



app.UseHttpsRedirection();
app.UseAuthentication(); // אימות
app.UseAuthorization();  // הרשאות

app.MapControllers();

app.Run();
