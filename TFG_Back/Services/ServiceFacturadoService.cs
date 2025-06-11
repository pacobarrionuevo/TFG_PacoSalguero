using TFG_Back.Models.Database.Entidades;
using TFG_Back.Models.Database.Repositorios;
using System.Data;
using Spire.Doc;
using Spire.Doc.Documents;

namespace TFG_Back.Services
{
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

        public async Task<ServiceFacturado> CreateAsync(ServiceFacturado servicioFacturado)
        {
            // Guardar en base de datos
            var result = await _repository.InsertAsync(servicioFacturado);

            return result;
        }

        public async Task<string> GenerarFacturaPDFAsync(IEnumerable<ServiceFacturado> servicios)
        {
            var plantillaPath = @"C:\Users\pacob\Desktop\PlantillaWord\PlantillaFacturas.docx";
            var outputPath = @$"C:\Users\pacob\Desktop\PlantillaWord\PlantillaFacturasCopia.docx{DateTime.Now:yyyyMMddHHmmss}.pdf";

            // 1. Cargar la plantilla
            Document document = new Document();
            document.LoadFromFile(plantillaPath);

            // 2. Ejecutar MailMerge para campos simples (si tienes algún otro campo como encabezado)
            string[] fieldNames = new string[] { "UserName" };  // Por ejemplo
            string[] fieldValues = new string[] { "Jose" };
            document.MailMerge.Execute(fieldNames, fieldValues);

            // 3. Crear la tabla con el mismo nombre que en la plantilla: "Factura"
            DataTable table = new DataTable("Factura");

            // 4. Agregar columnas con los mismos nombres que los campos de la tabla Word
            table.Columns.Add("centro");
            table.Columns.Add("cliente");
            table.Columns.Add("fecha");
            table.Columns.Add("observaciones");
            table.Columns.Add("paciente");
            table.Columns.Add("servicio");

            // 5. Rellenar la tabla
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

            // 6. Ejecutar el merge de tabla
            // document.MailMerge.ExecuteWithDataTable(table);

            // 7. Guardar como PDF
            // document.SaveToFile(outputPath, FileFormat.PDF);

            return outputPath;
        }
    }

}
