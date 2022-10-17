using Microsoft.EntityFrameworkCore;
using Simple_Stocks.Models;

namespace Simple_Stocks.Services
{
    public class StocksDbContext : DbContext
    {
        public StocksDbContext(DbContextOptions<StocksDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Followers & Blocks
            modelBuilder.Entity<UserFollow>()
               .HasOne(l => l.SourceUser)
               .WithMany(a => a.Following)
               .HasForeignKey(l => l.SourceUserId);

            modelBuilder.Entity<UserFollow>()
               .HasOne(l => l.FollowedUser)
               .WithMany(a => a.Followers)
               .HasForeignKey(l => l.FollowedUserId);

            modelBuilder.Entity<UserBlock>()
               .HasOne(l => l.SourceUser)
               .WithMany(a => a.BlockedUsers)
               .HasForeignKey(l => l.SourceUserId);

            //Liked Comments
            modelBuilder.Entity<LikedComment>()
                .HasOne(u => u.User)
                .WithMany(lc => lc.LikedComments)
                .HasForeignKey(ui => ui.UserId);

            modelBuilder.Entity<LikedComment>()
                .HasOne(c => c.Comment)
                .WithMany(lc => lc.LikedComments)
                .HasForeignKey(ci => ci.CommentId);

            //Liked Posts
            modelBuilder.Entity<LikedPost>()
                .HasOne(u => u.User)
                .WithMany(lp => lp.LikedPosts)
                .HasForeignKey(ui => ui.UserId);

            modelBuilder.Entity<LikedPost>()
                .HasOne(p => p.Post)
                .WithMany(lp => lp.LikedPosts)
                .HasForeignKey(pi => pi.PostId);

            //Post's Tags
            modelBuilder.Entity<PostTag>()
                .HasOne(p => p.Post)
                .WithMany(pt => pt.PostTags)
                .HasForeignKey(pi => pi.PostId);

            modelBuilder.Entity<PostTag>()
                .HasOne(t => t.Tag)
                .WithMany(pt => pt.PostTags)
                .HasForeignKey(ti => ti.TagId);

            //User's Stocks
            modelBuilder.Entity<UserStock>()
                .HasOne(u => u.User)
                .WithMany(us => us.UserStocks)
                .HasForeignKey(ui => ui.UserId);

            modelBuilder.Entity<UserStock>()
                .HasOne(s => s.Stock)
                .WithMany(us => us.UserStocks)
                .HasForeignKey(si => si.StockId);

            //Roles & Rights
            modelBuilder.Entity<RoleRight>()
               .HasOne(ro => ro.Role)
               .WithMany(rr => rr.RoleRights)
               .HasForeignKey(ri => ri.RoleId);

            modelBuilder.Entity<RoleRight>()
               .HasOne(rg => rg.Right)
               .WithMany(rr => rr.RoleRights)
               .HasForeignKey(ri => ri.RightId);

            //Restrict delete behavior for foreign keys to prevent errors
            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }

            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<Post>? Posts { get; set; }
        public virtual DbSet<Comment>? Comments { get; set; }
        public virtual DbSet<Stock>? Stocks { get; set; }
        public virtual DbSet<Tag>? Tags { get; set; }
        public virtual DbSet<LikedComment>? LikedComments { get; set; }
        public virtual DbSet<LikedPost>? LikedPosts { get; set; }
        public virtual DbSet<PostTag>? PostTags { get; set; }
        public virtual DbSet<UserStock>? UserStocks { get; set; }
        public virtual DbSet<UserFollow>? UserFollows { get; set; }
        public virtual DbSet<UserBlock>? UserBlocks { get; set; }
        public virtual DbSet<Right>? Rights { get; set; }
        public virtual DbSet<Role>? Roles { get; set; }
        public virtual DbSet<RoleRight>? RoleRights { get; set; }
    }
}
