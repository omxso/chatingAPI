using System;
using System.Text.Json.Serialization;

namespace API.DTOs
{
    public class MessageDto
    {
        public int Id { get; set; } //message Id that will be genrated by the DB
        public int SenderId { get; set; }//track the sender Id
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }
        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string Contant { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        [JsonIgnore] // this will not be sent to the client
        public bool SenderDeleated { get; set; } // as long as they match the name inside the entite then auto mapper will map them dirclty 
        [JsonIgnore]
        public bool RecipientDeleted { get; set; }
    }
}