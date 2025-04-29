using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;
using TFG_Back;
using TFG_Back.Models.Database.Repositorios;
using TFG_Back.Models.Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

Directory.SetCurrentDirectory(AppContext.BaseDirectory);

var builder = WebApplication.CreateBuilder(args);

// Configuraci�n inicial
builder.Services.Configure<Settings>(builder.Configuration.GetSection(Settings.SECTION_NAME));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<Settings>>().Value);

// Servicios principales
builder.Services.AddControllers().AddJsonOptions(options => {
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
// Configuracion de la base de datos
builder.Services.AddScoped<DBContext>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddScoped<ImageRepository>();
builder.Services.AddScoped<UserRepository>();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Autenticacion JWT
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

        // Soporte para token en query string
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context => {
                context.Token = context.Request.Query["token"];
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();
app.UseHttpsRedirection();


// Inicializacion de base de datos
await InitDatabaseAsync(app.Services);

await app.RunAsync();

static async Task InitDatabaseAsync(IServiceProvider serviceProvider)
{
    using var scope = serviceProvider.CreateScope();
    using var dbContext = scope.ServiceProvider.GetService<DBContext>();

    if (dbContext.Database.EnsureCreated())
    {
        var seeder = new Seeder(dbContext);
        await seeder.SeedAsync();
    }
}