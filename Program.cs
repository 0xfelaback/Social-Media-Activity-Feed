using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Query.Internal;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SocialMediaDataContext>(options => options.UseSqlite("Data Source = social.db"));
builder.Services.AddScoped<IPasswordHasher<string>, PasswordHasher<string>>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
jwtOptions => jwtOptions.TokenValidationParameters = new TokenValidationParameters
{
    //The parameters required to consider a JWT valid
    ValidateIssuer = true, //validate server that generates the token
    ValidateAudience = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Issuer"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty))
}
);
builder.Services.AddSingleton<ITokenProvider, TokenProvider>();
builder.Services.AddAuthorization();
var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => "Hello World!");

app.MapGet(AuthExtension.GetUserEndpoint, (int id, SocialMediaDataContext context) =>
{
    User? storeduser = context.Users.Find(id);
    return storeduser is null ? Results.NotFound() : Results.Ok(storeduser);
});

app.MapGet("/api/posts/{id}", async (long id, SocialMediaDataContext context) =>
{
    var postToView = await context.Posts.Where(p => p.PostID == id).Select(p => new { p.Initiator.UserName }).FirstOrDefaultAsync();
    //if (postToView is null) return Results.NotFound("This post does not exist");
    return postToView is null ? Results.NotFound() : Results.Ok(postToView);
}).RequireAuthorization();

/*app.MapGet(" /api/posts/{id}", async (long id, SocialMediaDataContext context) =>
{
    //post + post_media + array[comments]
    var result = context.Posts.Where().FirstOrDefault();
});*/






app.UseAuth();
app.UseFollowActionEndpoints();
app.UseFollowListEndpoints();
app.UseBlockActionEndpoints();
app.UseCloseFriendActionEndpoints();
app.UsePostActionEndpoints();
app.Run();




