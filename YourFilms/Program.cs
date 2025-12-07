using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using YourFilms.Services;
using YourFilms.Services.Tmdb;

var builder = WebApplication.CreateBuilder(args);
var corsPolicy = "AllowFrontend";


builder.Services.AddControllers();

builder.Services.AddHttpClient<TmdbClient>();
builder.Services.AddScoped<TmdbApiService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<YourFilmsDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("YourFilmsContext") ?? 
    throw new InvalidOperationException("Connection string 'YourFilmsContext' not found.")));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Cache") ?? 
    throw new InvalidOperationException("Connection string 'Cache' not found.");
});

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(corsPolicy);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
