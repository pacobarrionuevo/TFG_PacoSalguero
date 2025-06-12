namespace TFG_Back.Extensions
{
    // Clase de extensión para HttpRequest, proporciona métodos de utilidad.
    public static class HttpExtension
    {
        // Obtiene la URL base de la petición (ej. "https://localhost:7077/").
        public static string GetBaseUrl(this HttpRequest request)
        {
            return $"{request.Scheme}://{request.Host}/";
        }

        // Convierte una URL relativa (ej. "images/avatar.png") en una URL absoluta.
        // Esencial para que el cliente pueda acceder a los recursos estáticos.
        public static string GetAbsoluteUrl(this HttpRequest request, string relativeUrl)
        {
            Uri baseUrl = new Uri(request.GetBaseUrl());

            return new Uri(baseUrl, relativeUrl).ToString();
        }
    }
}