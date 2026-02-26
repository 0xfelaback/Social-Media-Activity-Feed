using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;

public interface ITokenProvider
{
    public string Create(User user);
}

public class TokenProvider(IConfiguration configuration) : ITokenProvider
{
    public string Create(User user)
    {
        //required to sign the JWT
        string secretKey = configuration["Jwt:Key"] ?? string.Empty;
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        //signing credentials
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity
            ([
                //store statements about user in here
                // 1. The primary unique identifier (User ID)
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub, user.UserID.ToString()),// 2. Social/Auth identifiers
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
                new Claim(Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.PhoneNumber, user.PhoneNumber ?? string.Empty),
            ]
            ),
            Expires = DateTime.UtcNow.AddMinutes(60),
            SigningCredentials = credentials,
            Issuer = configuration["Jwt:Issuer"],
            Audience = configuration["Jwt:Issuer"]
        };
        var handler = new JsonWebTokenHandler();
        string token = handler.CreateToken(tokenDescriptor);
        return token;
    }
}
