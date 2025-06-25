using LoggerService;
using Profiling.Api.Context;
using Profiling.Api.Contracts;
using Profiling.Api.Repository;
using Serilog;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext() 
                .WriteTo.Console()
                .WriteTo.Debug(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Debug, outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {ClassName}.{MethodName} - {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("Log/log-.txt", Serilog.Events.LogEventLevel.Verbose, fileSizeLimitBytes: 1_000_000,
                                rollingInterval: RollingInterval.Day, rollOnFileSizeLimit: true, buffered: true,
                                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {ClassName}.{MethodName} - {Message:lj}{NewLine}{Exception}")
                .CreateLogger();


// Add services to the container.
builder.Services.AddSingleton<DataContext>();

builder.Services.AddSingleton<ILoggerManager, LoggerManager>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policyBuilder =>
    {
        policyBuilder.AllowAnyOrigin();
        policyBuilder.AllowAnyMethod();
        policyBuilder.AllowAnyHeader();
    });
});
builder.Services.AddScoped<IRepositoryManager, RepositoryManger>();

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
