using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Shojaee.UserModule
{
    public class JWTHandlingService
    {
        private string key = "@789312sdahdkjs@!#123jkkash@#12/**4654dsa978321@!|}{dsahkjdsajgjghf";
        public string JwtGenerator(string customerid, string role, int expiretimehours)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, customerid),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(expiretimehours),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey
                (Encoding.ASCII.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return tokenString;
        }

      
    }
}
