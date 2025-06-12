using System.Security.Cryptography;
using System.Text;

namespace TFG_Back.Recursos
{
    // Clase de utilidad para el manejo de contraseñas.
    public class PasswordHelper
    {
        // Genera un hash SHA256 de una contraseña.
        // Es una forma de almacenar contraseñas de forma segura sin guardar el texto plano.
        public static string Hash(string password)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(password);
            byte[] inputHash = SHA256.HashData(inputBytes);
            // Devuelve el hash en formato Base64 para un almacenamiento consistente.
            return Convert.ToBase64String(inputHash);
        }
    }
}