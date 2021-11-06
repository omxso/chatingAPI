using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data
{ 
    public class DataContext : IdentityDbContext<AppUser, AppRole, int, 
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
     IdentityRoleClaim<int>, IdentityUserToken<int>> // cuse we want to access user roles and we give our entits int pram we need to pass in this type pram, we need to spicify every tye of role that we gone get
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

      //   public DbSet<AppUser> Users { get; set; } we can now remove this dbset becose IdentityDbContext brovide it 
      
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Photo> Photos { get; set; } //DbSet for the photos so we can query directly 2

        protected override void OnModelCreating(ModelBuilder builder) //we need to override a method in DbContext 
        {
            base.OnModelCreating(builder);//bass in to the class we aew driving from

            builder.Entity<AppUser>() 
               .HasMany(ur => ur.UserRoles) // User has many UserRoles
               .WithOne(u => u.User) // UserRoles have one User
               .HasForeignKey(ur => ur.UserId) // this is provided by Identity
               .IsRequired();

            builder.Entity<AppRole>() 
               .HasMany(ur => ur.UserRoles) // Role has many UserRoles
               .WithOne(u => u.Role) // UserRoles have one Role
               .HasForeignKey(ur => ur.RoleId) // this is provided by Identity
               .IsRequired();

            builder.Entity<UserLike>()//bass the Entity as pram to the user we want to confgi
               .HasKey(k => new {k.SourceUserId, k.LikedUserId});//primary key

            //in the following to builder.Entity this is how we create many to many realationship
            builder.Entity<UserLike>()
               .HasOne(s => s.SourceUser)//one to many
               .WithMany(l => l.LikesUsers) //many to one
               .HasForeignKey(s => s.SourceUserId)//set the foring key
               .OnDelete(DeleteBehavior.Cascade);//if we delet a user we delete the related entities//if SQL server (DeleteBehavior.NoAction)


            builder.Entity<UserLike>()
               .HasOne(s => s.LikedUser)//one to many
               .WithMany(l => l.LikedByUsers) //many to one
               .HasForeignKey(s => s.LikedUserId)//set the foring key
               .OnDelete(DeleteBehavior.Cascade);//if we delet a user we delete the related entities//if SQL server (DeleteBehavior.NoAction)

            builder.Entity<Message>()
                 .HasOne(u => u.Recipient)
                 .WithMany(m => m.MessagesReceived)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                 .HasOne(u => u.Sender)
                 .WithMany(m => m.MessagesSent)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Photo>().HasQueryFilter(P => P.isApproved); // Query filter to only return approved photos

            builder.ApplyUtcDateTimeConverter();
        }
    }

    public static class UtcDateAnnotation //convert DateTime into Utc
{
  private const String IsUtcAnnotation = "IsUtc";
  private static readonly ValueConverter<DateTime, DateTime> UtcConverter =
    new ValueConverter<DateTime, DateTime>(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

  private static readonly ValueConverter<DateTime?, DateTime?> UtcNullableConverter =
    new ValueConverter<DateTime?, DateTime?>(v => v, v => v == null ? v : DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

  public static PropertyBuilder<TProperty> IsUtc<TProperty>(this PropertyBuilder<TProperty> builder, Boolean isUtc = true) =>
    builder.HasAnnotation(IsUtcAnnotation, isUtc);

  public static Boolean IsUtc(this IMutableProperty property) =>
    ((Boolean?)property.FindAnnotation(IsUtcAnnotation)?.Value) ?? true;

  /// <summary>
  /// Make sure this is called after configuring all your entities.
  /// </summary>
  public static void ApplyUtcDateTimeConverter(this ModelBuilder builder)
  {
    foreach (var entityType in builder.Model.GetEntityTypes())
    {
      foreach (var property in entityType.GetProperties())
      {
        if (!property.IsUtc())
        {
          continue;
        }

        if (property.ClrType == typeof(DateTime))
        {
          property.SetValueConverter(UtcConverter);
        }

        if (property.ClrType == typeof(DateTime?))
        {
          property.SetValueConverter(UtcNullableConverter);
        }
      }
    }
   }
  }

}