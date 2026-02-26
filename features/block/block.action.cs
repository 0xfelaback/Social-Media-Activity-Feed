using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class BlockAction
{
    public static void UseBlockActionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/users/{username}/block", async (string username, BlockRequest blockRequest, SocialMediaDataContext context) =>
{
    var users = await context.Users.Where(u => u.UserName == username || u.UserName == blockRequest.userName).Select(s => new
    {
        s.UserID,
        s.UserName,
    }).ToListAsync();

    var initiator = users.FirstOrDefault(s => s.UserName == blockRequest.userName);
    var userToBlock = users.FirstOrDefault(s => s.UserName == username);
    if (initiator is null) return Results.NotFound("This account does not exist");
    if (userToBlock is null) return Results.NotFound("This user does not exist");
    if (initiator.UserName == userToBlock.UserName) return Results.Forbid();
    bool alreadyBlocked = await context.BlockedAccounts.AnyAsync(b => b.BlockingUserID == initiator.UserID && b.BlockedAccountId == userToBlock.UserID);
    if (alreadyBlocked) return Results.Conflict("Account already blocked this user");

    BlockedAccount blockAction = new BlockedAccount
    {
        BlockingUserID = initiator.UserID,
        BlockedAccountId = userToBlock.UserID,
    };

    await context.BlockedAccounts.AddAsync(blockAction);
    await context.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();


        app.MapDelete("/api/users/{username}/block", async (string username, [FromBody] BlockRequest unBlockRequest, SocialMediaDataContext context) =>
{
    var users = await context.Users.Where(u => u.UserName == username || u.UserName == unBlockRequest.userName).Select(s => new
    {
        s.UserID,
        s.UserName,
    }).ToListAsync();
    var initiator = users.FirstOrDefault(u => u.UserName == unBlockRequest.userName);
    var userToUnBlock = users.FirstOrDefault(u => u.UserName == username);

    if (initiator is null) return Results.NotFound("This account does not exist");
    if (userToUnBlock is null) return Results.NotFound("This user does not exist");
    if (initiator.UserName == userToUnBlock.UserName) return Results.Forbid();

    BlockedAccount? blockRecord = await context.BlockedAccounts.Where(b => b.BlockingUserID == initiator.UserID && b.BlockedAccountId == userToUnBlock.UserID).FirstOrDefaultAsync();
    if (blockRecord is null) return Results.Conflict("Account does not block this user");

    context.BlockedAccounts.Remove(blockRecord);
    await context.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization();
    }
}

public record BlockRequest
{
    public required string userName { get; set; }
}