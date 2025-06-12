namespace TFG_Back.Recursos
{
    // Clase de utilidad para manejar operaciones con archivos.
    public class FileHelper
    {
        // Define la ruta absoluta a la carpeta wwwroot, donde se almacenan los archivos estáticos.
        public static string WwwrootPath { get; } = Path.Combine(AppContext.BaseDirectory, "wwwroot");

        // Guarda un array de bytes en un archivo en la ruta relativa especificada dentro de wwwroot.
        public static async Task SaveAsync(byte[] bytes, string relativePath)
        {
            string absolutePath = Path.Combine(WwwrootPath, relativePath);
            // Asegura que el directorio de destino exista antes de escribir el archivo.
            Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);
            await File.WriteAllBytesAsync(absolutePath, bytes);
        }

        // Sobrecarga del método para guardar un archivo a partir de un Stream.
        public static async Task SaveAsync(Stream stream, string relativePath)
        {
            // Lee el stream en un MemoryStream para luego convertirlo a un array de bytes.
            using MemoryStream ms = new();
            await stream.CopyToAsync(ms);
            await SaveAsync(ms.ToArray(), relativePath);
        }
    }
}