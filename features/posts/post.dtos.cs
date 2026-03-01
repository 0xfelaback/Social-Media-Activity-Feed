using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public record PostViewResponse
{
    [Url]
    public required string postOwnerPfpUrl { get; set; }
    public required string postOwnerUserName { get; set; }
    public required DateTime DateCreated { get; set; }

    public required int LikesCount { get; set; }
    public required string postCaption { get; set; }
    public required List<PostMedia> postMedias { get; set; }
    public required List<Comment> postComments { get; set; }
    public required bool isSaved { get; set; }
}

public record PostViewRequest
{
    public required string userName { get; set; }
}

public record PostDeleteRequest
{
    public required string userName { get; set; }
}

public record PostRequest
{
    public required string userName { get; set; }
    [MaxLength(800)]
    public required string caption { get; set; }
    public required List<Mediainput> PostMediasLinkInputs { get; set; }
}

public class Mediainput
{
    public enum MediaType
    {
        Image, Video
    };
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public required MediaType mediaType { get; set; }
    [Url(ErrorMessage = "Invalid URL format")]
    public required string mediaURL { get; set; }
}

public record PostLikeRequest
{
    public required string userName { get; set; }
}

public record PostCommentRequest
{
    public required string userName { get; set; }
    public required string commentText { get; set; }
}

public record PostSaveRequest
{
    public required string userName { get; set; }
}