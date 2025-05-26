using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TFG_Back.Models.Database.Entidades
{
    public class EntradaAgenda
    {
        public int Id { get; set; }

         //[Required]
        //public DateTime Fecha { get; set; }


        [Required]
        [Column(TypeName = "datetime")] // Almacena fecha y hora juntas
        public DateTime FechaHora { get; set; }
        public string Cliente { get; set; }

        [Required]
        public string CentroTrabajo { get; set; }

        [Required]
        // Relacion con Service de paco
        public int ServiceId { get; set; }
        public Service? Service { get; set; }


        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        public string Paciente { get; set; }
        public string Observaciones { get; set; }

        //[Required]
       // public TimeSpan Hora { get; set; } //Uso TimeSpan para mejorar mejor las horas

    }
}
