
namespace Portfolio.API.Models;

public class JwtOptions
{
    public string Secret { get; set; } = string.Empty;   // chave simétrica
    public string Issuer { get; set; } = string.Empty;   // quem emite o token
    public string Audience { get; set; } = string.Empty; // quem consome
    public int ExpirationMinutes { get; set; } = 60;     // validade
}
