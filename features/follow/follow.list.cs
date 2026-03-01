using Microsoft.EntityFrameworkCore;

public static class FollowList
{
    public static void UseFollowListEndpoints(this WebApplication app)
    {
        app.MapGet("/api/users/{username}/followers", async (string username, SocialMediaDataContext context) =>
{
    var user = await context.Users.Where(f => f.UserName.ToLower() == username.ToLower()).Select(u => new { u.FollowersCount, u.FollowersAccounts }).Select(sa => new
    {
        count = sa.FollowersAccounts.Count,
        //sa.FollowersAccounts,
        followerAccountsIds = sa.FollowersAccounts.Select(f => new { f.FollowedUserID }).ToList()
    }).AsNoTracking().FirstOrDefaultAsync();
    if (user is null) return Results.NotFound("This account does not exist");
    var followers = await context.Users.Where(f => user.followerAccountsIds.Contains(new { FollowedUserID = f.UserID })).ToListAsync();

    List<FollowerInfo> followerInfo = new List<FollowerInfo>();

    foreach (var item in followers)
    {
        FollowerInfo info = new FollowerInfo
        {
            UserId = item.UserID,
            userName = item.UserName,
            firstName = item.FirstName,
            lastName = item.LastName,
            profileImage_MediaUrl = item.ProfileImage_MediaUrl ?? "https://defaults.com/default.png",
        };
        followerInfo.Add(info);
    }

    var result = new FollowerListResponse
    {
        followersCount = user.count,
        Followers = followerInfo,
    };
    return Results.Ok(result);
}).RequireAuthorization();

        app.MapGet("/api/users/{username}/following", async (string username, SocialMediaDataContext context) =>
        {
            var user = await context.Users.Where(f => f.UserName.ToLower() == username.ToLower()).Select(u => new { u.FollowingAccounts, u.FollowingCount, }).Select(sa => new
            {
                count = sa.FollowingAccounts.Count,
                followingAccountsIds = sa.FollowingAccounts.Select(f => new { f.FollowerID }).ToList(),
            }).AsNoTracking().FirstOrDefaultAsync();
            if (user is null) return Results.NotFound("This account does not exist");
            var followingAccs = await context.Users.Where(
f => user.followingAccountsIds.Contains(new { FollowerID = f.UserID })).ToListAsync();

            List<FollowerInfo> followingInfo = new List<FollowerInfo>();
            foreach (var item in followingAccs)
            {
                FollowerInfo info = new FollowerInfo
                {
                    UserId = item.UserID,
                    userName = item.UserName,
                    firstName = item.FirstName,
                    lastName = item.LastName,
                    profileImage_MediaUrl = item.ProfileImage_MediaUrl ?? "https://defaults.com/default.png",
                };
                followingInfo.Add(info);
            }

            var result = new FollowingListResponse
            {
                followingCount = user.count,
                Followers = followingInfo,
            };
            return Results.Ok(result);
        }).RequireAuthorization();

    }
}

public class FollowerInfo
{
    public long UserId { get; set; }
    public string userName { get; set; } = null!;
    public string firstName { get; set; } = null!;
    public string lastName { get; set; } = null!;
    public string? profileImage_MediaUrl { get; set; }
}
public record FollowerListResponse
{
    public int followersCount { get; set; }
    public ICollection<FollowerInfo> Followers { get; set; } = null!;

}
public record FollowingListResponse
{
    public int followingCount { get; set; }
    public ICollection<FollowerInfo> Followers { get; set; } = null!;
}