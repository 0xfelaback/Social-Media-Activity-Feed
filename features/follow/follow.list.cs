using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

public static class FollowList
{
    public static void UseFollowListEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users/{username}/followers", async (string username, SocialMediaDataContext context) =>
{
    var user = await context.Users.Where(f => f.UserName.ToLower() == username.ToLower()).Select(u => new { u.FollowersCount, u.FollowersAccounts }).AsNoTracking().FirstOrDefaultAsync();
    if (user is null) return Results.NotFound("This account does not exist");
    var result = new FollowerListResponse
    {
        followersCount = user.FollowersCount,
        Followers = user.FollowersAccounts,
    };
    return Results.Ok(result);
}).RequireAuthorization();

        app.MapGet("/api/users/{username}/following", async (string username, SocialMediaDataContext context) =>
        {
            var user = await context.Users.Where(f => f.UserName.ToLower() == username.ToLower()).Select(u => new { u.FollowingAccounts, u.FollowingCount, }).AsNoTracking().FirstOrDefaultAsync();
            if (user is null) return Results.NotFound("This account does not exist");
            var result = new FollowingListResponse
            {
                followingCount = user.FollowingCount,
                Following = user.FollowingAccounts
            };
            return Results.Ok(result);
        }).RequireAuthorization();

    }
}


public record FollowerListResponse
{
    public int followersCount { get; set; }
    public ICollection<Follow> Followers { get; set; } = null!;

}
public record FollowingListResponse
{
    public int followingCount { get; set; }
    public ICollection<Follow> Following { get; set; } = null!;
}