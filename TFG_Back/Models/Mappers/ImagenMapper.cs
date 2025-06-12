using TFG_Back.Extensions;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.DTO;

namespace TFG_Back.Models.Mappers
{
    // Clase Mapper para convertir entidades Image a DTOs ImageDto.
    public class ImageMapper
    {
        // Convierte una única entidad Image a un ImageDto.
        public ImageDto ToDto(Image image, HttpRequest httpRequest = null)
        {
            return new ImageDto()
            {
                Id = image.Id,
                Name = image.Name,
                // Si se proporciona un HttpRequest, convierte la ruta relativa en una URL absoluta.
                Url = httpRequest is null ? image.Path : httpRequest.GetAbsoluteUrl(image.Path),
            };
        }

        // Convierte una colección de entidades Image a una colección de ImageDto.
        public IEnumerable<ImageDto> ToDto(IEnumerable<Image> images, HttpRequest httpRequest = null)
        {
            return images.Select(image => ToDto(image, httpRequest));
        }
    }
}