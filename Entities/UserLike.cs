namespace API.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; } //user that like aouther user
        public int SourceUserId { get; set; }
        public AppUser LikedUser { get; set; } //the liked user
        public int LikedUserId { get; set; }
    }
}