
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(InitiatorID))]
public class Post
{
    [Key]
    public long PostID { get; set; }
    [Required]
    public long InitiatorID { get; set; }
    [ForeignKey(nameof(InitiatorID))]
    public User Initiator { get; set; } = null!;
    [Required]
    [MaxLength(800)]
    public string Caption { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    [Required]
    public int LikeCount { get; set; }
    public virtual ICollection<SavedPost> Saves { get; set; } = new List<SavedPost>();
    public virtual ICollection<PostMedia> PostMediasLinks { get; set; } = new List<PostMedia>();
    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public virtual ICollection<FeedContent>  Feed { get; set; }  = new List<FeedContent>();
}

[Index(nameof(PostID), nameof(SaverID))]
[PrimaryKey(nameof(SaverID), nameof(PostID))]
[Table("Saved_Posts")]
public class SavedPost
{
    public long SaverID { get; set; } 
    [ForeignKey(nameof(SaverID))]
    public User Saver { get; set; } = null!;

    public long PostID { get; set; } 
    [ForeignKey(nameof(PostID))]
    public Post Post { get; set; } = null!;
}

[Table("Post_Media_Links")]
public class PostMedia
{
    [Key]
    public long PostID { get; set; } 
    [ForeignKey(nameof(PostID))]
    public Post Post { get; set; } = null!;

    public enum Media_Type
    {
        Image, Video
    };
    [Required(ErrorMessage = "Please specify a Media Type")]
    public Media_Type? MediaType { get; set; }
    [Required]
    [Url(ErrorMessage = "Invalid URL format")]
    public string MediaURL { get; set; } = null!;
}

[PrimaryKey(nameof(LikerID), nameof(PostID))]
[Table("Post_Likes")]
public class PostLike
{
    public long LikerID { get; set; }
    [Required]
    [ForeignKey(nameof(LikerID))]
    public User Liker { get; set; } = null!;
    [Required]
    public long PostID { get; set; }
    [ForeignKey(nameof(PostID))]
    public Post Post { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}

public class Comment
{
    /*private Comment() {}
    public Comment(long _receivingUserID, long initaiatorID): base(_receivingUserID, initaiatorID)
    {
        CommenterID = InitiatorID;
    }*/
    [Key]
    public long PostID { get; set; }
    [ForeignKey(nameof(PostID))]
    public Post Post { get; set; } = null!;
    [Required]
    public long CommenterID { get; set; }
    [ForeignKey(nameof(CommenterID))]
    public User Commenter { get; set; } = null!;
    [Required]
    [MaxLength(1000)]
    public string CommentText { get; set; } = null!;
    public int LikeCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

