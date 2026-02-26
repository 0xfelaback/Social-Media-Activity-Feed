using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public static class PostAction
{
    public static void UsePostActionEndpoints(this WebApplication app)
    {
        app.MapPost("/api/posts", async (PostRequest postRequest, SocialMediaDataContext context) =>
{
    //User? initiator = await context.Users.Where(u => u.UserName == postRequest.userName).FirstOrDefaultAsync();
    var creator = await context.Users.Where(u => u.UserName == postRequest.userName).Select(s => new
    {
        s.UserID,
        isAccountOwner = s.UserName == postRequest.userName
    }
    ).FirstOrDefaultAsync();
    if (creator is null) return Results.NotFound("This account does not exist");
    if (!creator.isAccountOwner) return Results.Conflict("Post cannot be made on an account by another");
    List<PostMedia> postMedias = new List<PostMedia>();
    for (int i = 0; i < postRequest.PostMediasLinkInputs.Count; i++)
    {
        PostMedia media = new PostMedia
        {
            MediaURL = postRequest.PostMediasLinkInputs[i].mediaURL,
            MediaType = (PostMedia.Media_Type?)postRequest.PostMediasLinkInputs[i].mediaType
        };
        postMedias.Add(media);
    }
    Post newPost = new Post
    {
        InitiatorID = creator.UserID,
        Caption = postRequest.caption,
        CreatedAt = DateTime.Now,
        LikeCount = default,
        PostMediasLinks = postMedias,
        isDeleted = default
    };

    await context.Posts.AddAsync(newPost);
    await context.SaveChangesAsync();
    return Results.Created();
}).RequireAuthorization();


        app.MapDelete("/api/posts/{id}", async (long id, [FromBody] PostDeleteRequest postDeleteRequest, SocialMediaDataContext context) =>
        {
            long initiatorID = await context.Users.Where(u => u.UserName == postDeleteRequest.userName).Select(i => i.UserID).FirstOrDefaultAsync();
            if (initiatorID < 1) return Results.NotFound("This account does not exist");
            Post? postToDelete = await context.Posts.Where(p => p.PostID == id).FirstOrDefaultAsync();
            if (postToDelete is null) return Results.NotFound("This post does not exist");
            if (initiatorID != postToDelete.InitiatorID) return Results.Conflict("This user does not own the post");


            //soft delete 
            postToDelete.isDeleted = true;
            postToDelete.DeletedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();


        app.MapPost("/api/posts/{id}/like", async (long id, PostLikeRequest postLikeRequest, SocialMediaDataContext context) =>
        {
            Post? postToLike = context.Posts.FirstOrDefault(p => p.PostID == id);
            if (postToLike is null) return Results.NotFound("This post does not exist");
            var result = await context.Users.Where(u => u.UserName == postLikeRequest.userName).Select(s => new

            {
                LikerId = s.UserID,
                isBlocked = s.AccountBlockedBy.Any(b => b.BlockingUserID == s.UserID && b.BlockedAccountId ==
                postToLike.InitiatorID) ||
                s.AccountBlockedBy.Any(b => b.BlockingUserID == postToLike.InitiatorID && b.BlockedAccountId == s.UserID),
                alreadyLiked = s.UserLikedPosts.Any(l => l.LikerID == s.UserID && l.PostID == postToLike.PostID)
            }
            ).FirstOrDefaultAsync();
            if (result is null) return Results.NotFound("This post does not exist");
            if (result.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");
            if (result.alreadyLiked) return Results.Conflict("The user already liked post");

            PostLike newPostLike = new PostLike
            {
                LikerID = result.LikerId,
                PostID = postToLike.PostID,
                CreatedAt = DateTime.Now
            };
            Notification newNotification = new Notification
            {
                ReceivingUserID = postToLike.InitiatorID,
                InitaiatorID = result.LikerId,
                NotificationType = Notification.NotifType.Like,
                CreatedAt = DateTime.Now
            };
            postToLike.LikeCount++;
            await context.Notifications.AddAsync(newNotification);
            await context.PostLikes.AddAsync(newPostLike);
            await context.SaveChangesAsync();
            return Results.Created();
        }).RequireAuthorization();

        app.MapDelete("/api/posts/{id}/like", async (long id, [FromBody] PostLikeRequest postLikeRequest, SocialMediaDataContext context) =>
        {
            Post? postToUnlike = context.Posts.FirstOrDefault(p => p.PostID == id);
            if (postToUnlike is null) return Results.NotFound("This post does not exist");
            var result = await context.Users.Where(u => u.UserName == postLikeRequest.userName).Select(s => new
            {
                likerId = s.UserID,
                isBlocked = s.AccountBlockedBy.Any(b => b.BlockingUserID == s.UserID && b.BlockedAccountId ==
                postToUnlike.InitiatorID ||
                s.AccountBlockedBy.Any(b => b.BlockingUserID == postToUnlike.InitiatorID && b.BlockedAccountId == s.UserID)),
                alreadyLiked = s.UserLikedPosts.Any(l => l.LikerID == s.UserID && l.PostID == postToUnlike.PostID)
            }).FirstOrDefaultAsync();
            if (result is null) return Results.NotFound("This account does not exist");
            if (result.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");
            if (!result.alreadyLiked) return Results.Conflict("The user has not liked post");

            using var transaction = context.Database.BeginTransaction();
            try
            {
                await context.PostLikes.Where(l => l.LikerID == result.likerId && l.PostID == postToUnlike.PostID).ExecuteDeleteAsync();
                postToUnlike.LikeCount--;
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return Results.NoContent();
        }).RequireAuthorization();

        app.MapPost("/api/posts/{id}/comment", async (int id, PostCommentRequest postCommentRequest, SocialMediaDataContext context) =>
{

    var result = await context.Posts.Where(p => p.PostID == id).Select(p => new
    {
        Post = p,
        PostOwnerId = p.InitiatorID,
        commenterID = context.Users.First(u => u.UserName == postCommentRequest.userName).UserID,
    })
    .Select(s =>
        new
        {
            PostId = s.Post.PostID,
            s.commenterID,
            IsBlocked = s.Post.Initiator.AccountBlockedBy.Any(b => b.BlockingUserID == s.commenterID && b.BlockedAccountId == s.PostOwnerId) || s.Post.Initiator.AccountBlockedBy.Any(b => b.BlockingUserID == s.PostOwnerId && b.BlockedAccountId == s.commenterID)
        }
    )
    .FirstOrDefaultAsync();

    if (result is null) return Results.NotFound("This post does not exist");
    if (result.commenterID < 1) return Results.NotFound("This account does not exist");


    if (result.IsBlocked) return Results.NotFound("Action cannot be completed due to a block.");

    Comment newComment = new Comment
    {
        PostID = result.PostId,
        CommenterID = result.commenterID,
        CommentText = postCommentRequest.commentText,
        LikeCount = default,
        CreatedAt = DateTime.Now,
        isDeleted = default
    };
    Notification notificationAction = new Notification
    {
        ReceivingUserID = result.PostId,
        InitaiatorID = result.commenterID,
        NotificationType = Notification.NotifType.Comment,
        CreatedAt = DateTime.Now
    };

    await context.Comments.AddAsync(newComment);
    await context.Notifications.AddAsync(notificationAction);
    await context.SaveChangesAsync();
    return Results.Created();
}).RequireAuthorization();


        app.MapDelete("/api/posts/{id}/comment", async (int id, [FromBody] PostCommentRequest postCommentRequest, SocialMediaDataContext context) =>
        {
            long commenterID = await context.Users
            .Where(u => u.UserName == postCommentRequest.userName)
            .Select(u => u.UserID)
            .FirstOrDefaultAsync();
            if (commenterID < 1) return Results.NotFound("This account does not exist");

            var result = await context.Posts.Where(u => u.PostID == id).Select(p => new
            {
                PostID = p.PostID,
                PostOwnerID = p.InitiatorID,

                IsBlocked = p.Initiator.AccountBlockedBy.Any(b => b.BlockedAccountId == p.InitiatorID && b.BlockingUserID == commenterID) ||
                p.Initiator.BlockedAccounts.Any(b => b.BlockedAccountId == commenterID && b.BlockingUserID == p.InitiatorID)
            }).FirstOrDefaultAsync();

            /*var result = await context.Users.Where(u => u.UserName == postCommentRequest.userName).Select(u => new
            {
                Subquery processes - intensive task
                commenterID = u.UserID,
                postToComment = context.Posts.Where(p => p.PostID == id).Select(p => new { p.PostID, p.InitiatorID }).FirstOrDefault(),
                IsBlocked = context.BlockedAccounts.Any(b => (b.BlockedAccountId == u.UserID && b.BlockingUserID == context.Posts.Where(p => p.PostID == id).Select(p => p.InitiatorID).FirstOrDefault()) || (b.BlockedAccountId == context.Posts.Where(p => p.PostID == id).Select(p => p.InitiatorID).FirstOrDefault() && b.BlockingUserID == u.UserID))
            }).FirstOrDefaultAsync();*/

            if (result is null) return Results.NotFound("This account does not exist");
            if (result.IsBlocked) return Results.NotFound("Action cannot be completed due to a block.");

            Comment? commentAction = await context.Comments.Where(c => c.CommenterID == commenterID && c.PostID == result.PostID).FirstOrDefaultAsync();
            if (commentAction is null) return Results.Conflict("This comment does not exist");

            commentAction.isDeleted = true;
            commentAction.DeletedAt = DateTime.Now;

            context.Comments.Remove(commentAction);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();


        app.MapPost("/api/posts/{id}/save", async (long id, PostSaveRequest postSaveRequest, SocialMediaDataContext context) =>
{
    long saverID = await context.Users.Where(u => u.UserName == postSaveRequest.userName).Select(s => s.UserID).FirstOrDefaultAsync();
    if (saverID < 1) return Results.NotFound("This account does not exist");

    var result = await context.Posts.Where(p => p.PostID == id).Select(s => new
    {
        s.PostID,
        s.InitiatorID,
        isBlocked = s.Initiator.AccountBlockedBy.Any(b => b.BlockedAccountId == s.InitiatorID && b.BlockingUserID == saverID) ||
        s.Initiator.AccountBlockedBy.Any(b => saverID == s.InitiatorID && b.BlockingUserID == b.BlockedAccountId),
        isAlreadySaved = s.Saves.Any(i => i.SaverID == saverID && i.PostID == s.PostID)
    }).FirstOrDefaultAsync();

    if (result is null) return Results.NotFound("This post does not exist");
    if (result.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");
    if (result.isAlreadySaved) return Results.Conflict("The user already saved post");

    SavedPost newPostSave = new SavedPost
    {
        SaverID = saverID,
        PostID = result.PostID
    };

    await context.SavedPosts.AddAsync(newPostSave);
    await context.SaveChangesAsync();
    return Results.Created();
}).RequireAuthorization();

        app.MapDelete("/api/posts/{id}/save", async (long id, [FromBody] PostSaveRequest postSaveRequest, SocialMediaDataContext context) =>
        {
            long unSaverID = await context.Users.Where(u => u.UserName == postSaveRequest.userName).Select(s => s.UserID).FirstOrDefaultAsync();
            if (unSaverID < 1) return Results.NotFound("This account does not exist");

            var result = await context.Posts.Where(p => p.PostID == id).Select(p => new
            {
                p.PostID,
                p.InitiatorID,
                isBlocked = p.Initiator.AccountBlockedBy.Any(b => b.BlockedAccountId == p.InitiatorID && b.BlockingUserID == unSaverID) ||
                p.Initiator.AccountBlockedBy.Any(b => unSaverID == p.InitiatorID && b.BlockingUserID == b.BlockedAccountId)
            }).FirstOrDefaultAsync();
            if (result is null) return Results.NotFound("This post does not exist");
            if (result.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");

            SavedPost? savedRecordToDelete = await context.SavedPosts.Where(p => p.PostID == result.PostID && p.SaverID == unSaverID).FirstOrDefaultAsync();
            if (savedRecordToDelete is null) return Results.Conflict("The user did not save this post");

            context.SavedPosts.Remove(savedRecordToDelete);
            await context.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();

    }
}
