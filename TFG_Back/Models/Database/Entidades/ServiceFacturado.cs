﻿namespace TFG_Back.Models.Database.Entidades
{
    // Entidad que representa un servicio que ha sido facturado.
    public class ServiceFacturado
    {
        public int Id { get; set; }
        public string Centro { get; set; }
        public string Cliente { get; set; }
        public DateTime Fecha { get; set; }
        public string Paciente { get; set; }
        public string Servicio { get; set; }
        public string Observaciones { get; set; }
    }
}