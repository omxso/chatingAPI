using System.ComponentModel.DataAnnotations.Schema;

namespace API.Entities
{
    [Table("Photos")]//make sure the table name is photos in db
    public class Photo
    {
        public int Id { get; set; }
        public string url { get; set; }
        public bool isApproved { get; set; } // check if the photo is approved 1
        public bool isMine { get; set; }
        public string PublicId { get; set; }// id for photo in cloud 
        public AppUser AppUser { get; set; }
        public int AppUserId { get; set; }
    }
}