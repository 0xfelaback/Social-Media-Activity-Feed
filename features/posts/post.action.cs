using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

public static class PostAction
{
    public static void UsePostActionEndpoints(this WebApplication app)
    {

        app.MapPost("/api/posts", async (PostRequest postRequest, SocialMediaDataContext context) =>
        {
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
            if (postToDelete.isDeleted == true) return Results.Forbid();

            //soft delete 
            postToDelete.isDeleted = true;
            postToDelete.DeletedAt = DateTime.Now;
            await context.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();

        app.MapPost("/api/posts/{id}/restore", async (long id, PostDeleteRequest postDeleteRequest, SocialMediaDataContext context) =>
        {
            long initiatorID = await context.Users.Where(u => u.UserName == postDeleteRequest.userName).Select(i => i.UserID).FirstOrDefaultAsync();
            if (initiatorID < 1) return Results.NotFound("This account does not exist");
            Post? postToDelete = await context.Posts.Where(p => p.PostID == id).FirstOrDefaultAsync();
            if (postToDelete is null) return Results.NotFound("This post does not exist");
            if (initiatorID != postToDelete.InitiatorID) return Results.Conflict("This user does not own the post");
            if (postToDelete.isDeleted != true) return Results.Forbid();

            postToDelete.isDeleted = false;
            postToDelete.DeletedAt = null;
            await context.SaveChangesAsync();
            return Results.NoContent();
        }).RequireAuthorization();

        app.MapGet("/api/posts/{id}", async (long id, [FromBody] PostViewRequest postViewRequest, SocialMediaDataContext context) =>
        {
            var result = context.Posts.Where(p => p.PostID == id).Select(s => new
            {
                postSaves = s.Saves,
                postOwnerId = s.InitiatorID,
                postId = s.PostID,
                postIsDeleted = s.isDeleted,
                postCreatedAt = s.CreatedAt,
                postLikeCount = s.LikeCount,
                postCaption = s.Caption,
                postComments = s.Comments,
                postMediaLinks = s.PostMediasLinks,
                postOwnerPfp = s.Initiator.ProfileImage_MediaUrl,
                postOwnerUserName = s.Initiator.UserName,
                postInitiatorAccountBlockedBy = s.Initiator.AccountBlockedBy,
                ViewerUserId = context.Users.Where(u => u.UserName == postViewRequest.userName).Select(s => s.UserID).FirstOrDefault(),
            }).Select(sa => new
            {
                PostOwnerPfp = sa.postOwnerPfp,
                PostOwnerUsername = sa.postOwnerUserName,
                sa.postCreatedAt,
                sa.postIsDeleted,
                sa.postLikeCount,
                sa.postCaption,
                comments = sa.postComments,
                media = sa.postMediaLinks,
                isBlocked = sa.postInitiatorAccountBlockedBy.Any(b =>
                    (b.BlockingUserID == sa.ViewerUserId && b.BlockedAccountId == sa.ViewerUserId) ||
                    (b.BlockingUserID == sa.ViewerUserId && b.BlockedAccountId == sa.ViewerUserId)),
                isPostSaved = sa.postSaves.Any(s => s.PostID == sa.postId && s.SaverID == sa.ViewerUserId)
            }).AsNoTracking().FirstOrDefault();
            if (result is null) return Results.NotFound("This post does not exist");
            if (result.postIsDeleted) return Results.NotFound("This post has been deleted");
            if (result.isBlocked) return Results.NotFound("Action cannot be completed due to a block.");

            List<Comment> commentsInPost = result.comments.ToList();
            List<PostMedia> mediaInPost = result.media.ToList();

            PostViewResponse postViewResponse = new PostViewResponse
            {
                postOwnerUserName = result.PostOwnerUsername,
                postOwnerPfpUrl = result.PostOwnerPfp ?? "http://default/defaultPfp.png",
                DateCreated = result.postCreatedAt,
                LikesCount = result.postLikeCount,
                postCaption = result.postCaption,
                postMedias = mediaInPost,
                postComments = commentsInPost,
                isSaved = result.isPostSaved
            };


            return Results.Ok(postViewResponse);
        });


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
        p.PostID,
        PostOwnerId = p.InitiatorID,
        accountsBlockedBy = p.Initiator.AccountBlockedBy,
        commenterID = context.Users.Where(u => u.UserName == postCommentRequest.userName).Select(s => s.UserID).FirstOrDefault(),
    })
    .Select(s =>
        new
        {
            PostId = s.PostID,
            s.commenterID,
            s.PostOwnerId,
            IsBlocked = s.accountsBlockedBy.Any(b => b.BlockingUserID == s.commenterID && b.BlockedAccountId == s.PostOwnerId) || s.accountsBlockedBy.Any(b => b.BlockingUserID == s.PostOwnerId && b.BlockedAccountId == s.commenterID)
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
        ReceivingUserID = result.PostOwnerId,
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
                p.PostID,
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
