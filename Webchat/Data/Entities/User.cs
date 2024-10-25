using Microsoft.AspNetCore.Identity;

namespace Chat.Data.Entities
{
    public class User : IdentityUser
    {
        public string? DisPlayName { get; set; }
        public DateTime BirthDay { get; set; }
    }
}
