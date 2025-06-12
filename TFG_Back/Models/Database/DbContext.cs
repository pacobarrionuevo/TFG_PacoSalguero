using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TFG_Back.Models.Database.Entidades;
using static System.Net.Mime.MediaTypeNames;

namespace TFG_Back.Models.Database
{
    public class DBContext : DbContext
    {
        // Define el nombre del archivo de la base de datos SQLite.
        private const string DATABASE_PATH = "AA_TFG_Sanitarios.db";

        // Define las tablas de la base de datos como propiedades DbSet.
        private readonly Settings _settings;
        public DbSet<User> Users { get; set; }
        public DbSet<Entidades.Image> Images { get; set; }
        public DbSet<EntradaAgenda> EntradasAgenda { get; set; }
        public DbSet<Service> Servicios { get; set; }
        public DbSet<PaymentMethod> Payments { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<FriendShip> Friendships { get; set; }
        public DbSet<UserHasFriendship> UserHasFriendship { get; set; }

        public DBContext(IOptions<Settings> options)
        {
            _settings = options.Value;
        }

        // Configura Entity Framework para usar una base de datos SQLite.
        // La base de datos se creará en el directorio base de la aplicación.
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            options.UseSqlite($"DataSource={baseDir}{DATABASE_PATH}");
        }

        // Configuración adicional del modelo de datos.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura la propiedad LastSeen para que siempre se trate como UTC.
            // Esto es crucial para evitar problemas de zona horaria al almacenar y recuperar fechas.
            modelBuilder.Entity<User>()
                .Property(u => u.LastSeen)
                .HasConversion(
                    // Conversor para guardar: no se necesita conversión especial.
                    v => v,
                    // Conversor para leer: Especifica que el DateTime leído es de tipo UTC.
                    v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : (DateTime?)null
                );
        }
    }
}