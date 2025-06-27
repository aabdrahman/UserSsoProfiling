using System.Text;
using LoggerService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Profiling.Api;
using Profiling.Api.Context;
using Profiling.Api.Contracts;
using Profiling.Api.Repository;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File(
        "Logs/log-.txt", 
        rollingInterval: RollingInterval.Day,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss}||{Level:u3}] [{ClassName}].[{MethodName}] - {Message:lj}{NewLine}{Exception}"
    )
    .CreateLogger();

// Add services to the container.

builder.Services.AddSingleton<DataContext>();

builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyHeader();
    });
});

builder.Services.AddAuthentication(options =>
{
   
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(opts =>
{
    var jwtSettingsConfig = builder.Configuration.GetSection("JwTSettings");

    opts.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
        ValidIssuer = jwtSettingsConfig["ValidIssuer"],
        ValidAudience = jwtSettingsConfig["ValidAudience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt-Secret"]!))
    };
});

builder.Services.AddScoped<IRepositoryManager, RepositoryManger>();

builder.Services.AddControllers();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "User Profiling", Description = "User Profiling System API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your Token here",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    };

    options.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                },
                Name = "Bearer"
            },
            new List<string>()
        }
    };

    options.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

//Configure the HTTP request pipeline.

app.UseSwagger(c => {
    c.RouteTemplate = "UserProfiling/swagger/{documentname}/swagger.json";
});
app.UseSwaggerUI(c => {
    c.SwaggerEndpoint("/UserProfiling/swagger/v1/swagger.json", "UserProfling");
    c.RoutePrefix = "UserProfiling/swagger";
    
});

app.UseExceptionHandler(opts => { });

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
