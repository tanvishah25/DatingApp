using System.Diagnostics.CodeAnalysis;

namespace DatingApp.Helpers
{
    public class MessageParams :PaginationParams
    {
        public string? Username { get; set; }
        public string Container { get; set; } = "Unread";
    }
}
