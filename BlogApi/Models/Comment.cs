namespace BlogApi.Models
{
    public class BlogComment : BaseModel
    {
        public int CommentId { get; set; }
        public int? UserId { get; set; }
        public int? BlogId { get; set; }
        public string? UserComment { get; set; }
        public bool? IsDeleted { get; set; }
        public int? ReplyToCommentId { get; set; }
    }

    public class DetailedComment : BlogComment
    {
        public string? UserCommentFullName { get; set; }
        public IEnumerable<DetailedCommentReaction> CommentReactions { get; set; }
    }

    public class BlogReaction : BaseModel
    {
        public int ReactionId { get; set; }
        public int? UserId { get; set; }
        public int? BlogId { get; set; }
        public int? UserReaction { get; set; }
    }

    public class DetailedReaction : BlogReaction
    {
        public string? UserReactionFullName { get; set; }
    }

    public class CommentReaction : BaseModel
    {
        public int CommentReactionId { get; set; }
        public int? CommentId { get; set; }
        public int? UserId { get; set; }
        public int? UserReaction { get; set; }
    }

    public class DetailedCommentReaction : CommentReaction
    {
        public string? UserCommentReactionFullName { get; set; }
    }

}
