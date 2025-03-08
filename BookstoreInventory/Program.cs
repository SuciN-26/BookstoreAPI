using BookstoreInventory.Caching;
using BookstoreInventory.Data;
using BookstoreInventory.DTOs;
using BookstoreInventory.Repositories;
using BookstoreInventory.Utils;
using BookstoreInventory.Validators;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Serilog;
using System.Text.Json.Serialization;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day, retainedFileCountLimit: 5)
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

//Configure Logging with serilog
builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseLazyLoadingProxies().UseSqlServer(config.GetConnectionString("DefaultConnection"))
);

builder.Services.AddControllers(options =>
{
    options.Filters.Add<SuccessResultFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddMemoryCache();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// registration dependencies 
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.Decorate<IBookRepository, CachedBookService>();

builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IValidator<CreateAuthorDto>, CreateAuthorValidator>();
builder.Services.AddScoped<IValidator<CreateBookDto>, CreateBookValidator>();

builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlerMiddleware>();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
