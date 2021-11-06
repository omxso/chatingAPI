using System;

namespace API.Entities
{
    public class Message
    {
        public int Id { get; set; } //message Id that will be genrated by the DB
        public int SenderId { get; set; }//track the sender Id
        public string SenderUsername { get; set; }
        public AppUser Sender { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public AppUser Recipient { get; set; }
        public string Contant { get; set; }
        public DateTime? DateRead { get; set; }//we manke this prop optional cuse we wants it ri be null if the message have not been read
        public DateTime MessageSent { get; set; } = DateTime.UtcNow; // DateTime.UtcNow;//set to the current time as soon as a new enstant is created 
        public bool SenderDeleated { get; set; }
        public bool RecipientDeleted { get; set; }

    }
}