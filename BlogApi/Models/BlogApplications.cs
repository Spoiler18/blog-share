﻿namespace BlogApi.Models
{
    public class BlogApplications : BaseModel
    {
        public int BlogId { get; set; }
        public int? UserId { get; set; }
        public string? BlogTitle { get; set; }
        public string? BlogDescription { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class DetailedBlogApplications : BlogApplications
    {
        public string? FullName { get; set; }
        public bool? IsDeleted { get; set; }
        public IEnumerable<BlogImage>? BlogImages { get; set; }
        public IEnumerable<DetailedReaction>? BlogReactions { get; set; }
        public IEnumerable<DetailedComment>? BlogComments { get; set; }
    }

    public class BlogImage : BaseModel
    {
        public int ImageId { get; set; }
        public int? BlogId { get; set; }
        public string? ImagePath { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
