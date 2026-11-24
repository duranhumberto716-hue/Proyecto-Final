using System.Security.Cryptography;
using System.Text;

namespace Proyecto_Final.Services;

/// <summary>
/// Utilidades de seguridad. Las contraseñas se almacenan como hash SHA-256.
/// </summary>
public static class Seguridad
{
    public static string HashPassword(string password)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password ?? string.Empty));
        return Convert.ToHexString(bytes);
    }
}

