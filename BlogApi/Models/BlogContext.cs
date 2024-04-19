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
        public virtual DbSet<BlogComment> Comments { get; set; }
        public virtual DbSet<BlogReaction> Reactions { get; set; }
        public virtual DbSet<CommentReaction> CommentReactions { get; set; }

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

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
