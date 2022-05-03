using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthServer.Service.Services
{
    internal class SignService
    {
        public static SecurityKey GetSymmetricSecurityKey(string securityKey) => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
    }
}
