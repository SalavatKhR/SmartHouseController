using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;

namespace WebAPI.Data;

public class ApplicationDbContext: IdentityDbContext<AppUser>
{
    public DbSet<Subscription> Subscriptions { get; set; }
    public ApplicationDbContext()
    {
    }
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<Subscription>(entity =>
        {
            entity.HasKey(e => new {e.UserId, e.Topic});
            entity.Property(e => e.DeviceName).IsRequired();
        });
        
        builder.Entity<AppUser>(entity =>
        {
            entity.HasData(
                new AppUser()
                {
                    Id = "5f34130c-2ed9-4c83-a600-e474e8f48bac",
                    UserName = "user01@gmail.com",
                    NormalizedUserName = "USER01@GMAIL.COM",
                    Email = "user01@gmail.com",
                    NormalizedEmail = "USER01@GMAIL.COM",
                    ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                    PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                    SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                    EmailConfirmed = true
                }
            );
            entity.HasData(
                new AppUser()
                {
                    Id = "120877ed-84b9-4ed5-9b87-d78965fc4fe0",
                    UserName = "user02@gmail.com",
                    NormalizedUserName = "USER02@GMAIL.COM",
                    Email = "user02@gamil.com",
                    NormalizedEmail = "USER02@GMAIL.COM",
                    ConcurrencyStamp = "37285e0f-b3c2-4a75-85f6-73a3c4c6da29",
                    PasswordHash = "AQAAAAEAACcQAAAAEED86xKz3bHadNf8B1Hg8t5qNefw4Bq1Kr2q6Jx9Ss/DcRIcUpLiFkDgQZTqUgJThA==", //qWe!123
                    SecurityStamp = "DKBWMTFC7TZQZ6UFNZ5BN5XQNDYUBJYQ,09bd35b0-9c9f-4772-8789-e6d4b9fbe9c4",
                    EmailConfirmed = true
                }
            );
        });
    }
}