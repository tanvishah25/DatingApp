using System.ComponentModel.DataAnnotations;

namespace DatingApp.Entities
{
    public class Group
    {
        [Key]
        public required string Groupname { get; set; }
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
    }
}
