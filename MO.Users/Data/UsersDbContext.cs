﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MO.Users.Data;

public class UsersDbContext : IdentityDbContext<AuthUser>
{
    public UsersDbContext(DbContextOptions options):base(options)
    {
        Database.EnsureCreated();
    }
    public DbSet<User> AppUsers { get; set; }
}
public class AuthUser : IdentityUser
{
    public User AppUser { get; set; }
}

public class User
{
    public long UserId { get; set; }
    public string Username { get; set; }
}