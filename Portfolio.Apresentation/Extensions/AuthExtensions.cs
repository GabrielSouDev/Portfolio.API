using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Portfolio.API.Models;
using Portfolio.Apresentation.Models;

namespace Portfolio.API.Extensions;

public static class AuthExtensions
{
    public static void AddAuthExtensions(this WebApplication app)
    {
        var group = app.MapGroup("auth");

        group.MapPost("/login", ([FromServices] IOptions<AdminUser> masterUser,
                                 [FromServices] IOptions<JwtOptions> jwtOptions,
                                 [FromBody] AdminUser loginRequest) =>
        {
            if (loginRequest is null
                || loginRequest.UserName != masterUser.Value.UserName
                || loginRequest.Password != masterUser.Value.Password)
            {
                return Results.Unauthorized();
            }

            var token = GenerateJwtToken(jwtOptions.Value, loginRequest.UserName);

            return Results.Ok(new
            {
                access_token = token,
                token_type = "Bearer",
                expires_in = jwtOptions.Value.ExpirationMinutes * 60
            });
        });
    }

    private static string GenerateJwtToken(JwtOptions options, string userName)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, userName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, userName),
        };

        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(options.ExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: string.IsNullOrWhiteSpace(options.Issuer) ? null : options.Issuer,
            audience: string.IsNullOrWhiteSpace(options.Audience) ? null : options.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}