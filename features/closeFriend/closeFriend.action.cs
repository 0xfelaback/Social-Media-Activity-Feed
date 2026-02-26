using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class CloseFriendAction
{
    public static void UseCloseFriendActionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/users/{username}/close-friends", async (string username, CloseFriendRequest closeFriendRequest, SocialMediaDataContext context) =>
{
    var users = await context.Users.Where(u => u.UserName == closeFriendRequest.userName || u.UserName == username).Select(s => new
    {
        s.UserID,
        s.UserName,
    }).ToListAsync();

    var initiator = users.FirstOrDefault(u => u.UserName == closeFriendRequest.userName);
    var userToAdd = users.FirstOrDefault(u => u.UserName == username);
    if (initiator is null) return Results.NotFound("This account does not exist");
    if (userToAdd is null) return Results.NotFound("This user does not exist");
    if (initiator.UserName == userToAdd.UserName) return Results.Forbid();

    bool alreadyAdded = await context.CloseFriends.AnyAsync(b => b.AddingUserID == initiator.UserID && b.CloseFriendAccountId == userToAdd.UserID);
    if (alreadyAdded) return Results.Conflict("Account already added this user.");

    CloseFriend closeFriendAction = new CloseFriend
    {
        AddingUserID = initiator.UserID,
        CloseFriendAccountId = userToAdd.UserID,
    };
    await context.CloseFriends.AddAsync(closeFriendAction);
    await context.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();



        app.MapDelete("/api/users/{username}/close-friends", async (string username, [FromBody] CloseFriendRequest closeFriendRequest, SocialMediaDataContext context) =>
{
    var users = await context.Users.Where(u => u.UserName == closeFriendRequest.userName || u.UserName == username).Select(s => new
    {
        s.UserID,
        s.UserName,
    }).ToListAsync();

    var initiator = users.FirstOrDefault(u => u.UserName == closeFriendRequest.userName);
    var userToremove = users.FirstOrDefault(u => u.UserName == username);
    if (initiator is null) return Results.NotFound("This account does not exist");
    if (userToremove is null) return Results.NotFound("This user does not exist");
    if (initiator.UserName == userToremove.UserName) return Results.Forbid();
    CloseFriend? closeFriendRecord = await context.CloseFriends.Where(b => b.AddingUserID == initiator.UserID && b.CloseFriendAccountId == userToremove.UserID).FirstOrDefaultAsync();
    if (closeFriendRecord is null) return Results.Conflict("Account has not added this user");

    context.CloseFriends.Remove(closeFriendRecord);
    await context.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();
    }
}

public record CloseFriendRequest
{
    public required string userName { get; set; }
}