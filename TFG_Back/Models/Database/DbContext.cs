using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TFG_Back.Models.Database.Entidades;
using static System.Net.Mime.MediaTypeNames;

namespace TFG_Back.Models.Database
{
    public class DBContext : DbContext
    {
        // Nombre de la base de datos y luego se llama abajo
        private const string DATABASE_PATH = "AA_TFG_Sanitarios.db";

        // Tablas de la base de datos
        private readonly Settings _settings;
        public DbSet<User> Users { get; set; }
        public DbSet<Entidades.Image> Images { get; set; }
        public DbSet<EntradaAgenda> EntradasAgenda { get; set; }

        public DbSet<Service> Servicios { get; set; }

        // Configuramos el EntityFramework para crear un archivo de BBDD Sqlite
        public DBContext(IOptions<Settings> options)
        {
            _settings = options.Value;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)            
        {
        #if DEBUG
                string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                options.UseSqlite($"DataSource={baseDir}{DATABASE_PATH}");
#else
            string connection = "Server = db18491.databaseasp.net; Database = db18491; Uid = db18491; Pwd = 8Qj_Z-9r!2fL";
            options.UseMySql(connection, ServerVersion.AutoDetect(connection));
#endif

        }
    }
}
