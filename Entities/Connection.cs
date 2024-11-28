using System.ComponentModel.DataAnnotations.Schema;

namespace DatingApp.Entities
{
    public class Connection
    {
        public required string ConnectionId { get; set; }
        public required string Username { get; set; }
        public string Groupname { get; set; }

        [ForeignKey("Groupname")]
        public Group Group { get; set; }
    }    
}
