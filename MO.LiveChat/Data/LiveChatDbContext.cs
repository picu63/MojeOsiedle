using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MO.LiveChat.Data;

public class LiveChatDbContext : DbContext
{
    public LiveChatDbContext(DbContextOptions options): base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<ChatUser> ChatUsers { get; set; }
    public DbSet<Reaction> Reactions { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>()
            .Property(r => r.CastDate)
            .HasDefaultValueSql("getdate()");
        modelBuilder.Entity<Reaction>()
            .HasOne(r => r.Message)
            .WithMany()
            .HasForeignKey(r => r.MessageId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Reaction>()
            .Property(p => p.ReactionDate)
            .HasDefaultValueSql("getdate()");
        base.OnModelCreating(modelBuilder);
    }
}

public class ChatUser
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid ChatUserId { get; set; }
    public string Username { get; set; }
    public List<Message> Messages { get; set; }
    public List<Reaction> Reactions { get; set; }
}

public class Message
{
    [Key]
    public Guid MessageId { get; set; }
    public string Text { get; set; }
    public Guid GroupId { get; set; }
    [ForeignKey("GroupId")]
    public Group Group { get; set; }
    public List<Reaction> Reactions { get; set; }
    public Guid ChatUserId { get; set; }
    [ForeignKey("ChatUserId")]
    public ChatUser ChatUser { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CastDate { get; set; }
}

public class Group
{
    [Key]
    public Guid GroupId { get; set; }
    public string Name { get; set; }
    public List<Message> Messages { get; set; }
}

public class Reaction
{
    [Key]
    public Guid ReactionId { get; set; }
    public Guid ChatUserId { get; set; }
    [ForeignKey("ChatUserId")]
    public ChatUser ChatUser { get; set; }
    public Guid MessageId { get; set; }
    //[ForeignKey("MessageId")]
    public Message Message { get; set; }
    public ReactionType ReactionType { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime ReactionDate { get; set; }
}

public enum ReactionType
{
    Like = 0,
}