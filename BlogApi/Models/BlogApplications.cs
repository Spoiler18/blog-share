namespace BlogApi.Models
{
    public class BlogApplications : BaseModel
    {
        public int BlogId { get; set; }
        public int? UserId { get; set; }
        public string? BlogTitle { get; set; }
        public string? BlogDescription { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
