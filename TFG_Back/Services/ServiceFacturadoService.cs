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

            // 1. Carga el documento de plantilla de Word.
            Document document = new Document();
            document.LoadFromFile(plantillaPath);

            // 2. Ejecuta MailMerge para campos simples (ej. nombre de usuario).
            string[] fieldNames = new string[] { "UserName" };
            string[] fieldValues = new string[] { "Jose" };
            document.MailMerge.Execute(fieldNames, fieldValues);

            // 3. Crea una DataTable con el mismo nombre que la tabla en la plantilla ("Factura").
            DataTable table = new DataTable("Factura");

            // 4. Agrega columnas a la DataTable que coincidan con los campos de la tabla de Word.
            table.Columns.Add("centro");
            table.Columns.Add("cliente");
            table.Columns.Add("fecha");
            table.Columns.Add("observaciones");
            table.Columns.Add("paciente");
            table.Columns.Add("servicio");

            // 5. Rellena la DataTable con los datos de los servicios.
            foreach (var servicio in servicios)
            {
                table.Rows.Add(
                    servicio.Centro,
                    servicio.Cliente,
                    servicio.Fecha.ToString("dd/MM/yyyy"),
                    servicio.Observaciones,
                    servicio.Paciente,
                    servicio.Servicio
                );
            }

            // 6. Ejecuta el merge de la tabla (actualmente comentado).
            // document.MailMerge.ExecuteWithDataTable(table);

            // 7. Guarda el documento resultante como PDF (actualmente comentado).
            // document.SaveToFile(outputPath, FileFormat.PDF);

            return outputPath;
        }
    }
}