using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Repository.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Service.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration configuration;

    public TokenService(IConfiguration configuration)
    {
        this.configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // 1. הגדרת ה-Key וה-Credentials (חתימה)
        // המפתח חייב להיות לפחות 32 תווים כדי ש-HmacSha256 יעבוד
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // 2. הגדרת ה-Claims
        // אלו הפרטים שיהיו מוצפנים בתוך הטוקן ונוכל לשלוף אותם בכל בקשה
        var claims = new[]
        {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.UserRole.ToString()), // חשוב להרשאות!
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // מזהה ייחודי לטוקן
            };

        // 3. יצירת אובייקט הטוקן
        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1), // תוקף הטוקן (למשל 1 שעות)
            signingCredentials: credentials);

        // 4. המרת אובייקט הטוקן למחרוזת (String)
        return new JwtSecurityTokenHandler().WriteToken(token);
    } 
}