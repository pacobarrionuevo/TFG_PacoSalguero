using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database.Repositorios;
using System.Data;
using Spire.Doc;
using Spire.Doc.Documents;

namespace TFG_Back.Services
{
    // Servicio para la lógica de negocio de los servicios facturados.
    public class ServiceFacturadoService
    {
        private readonly ServiceFacturadoRepository _repository;
        private readonly IWebHostEnvironment _env;

        public ServiceFacturadoService(ServiceFacturadoRepository repository, IWebHostEnvironment env)
        {
            _repository = repository;
            _env = env;
        }

        public async Task<ICollection<ServiceFacturado>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        // Crea un nuevo registro de servicio facturado.
        public async Task<ServiceFacturado> CreateAsync(ServiceFacturado servicioFacturado)
        {
            var result = await _repository.InsertAsync(servicioFacturado);
            return result;
        }

        // Genera un PDF a partir de una plantilla de Word y una lista de servicios.
        public async Task<string> GenerarFacturaPDFAsync(IEnumerable<ServiceFacturado> servicios)
        {
            var plantillaPath = Path.Combine(_env.WebRootPath, "plantillas", "PlantillaFacturas.docx");

            var fileName = $"PlantillaFacturas.pdf";
            var outputDirectory = Path.Combine(_env.WebRootPath, "plantillas"); 
            Directory.CreateDirectory(outputDirectory); 

            var outputPath = Path.Combine(outputDirectory, fileName);

            // Cargar la plantilla
            Document document = new Document();
            document.LoadFromFile(plantillaPath);

            // Crear DataTable con los datos
            DataTable table = new DataTable("Factura");
            table.Columns.Add("centro");
            table.Columns.Add("cliente");
            table.Columns.Add("fecha");
            table.Columns.Add("paciente");
            table.Columns.Add("servicio");
            table.Columns.Add("observaciones");

            foreach (var servicio in servicios)
            {
                table.Rows.Add(
                    servicio.Centro,
                    servicio.Cliente,
                    servicio.Fecha.ToString("dd/MM/yyyy"),
                    servicio.Paciente,
                    servicio.Servicio,
                    servicio.Observaciones
                );
            }

            // Exportar como PDF
            document.MailMerge.ExecuteWidthRegion(table);
            document.SaveToFile(outputPath, FileFormat.PDF);

            return $"/plantillas/{fileName}";
        }

    }
}