using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class FollowActions
{
    public static void UseFollowActionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/users/{creatorUsername}/follow", async (string creatorUsername, FollowRequest followRequest, SocialMediaDataContext context) =>
{
    //Insert into Follows table; trigger a Notification.
    var users = await context.Users.Where(u => u.UserName == creatorUsername || u.UserName == followRequest.userName).Select(s => new
    {
        User = s,
        followResquestUserID = context.Users.FirstOrDefault(u => u.UserName == followRequest.userName).UserID,
    }
    ).Select(sa => new
    {
        sa.User.UserName,
        User = sa.User,
        isBlocked = sa.User.AccountBlockedBy.Any(b => b.BlockedAccountId == sa.User.UserID && b.BlockingUserID == sa.followResquestUserID || b.BlockedAccountId == sa.followResquestUserID && b.BlockingUserID == sa.User.UserID),
        alreadyFollow = sa.User.FollowingAccounts.Any(f => f.Followed.UserName == creatorUsername)

    }).ToListAsync();

    var initiator = users.FirstOrDefault(u => u.UserName == followRequest.userName);
    if (initiator is null) return Results.NotFound("This account does not exist");
    var followeduser = users.FirstOrDefault(u => u.UserName == creatorUsername);
    if (followeduser is null) return Results.NotFound("This user does not exist");
    if (initiator.UserName == followeduser.UserName) return Results.Forbid();

    if (initiator.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");
    if (followeduser.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");
    if (initiator.alreadyFollow) return Results.Conflict("Account already follows this user");

    Follow followAction = new Follow
    {
        FollowerID = initiator.User.UserID,
        FollowedUserID = followeduser.User.UserID,
    };
    Notification notificationAction = new Notification
    {
        ReceivingUserID = followeduser.User.UserID,
        InitaiatorID = initiator.User.UserID,
        NotificationType = Notification.NotifType.Follow,
        CreatedAt = DateTime.Now
    };

    followeduser.User.FollowersCount++;
    initiator.User.FollowingCount++;

    await context.Follows.AddAsync(followAction);
    await context.Notifications.AddAsync(notificationAction);
    await context.SaveChangesAsync();
    return Results.NoContent();

}).RequireAuthorization();

        app.MapDelete("/api/users/{creatorUsername}/follow", async (string creatorUsername, [FromBody] FollowRequest followRequest, SocialMediaDataContext context) =>
{
    var users = await context.Users.Where(u => u.UserName == creatorUsername || u.UserName == followRequest.userName).
    Select(s => new
    {
        User = s,
        status = new
        {
            isBlocked = s.AccountBlockedBy.Any(b => b.BlockedAccountId == s.UserID && b.BlockingUserID == context.Users.First(u => u.UserName == followRequest.userName).UserID ||
        b.BlockedAccountId == context.Users.First(u => u.UserName == followRequest.userName).UserID && b.BlockingUserID == s.UserID),
            alreadyFollow = s.FollowingAccounts.Any(f => f.Followed.UserName == creatorUsername)
        }
    }).ToListAsync();

    var initiator = users.FirstOrDefault(u => u.User.UserName == followRequest.userName);
    if (initiator is null) return Results.NotFound("This account does not exist");
    var followeduser = users.FirstOrDefault(u => u.User.UserName == creatorUsername);
    if (followeduser is null) return Results.NotFound("This user does not exist");
    if (initiator.User.UserName == followeduser.User.UserName) return Results.Forbid();

    if (initiator.status is null) return Results.Forbid();
    if (initiator.status.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");
    if (followeduser.status.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");
    if (!initiator.status.alreadyFollow) return Results.Conflict("Account does not follow this user.");

    // Atomicity
    using var transaction = context.Database.BeginTransaction();
    try
    {
        int rowDeleted = await context.Follows.Where(i => i.FollowerID == initiator.User.UserID && i.FollowedUserID == followeduser.User.UserID).ExecuteDeleteAsync();

        if (rowDeleted > 0)
        {
            followeduser.User.FollowersCount--;
            initiator.User.FollowingCount--;
            await context.SaveChangesAsync();
        }

        await transaction.CommitAsync();
    }
    catch (Exception)
    {
        await transaction.RollbackAsync();
        throw;
    }
    return Results.NoContent();
}).RequireAuthorization();
    }
}

public record FollowRequest
{
    public required string userName { get; set; }
}