using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TFG_Back.Models.Database.Entidades
{
    // Entidad que representa una entrada en la agenda.
    public class EntradaAgenda
    {
        public int Id { get; set; }

        // Almacena la fecha y la hora juntas para facilitar el ordenamiento y filtrado.
        [Required]
        [Column(TypeName = "datetime")]
        public DateTime FechaHora { get; set; }
        public string Cliente { get; set; }

        [Required]
        public string CentroTrabajo { get; set; }

        // Clave foránea para la relación con la entidad Service.
        [Required]
        public int ServiceId { get; set; }
        // Propiedad de navegación para el servicio asociado.
        public Service? Service { get; set; }

        // Se especifica el tipo de columna para asegurar la precisión decimal en la base de datos.
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public string Paciente { get; set; }
        public string Observaciones { get; set; }
    }
}