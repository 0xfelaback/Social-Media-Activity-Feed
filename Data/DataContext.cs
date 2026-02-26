using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SocialMediaDataContext(DbContextOptions<SocialMediaDataContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    public DbSet<Follow> Follows { get; set; } = null!;
    public DbSet<CloseFriend> CloseFriends { get; set; } = null!;
    public DbSet<BlockedAccount> BlockedAccounts { get; set; } = null!;
    public DbSet<Post> Posts { get; set; } = null!;
    public DbSet<SavedPost> SavedPosts { get; set; } = null!;
    public DbSet<PostMedia> PostMedias { get; set; } = null!;
    public DbSet<PostLike> PostLikes { get; set; } = null!;
    public DbSet<Comment> Comments { get; set; } = null!;
    public DbSet<Message> Messages { get; set; } = null!;

    public DbSet<FeedContent> FeedContents { get; set; } = null!;
    public DbSet<MessageMedia> MessageMedias { get; set; } = null!;
    public DbSet<Notification> Notifications { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<FeedContent>().HasIndex(k => new { k.FollowerID, k.FollowedUserID }).IsUnique();
        modelBuilder.Entity<FeedContent>().HasOne(f => f.Follower).WithMany(f => f.FeedContents).HasForeignKey(fk => new { fk.FollowerID });
        modelBuilder.Entity<FeedContent>().HasOne(f => f.Followed).WithMany(f => f.FeedContentsIn).HasForeignKey(fk => new { fk.FollowedUserID });

        //many-many entities fk entities
        modelBuilder.Entity<Follow>().HasKey(f => new { f.FollowerID, f.FollowedUserID });
        modelBuilder.Entity<Follow>().HasIndex(f => new { f.FollowedUserID, f.FollowerID });
        modelBuilder.Entity<Follow>().HasOne(f => f.Follower).WithMany(k => k.FollowingAccounts).OnDelete(DeleteBehavior.Restrict).HasForeignKey(fk => fk.FollowerID);
        modelBuilder.Entity<Follow>().HasOne(f => f.Followed).WithMany(k => k.FollowersAccounts).OnDelete(DeleteBehavior.Restrict).HasForeignKey(fk => fk.FollowedUserID);

        modelBuilder.Entity<BlockedAccount>().HasKey(b => new { b.BlockingUserID, b.BlockedAccountId });
        modelBuilder.Entity<BlockedAccount>().HasIndex(b => new { b.BlockedAccountId, b.BlockingUserID });
        modelBuilder.Entity<BlockedAccount>().HasOne(b => b.BlockingUser).WithMany(b => b.BlockedAccounts).HasForeignKey(fk => fk.BlockingUserID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<BlockedAccount>().HasOne(b => b.Blocked_Account).WithMany(b => b.AccountBlockedBy).HasForeignKey(fk => fk.BlockedAccountId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<CloseFriend>().HasKey(c => new { c.AddingUserID, c.CloseFriendAccountId });
        modelBuilder.Entity<CloseFriend>().HasIndex(c => new { c.CloseFriendAccountId, c.AddingUserID });
        modelBuilder.Entity<CloseFriend>().HasOne(c => c.AddingUser).WithMany(c => c.AddedCloseFriends).HasForeignKey(fk => fk.AddingUserID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<CloseFriend>().HasOne(c => c.CloseFriendAccount).WithMany(c => c.AddedAsCloseFriendBy).HasForeignKey(fk => fk.CloseFriendAccountId).OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>().HasIndex(m => new { m.SenderID, m.ReceiptientID }).IsUnique();
        modelBuilder.Entity<Message>().HasIndex(m => new { m.ReceiptientID, m.SenderID });
        modelBuilder.Entity<Message>().HasOne(m => m.Sender).WithMany(m => m.MessagesSent).HasForeignKey(fk => fk.SenderID).OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Message>().HasOne(m => m.Receiptient).WithMany(m => m.MessagesReceived).HasForeignKey(fk => fk.ReceiptientID).OnDelete(DeleteBehavior.Restrict);


    }
}