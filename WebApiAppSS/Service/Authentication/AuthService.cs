using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApiAppSS.Service.Authentication
{
    public class AuthService
    {
        private readonly string _secretKey;
        private readonly string _issuer;

        public AuthService(IOptions<TokenSettings> tokenSettings)
        {
            _secretKey = tokenSettings.Value.Secret;
            _issuer = tokenSettings.Value.Issuer;
        }

        public string GenerateToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Email, email)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
