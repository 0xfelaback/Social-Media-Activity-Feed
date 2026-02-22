using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SocialMediaDataContext>(options => options.UseSqlite("Data Source = Database.db"));

var app = builder.Build();



app.MapGet("/", () => "Hello World!");

app.Run();
