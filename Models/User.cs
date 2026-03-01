using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

[Index(nameof(UserName), IsUnique = true)]
public class User
{
    [Key]
    public long UserID { get; set; }
    [MaxLength(25)]
    [Required(ErrorMessage = "Username is a required field")]
    public string UserName { get; set; } = null!;
    [MaxLength(50)]
    [Required(ErrorMessage = "Firstname is a required field")]
    public string FirstName { get; set; } = null!;
    [MaxLength(50)]
    [Required(ErrorMessage = "Lastname is a required field")]
    public string LastName { get; set; } = null!;
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? Email { get; set; }
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string? PhoneNumber { get; set; }
    [Url(ErrorMessage = "Invalid URL Format")]
    public string? ProfileImage_MediaUrl { get; set; }
    [Required]
    public int FollowersCount { get; set; }
    [Required]
    public int FollowingCount { get; set; }
    [MaxLength(200)]
    public string? Bio { get; set; }
    [MaxLength(255)]
    [Required]
    public string PasswordHash { get; set; } = null!;
    [Required]
    public DateTime CreatedAt { get; set; }
    [Required]
    public bool AccountDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    [Required]
    public UserProfile UserProfile { get; set; } = null!;

    public virtual ICollection<Follow> FollowingAccounts { get; set; } = new List<Follow>();// FollowersAccounts
    public virtual ICollection<Follow> FollowersAccounts { get; set; } = new List<Follow>(); // FollowedAccounts
    public virtual ICollection<BlockedAccount> BlockedAccounts { get; set; } = new List<BlockedAccount>();
    public virtual ICollection<BlockedAccount> AccountBlockedBy { get; set; } = new List<BlockedAccount>();
    public virtual ICollection<CloseFriend> AddedCloseFriends { get; set; } = new List<CloseFriend>();
    public virtual ICollection<CloseFriend> AddedAsCloseFriendBy { get; set; } = new List<CloseFriend>();
    public virtual ICollection<Post> UserPosts { get; set; } = new List<Post>();
    public virtual ICollection<SavedPost> UserSavedPosts { get; set; } = new List<SavedPost>();
    public virtual ICollection<PostLike> UserLikedPosts { get; set; } = new List<PostLike>();
    public virtual ICollection<Comment> UserComments { get; set; } = new List<Comment>();
    public virtual ICollection<Message> MessagesSent { get; set; } = new List<Message>();
    public virtual ICollection<Message> MessagesReceived { get; set; } = new List<Message>();
    [InverseProperty(nameof(Notification.ReceivingUser))]
    public virtual ICollection<Notification> NotificationsReceived { get; set; } = new List<Notification>();
    [InverseProperty(nameof(Notification.Initaiator))]
    public virtual ICollection<Notification> NotificationsSent { get; set; } = new List<Notification>();
    public virtual ICollection<FeedContent> FeedContents { get; set; } = new List<FeedContent>();
    public virtual ICollection<FeedContent> FeedContentsIn { get; set; } = new List<FeedContent>();
}


[Table("User_Profiles")]
public class UserProfile
{
    [Key]
    [ForeignKey("User")]
    public long UserID { get; set; }
    [Required]
    public User User { get; set; } = null!;
    [Url(ErrorMessage = "Invalid URL Format")]
    public string? Website { get; set; }
    [MaxLength(20)]
    public string? Gender { get; set; }
    [Required]
    public bool PushNotifications { get; set; }
    [Required]
    public bool AccountPrivacy { get; set; }
    [Required]
    public bool Verified { get; set; }
}

public class Follow
{
    public long FollowerID { get; set; }
    public long FollowedUserID { get; set; }
    [JsonIgnore]
    public User Follower { get; set; } = null!;
    [JsonIgnore]
    public User Followed { get; set; } = null!;
}

[Table("Blocked_Accounts")]
public class BlockedAccount
{
    public long BlockingUserID { get; set; }
    [JsonIgnore]
    public User BlockingUser { get; set; } = null!;
    public long BlockedAccountId { get; set; }
    [JsonIgnore]
    public User Blocked_Account { get; set; } = null!;
}

[Table("Close_Friends")]
public class CloseFriend
{
    public long AddingUserID { get; set; }
    [JsonIgnore]
    public User AddingUser { get; set; } = null!;

    public long CloseFriendAccountId { get; set; }
    [JsonIgnore]
    public User CloseFriendAccount { get; set; } = null!;
}
