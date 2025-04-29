using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TFG_Back.Models.Database.Entidades
{
    public class EntradaAgenda
    {
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; }


        [Required]
        public string Cliente { get; set; }

        [Required]
        public string CentroTrabajo { get; set; }

        [Required]
        public string Servicio { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public string Paciente { get; set; }
        public string Observaciones { get; set; }

        [Required]
        public TimeSpan Hora { get; set; } //Uso TimeSpan para mejorar mejor las horas
    }
}
