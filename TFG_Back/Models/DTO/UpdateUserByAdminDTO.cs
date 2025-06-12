namespace TFG_Back.Models.DTO
{
    // DTO específico para la actualización de datos de un usuario por un administrador.
    // Limita los campos que se pueden modificar.
    public class UpdateUserByAdminDTO
    {
        public string UserNickname { get; set; }
        public string UserEmail { get; set; }
    }
}