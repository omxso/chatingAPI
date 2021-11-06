namespace API.DTOs
{
    public class PhotoForApprovalDto //PhotoForApprovalDto with the Photo Id, the Url, the Username and the isApproved status
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Username { get; set; }
        public bool IsApproved { get; set; }
    }
}