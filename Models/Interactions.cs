
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

public class Message
{
    [Key]
    public long MessageID { get; set; }
    public long SenderID { get; set; }
    public long ReceiptientID { get; set; }
    public User Sender { get; set; } = null!;
    public User Receiptient { get; set; } = null!;
    [Required]
    public string MessageContent { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public ICollection<MessageMedia>? MessageMedia { get; set; }
}

[Table("Message_Media_Links")]
public class MessageMedia
{
    [Key]
    public long MessageID;
    [ForeignKey(nameof(MessageID))]
    public Message Message { get; set; } = null!;

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

[Table("Feed_Contents")]
public class FeedContent
{
    [Key]
    public long UserID { get; set; }
    public long FollowerID { get; set; }
    public long FollowedUserID { get; set; }
    public User Follower { get; set; } = null!;
    public User Followed { get; set; } = null!;
    public long PostID { get; set; }
    [ForeignKey(nameof(PostID))]
    public Post Post { get; set; } = null!;
}

[Index(nameof(ReceivingUserID), nameof(InitaiatorID))]
[Index(nameof(NotificationType))]
public class Notification
{
    [Key]
    public long NotificationID { get; set; }
    public long ReceivingUserID { get; set; }
    [ForeignKey(nameof(ReceivingUserID))]
    [InverseProperty(nameof(User.NotificationsReceived))]
    public User ReceivingUser { get; set; } = null!;
    public long InitaiatorID { get; set; }
    [ForeignKey(nameof(InitaiatorID))]
    [InverseProperty(nameof(User.NotificationsSent))]
    public User Initaiator { get; set; } = null!;
    public enum NotifType
    {
        Follow,
        Like,
        Comment
    }
    [Required(ErrorMessage = "Please specify a valid Notification Type")]
    public NotifType NotificationType { get; set; }
    public DateTime CreatedAt { get; set; }
}