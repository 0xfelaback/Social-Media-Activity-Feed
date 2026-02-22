using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Index(nameof(UserName), IsUnique = true)]
public class User
{
    [Key]
    public long UserID { get; set; }
    [MaxLength(25)]
    [Required]
    public string UserName { get; set; } = null!;
    [MaxLength(50)]
    [Required]
    public string FirstName { get; set; } = null!;
    [MaxLength(50)]
    [Required]
    public string LastName { get; set; } = null!;
    [EmailAddress(ErrorMessage = "Invalid Email Address.")]
    public string? Email { get; set; }
    [Phone(ErrorMessage = "Invalid Phone Number")]
    public string? PhoneNumber { get; set; }
    [Url(ErrorMessage = "Invalid URL Format")]
    [Required]
    public string ProfileImage_MediaUrl { get; set; } = null!;
    [Required]
    public int FollowersCount { get; set; }
    [Required]
    public int FollowingCount { get; set; }
    [MaxLength(200)]
    public string? Bio { get; set; }
    [MaxLength(255)]
    [Required]
    //requires hashing
    public string PasswordHash { get; set; } = null!;
    public UserProfile UserProfile { get; set; } = null!;
    
    public virtual ICollection<Follow> FollowingAccounts { get; set; } = new List<Follow>();
    public virtual ICollection<Follow> FollowedAccounts { get; set; } = new List<Follow>();
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
    public string Website { get; set; } = null!;
    [MaxLength(20)]
    public string Gender { get; set; } = null!;
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
    public long FollowedID { get; set; }
    public User Follower { get; set; } = null!;
    public User Followed { get; set; } = null!;
}

[Table("Blocked_Accounts")]
public class BlockedAccount
{
    public long BlockingUserID { get; set; }
    public User BlockingUser { get; set; } = null!;
    public long BlockedAccountId { get; set; }
    public User Blocked_Account { get; set; } = null!;
}

[Table("Close_Friends")]
public class CloseFriend
{
    public long AddingUserID { get; set; }
    public User AddingUser { get; set; } = null!;

    public long CloseFriendAccountId { get; set; }
    public User CloseFriendAccount { get; set; } = null!;
}
