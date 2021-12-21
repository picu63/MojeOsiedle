using System.ComponentModel.DataAnnotations.Schema;
using Apis;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MO.LiveChat.Data;

public class LiveChatDbContext : IdentityDbContext<AuthUser>
{
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
            .Property(p => p.ReactionDate)
            .HasDefaultValueSql("getdate()");

        base.OnModelCreating(modelBuilder);
    }
}

public class AuthUser : IdentityUser
{
    public ChatUser ChatUser { get; set; }
}

public class ChatUser
{
    public long ChatUserId { get; set; }
    public string Username { get; set; }
    public List<Message> Messages { get; set; }
    public List<Reaction> Reactions { get; set; }
}

public class Message
{
    public long MessageId { get; set; }
    public string Text { get; set; }
    public long GroupId { get; set; }
    public Group Group { get; set; }
    public List<Reaction> Reactions { get; set; }
    public long? ChatUserId { get; set; }
    public ChatUser ChatUser { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CastDate { get; set; }
}

public class Group
{
    public long GroupId { get; set; }
    public string Name { get; set; }
    public List<Message> Messages { get; set; }
}

public class Reaction
{
    public long ReactionId { get; set; }
    public long ChatUserId { get; set; }
    public ChatUser ChatUser { get; set; }
    public long MessageId { get; set; }
    public Message Message { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime ReactionDate { get; set; }
}