using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SocialMediaDataContext>(options => options.UseSqlite("Data Source = social.db"));
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});
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

app.MapGet("/api/feed/{username}", async (string username, SocialMediaDataContext context, string? cursor, int limit = 10) =>
{
    DateTime? cursorTime = default;
    long? cursorId = default;
    if (!string.IsNullOrWhiteSpace(cursor))
    {
        var decodedCursor = Cursor.Decode(cursor);
        if (decodedCursor is null)
        {
            return Results.BadRequest();
        }
        cursorTime = decodedCursor.dateTime;
        cursorId = decodedCursor.lastId;
    }

    var followedIds = await context.Users.AsNoTracking().Where(u => u.UserName == username).SelectMany(s => s.FollowingAccounts.Select(f => f.FollowedUserID)).ToListAsync();

    var query = context.Posts.AsNoTracking().Where(p => followedIds.Contains(p.InitiatorID));

    if (cursorTime.HasValue && cursorId.HasValue)
    {
        query = query.Where(x => x.CreatedAt < cursorTime || x.CreatedAt == cursorTime && x.PostID <= cursorId);
    }
    var posts = await query.Select(s => new { s.PostID, s.InitiatorID, s.Caption, s.CreatedAt, s.LikeCount, s.PostMediasLinks, s.Comments, s.PostLikes })
    .OrderByDescending(p => p.CreatedAt)
    .ThenByDescending(p => p.PostID)
    .Take(limit + 1)
    .ToListAsync();

    if (posts is null) return Results.Conflict();
    var hasMore = posts.Count > limit;
    DateTime? nextDateTime = posts.Count > limit ? posts[^1].CreatedAt : null; //access from the last element in List
    long? nextId = posts.Count > limit ? posts[^1].PostID : null;
    if (hasMore) posts.RemoveAt(posts.Count - 1); //remove last element from list
    return Results.Ok(
        new
        {
            Posts = posts,
            Cursor = nextDateTime is not null && nextId is not null ? Cursor.Encode(nextDateTime.Value, nextId.Value) : null,
            HasMore = hasMore
        }
        );
}).RequireAuthorization();

app.UseAuth();
app.UseFollowActionEndpoints();
app.UseFollowListEndpoints();
app.UseBlockActionEndpoints();
app.UseCloseFriendActionEndpoints();
app.UsePostActionEndpoints();
app.UseNotificationActionEndpoints();
app.Run();

public sealed record Cursor(DateTime dateTime, long lastId)
{

    public static string Encode(DateTime dateTime, long lastId)
    {
        var cursor = new Cursor(dateTime, lastId);
        string json = JsonSerializer.Serialize(cursor);
        return Base64UrlTextEncoder.Encode(Encoding.UTF8.GetBytes(json));
    }
    public static Cursor? Decode(string? cursor)
    {
        if (string.IsNullOrWhiteSpace(cursor))
        {
            return null;
        }
        try
        {
            string json = Encoding.UTF8.GetString(Base64UrlTextEncoder.Decode(cursor));
            return JsonSerializer.Deserialize<Cursor>(json);
        }
        catch
        {
            return null;
        }
    }
}
