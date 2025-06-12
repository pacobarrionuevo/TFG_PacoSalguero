using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using TFG_Back;
using TFG_Back.Models.Database.Repositorios;
using TFG_Back.Models.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TFG_Back.Services;
using TFG_Back.Servicios;
using TFG_Back.WebSocketAdvanced;
using Microsoft.OpenApi.Models;

// Establece el directorio de trabajo actual al directorio base de la aplicación.
Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

// --- Configuración de Servicios ---

// Carga la configuración desde appsettings.json en la clase Settings.
builder.Services.Configure<Settings>(builder.Configuration.GetSection(Settings.SECTION_NAME));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<Settings>>().Value);

// Configura los controladores y la serialización JSON para ignorar ciclos de referencia.
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

// --- Inyección de Dependencias (Scoped: una instancia por petición HTTP) ---
builder.Services.AddScoped<DBContext>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<ImageRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ServiceRepository>();
builder.Services.AddScoped<PaymentMethodRepository>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<FriendRequestRepository>();
builder.Services.AddScoped<ServiceFacturadoRepository>();
builder.Services.AddScoped<WebSocketMethods>();

// --- Inyección de Dependencias (Transient: una nueva instancia cada vez que se solicita) ---
builder.Services.AddTransient<ServicesService>();
builder.Services.AddTransient<PaymentMethodService>();
builder.Services.AddTransient<CustomerService>();
builder.Services.AddTransient<ImageService>();
builder.Services.AddTransient<AdminService>();
builder.Services.AddTransient<FriendRequestService>();
builder.Services.AddTransient<ServiceFacturadoService>();
builder.Services.AddScoped<WebSocketNetwork>(); // Scoped para que haya una única instancia por petición (importante para WebSocket).
builder.Services.AddTransient<Middleware>();

// --- Configuración de Swagger/OpenAPI ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TFG API", Version = "v1" });

    // Configura Swagger para que se pueda usar la autenticación JWT en la UI.
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Introduce 'Bearer' seguido de tu token JWT",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {
            securityScheme,
            Array.Empty<string>()
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});

// --- Configuración de CORS ---
// Se configura la política de CORS para permitir peticiones desde cualquier origen,
// necesario para la comunicación con el frontend de Angular.
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// --- Configuración de Autenticación JWT ---
var jwtSettings = builder.Configuration.GetSection(Settings.SECTION_NAME).Get<Settings>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false, 
            ValidateAudience = false, 
            ValidateLifetime = true, 
            ValidateIssuerSigningKey = true, 
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.JwtKey))
        };

        // Permite que el token JWT se envíe en la query string,
        // lo cual es necesario para la autenticación de WebSockets.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context => {
                context.Token = context.Request.Query["token"];
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();


// Habilita Swagger solo en el entorno de desarrollo.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Middleware para mover el token de la query string al header de Authorization.
// Esto asegura que el middleware de autenticación estándar pueda procesarlo.
app.Use(async (context, next) => {
    var token = context.Request.Query["token"];
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers["Authorization"] = $"Bearer {token}";
    }
    await next();
});

app.UseStaticFiles(); 
app.UseCors(); 
app.UseAuthentication(); 
app.UseAuthorization(); 
app.UseWebSockets();
app.UseMiddleware<Middleware>(); // Middleware personalizado para WebSockets.
app.UseHttpsRedirection(); 
app.MapControllers(); 

await InitDatabaseAsync(app.Services);

await app.RunAsync();

// Método para asegurar que la base de datos esté creada y poblada con datos iniciales.
static async Task InitDatabaseAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    using var dbContext = scope.ServiceProvider.GetService<DBContext>();

    // EnsureCreated devuelve true si la base de datos fue creada, false si ya existía.
    if (dbContext.Database.EnsureCreated())
    {
        var seeder = new Seeder(dbContext);
        await seeder.SeedAsync(); // Llama al Seeder para poblar la BD.
    }
}