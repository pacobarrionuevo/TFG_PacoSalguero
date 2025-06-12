namespace TFG_Back.Models.DTO
{
    // DTO para encapsular las estadísticas que se mostrarán en el panel de administración.
    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalServices { get; set; }
        public int TotalAgendaEntries { get; set; }
    }
}