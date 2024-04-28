using Microsoft.EntityFrameworkCore;

namespace BlogApi.Models
{

    public partial class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions
        <BlogContext> options)
            : base(options)
        {
        }
        public virtual DbSet<BlogApplications> BlogApplication { get; set; }
        public virtual DbSet<UserDetails> UserDetail { get; set; }
        public virtual DbSet<BlogComment> BlogComments { get; set; }
        public virtual DbSet<BlogReaction> BlogReactions { get; set; }
        public virtual DbSet<CommentReaction> CommentReactions { get; set; }
        public virtual DbSet<BlogImage> BlogImages { get; set; }
        public virtual DbSet<UserLog> UserLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogApplications>(entity => {
                entity.HasKey(k => k.BlogId);
            });

            modelBuilder.Entity<UserDetails>(entity => {
                entity.HasKey(k => k.UserId);
            });

            modelBuilder.Entity<BlogComment>(entity => {
                entity.HasKey(k => k.CommentId);
            });

            modelBuilder.Entity<BlogReaction>(entity => {
                entity.HasKey(k => k.ReactionId);
            });

            modelBuilder.Entity<CommentReaction>(entity => {
                entity.HasKey(k => k.CommentReactionId);
            });

            modelBuilder.Entity<BlogImage>(entity => {
                entity.HasKey(k => k.ImageId);
            });

            modelBuilder.Entity<UserLog>(entity => {
                entity.HasKey(k => k.LogId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
