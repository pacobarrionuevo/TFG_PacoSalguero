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

        public ServiceFacturadoService(ServiceFacturadoRepository repository)
        {
            _repository = repository;
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
            // Define las rutas de la plantilla y del archivo de salida.
            var plantillaPath = @"C:\Users\pacob\Desktop\PlantillaWord\PlantillaFacturas.docx";
            var outputPath = @$"C:\Users\pacob\Desktop\PlantillaWord\PlantillaFacturasCopia{DateTime.Now:yyyyMMddHHmmss}.pdf";

            // 2. Cargar la plantilla
            Document document = new Document();
            document.LoadFromFile(plantillaPath);

            // 5. Crear el DataTable con el nombre de la región: "Factura"
            DataTable table = new DataTable("Factura");

            // 6. Agregar columnas con los mismos nombres que los campos de la plantilla
            table.Columns.Add("centro");
            table.Columns.Add("cliente");
            table.Columns.Add("fecha");
            table.Columns.Add("paciente");
            table.Columns.Add("servicio");
            table.Columns.Add("observaciones");

            // 7. Agregar filas a partir de la lista de servicios facturados
            foreach (var servicio in servicios)
            {
                table.Rows.Add(
                    servicio.Centro,
                    servicio.Cliente,
                    servicio.Fecha.ToString("dd/MM/yyyy"), // Ajusta formato si es necesario
                    servicio.Paciente,
                    servicio.Servicio,
                    servicio.Observaciones
                );
            }

            // 8. Ejecutar la combinación de correspondencia para la tabla
            document.MailMerge.ExecuteWidthRegion(table);

            // 9. Exportar como PDF
            document.SaveToFile(outputPath, FileFormat.PDF);

            // Retornar la ruta del archivo generado
            return outputPath;

        }
    }
}