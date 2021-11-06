namespace API.DTOs
{
    public class PhotoDto
    {
        public int Id { get; set; }
        public string url { get; set; }
        public bool isMine { get; set; }
        public bool isApproved { get; set; } //Update the PhotoDto 3
    }
}