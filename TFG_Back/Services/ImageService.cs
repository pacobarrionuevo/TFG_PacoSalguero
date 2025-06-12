using TFG_Back.Models.Database;
using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.DTO;
using TFG_Back.Recursos;
using TFG_Back.Models.DTO;

namespace TFG_Back.Services
{
    // Servicio para la gestión de la lógica de negocio de las imágenes.
    public class ImageService
    {
        // Define la carpeta donde se guardarán las imágenes.
        private const string IMAGES_FOLDER = "fotos";

        private readonly UnitOfWork _unitOfWork;

        public ImageService(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Task<ICollection<Image>> GetAllAsync()
        {
            return _unitOfWork._imageRepository.GetAllAsync();
        }

        public Task<Image> GetAsync(long id)
        {
            return _unitOfWork._imageRepository.GetByIdAsync(id);
        }

        // Inserta una nueva imagen en la base de datos y guarda el archivo físico.
        public async Task<Image> InsertAsync(CreateUpdateImageRequest image)
        {
            // Genera una ruta relativa única para el archivo.
            string relativePath = $"{IMAGES_FOLDER}/{Guid.NewGuid()}_{image.File.FileName}";

            Image newImage = new Image
            {
                Name = image.Name,
                Path = relativePath
            };

            await _unitOfWork._imageRepository.InsertAsync(newImage);

            // Solo guarda el archivo si la inserción en la base de datos fue exitosa.
            if (await _unitOfWork.SaveAsync())
            {
                await StoreImageAsync(relativePath, image.File);
            }

            return newImage;
        }

        // Actualiza los datos de una imagen.
        public async Task<Image> UpdateAsync(long id, CreateUpdateImageRequest image)
        {
            Image entity = await _unitOfWork._imageRepository.GetByIdAsync(id);
            entity.Name = image.Name;

            _unitOfWork._imageRepository.Update(entity);

            // Si se proporciona un nuevo archivo, lo guarda sobrescribiendo el anterior.
            if (await _unitOfWork.SaveAsync() && image.File != null)
            {
                await StoreImageAsync(entity.Path, image.File);
            }

            return entity;
        }

        // Elimina una imagen de la base de datos.
        // Nota: No elimina el archivo físico para evitar referencias rotas si se usa en otro lugar.
        public async Task DeleteAsync(long id)
        {
            Image image = await _unitOfWork._imageRepository.GetByIdAsync(id);
            _unitOfWork._imageRepository.Delete(image);

            await _unitOfWork.SaveAsync();
        }

        // Método privado para encapsular el guardado del archivo.
        private async Task StoreImageAsync(string relativePath, IFormFile file)
        {
            using Stream stream = file.OpenReadStream();
            await FileHelper.SaveAsync(stream, relativePath);
        }
    }
}