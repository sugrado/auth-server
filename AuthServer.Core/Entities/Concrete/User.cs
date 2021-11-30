using Microsoft.AspNetCore.Identity;

namespace AuthServer.Core.Entities.Concrete
{
    public class User : IdentityUser
    {
        public string City { get; set; }
    }
}
