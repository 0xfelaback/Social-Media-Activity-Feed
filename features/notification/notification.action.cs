using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

public static class NotificationAction
{
    public static void UseNotificationActionEndpoints(this WebApplication app)
    {
        app.MapGet("/api/notifications", async ([FromBody] NotificationRequest notificationRequest, SocialMediaDataContext context) =>
{
    //TODO - include commentText, postLikes for each notification of the types. requires TPH/TPT

    var result = await context.Users.Where(u => u.UserName == notificationRequest.userName).Select(s => new
    {
        s.UserID,
        Notifications = s.NotificationsReceived.Select(sa => new
        {
            receivingUserID = sa.ReceivingUserID,
            sa.NotificationType,
            sa.InitaiatorID,
            initiatorUsername = sa.Initaiator.UserName,
            pfp = sa.Initaiator.ProfileImage_MediaUrl,
            sa.CreatedAt,
        })
        //commentText = context.Comments.Where(p => p.Post.InitiatorID == s.UserID).Select(s => s.CommentText).ToList()
    }).AsNoTracking().FirstOrDefaultAsync();

    if (result is null) return Results.NotFound("No notifications were found");

    List<NotificationResponse> response = new List<NotificationResponse> { };
    foreach (var item in result.Notifications)
    {
        NotificationResponse notification = new NotificationResponse
        {
            initiatorId = item.InitaiatorID,
            initiatorName = item.initiatorUsername,
            notifType = item.NotificationType.ToString(),
            initiatorProfileImageUrl = item.pfp ?? "https://defaults.com/default.png",
            CreatedAt = item.CreatedAt,
            //CommentText = item.commentText.FirstOrDefault() ?? null,
            //likedPostID = default
        };
        response.Add(notification);
    }

    return Results.Ok(response);
}).RequireAuthorization();

    }
}

public record NotificationRequest
{
    public required string userName { get; set; }
}
public record NotificationResponse
{
    public required long initiatorId { get; set; }
    public required string initiatorName { get; set; }
    public required string notifType { get; set; }
    public required string initiatorProfileImageUrl { get; set; }
    public required DateTime CreatedAt { get; set; }
    public string? CommentText { get; set; }
    public long? likedPostID { get; set; }
}