using Auth.Api.Models;
using Auth.Business.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Api.Handlers
{
    public class JwtHandler : IJwtHandler
    {
        private readonly IConfiguration _configuration;

        public JwtHandler(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(UserModel user)
        {
            var authClaims = new List<Claim>
            {
                new (ClaimTypes.Name, user.UserName),
            };

            foreach (var role in user.Roles.Replace(" ", string.Empty).Split(','))
            {
                authClaims.Add(new(ClaimTypes.Role, role.Equals(UserRoles.Admin, StringComparison.OrdinalIgnoreCase) ? UserRoles.Admin : UserRoles.User));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            _ = int.TryParse(_configuration["JWT:TokenValidityInMinutes"], out int tokenValidityInMinutes);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(tokenValidityInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
