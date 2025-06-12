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
builder.Services.AddScoped<ServiceRepository>();
builder.Services.AddScoped<PaymentMethodRepository>();
builder.Services.AddScoped<CustomerRepository>();
builder.Services.AddScoped<FriendRequestRepository>();
builder.Services.AddScoped<WebSocketMethods>();

builder.Services.AddTransient<ServicesService>();
builder.Services.AddTransient<PaymentMethodService>();
builder.Services.AddTransient<CustomerService>();
builder.Services.AddTransient<ImageService>();
builder.Services.AddTransient<AdminService>();
builder.Services.AddTransient<FriendRequestService>();
builder.Services.AddScoped<WebSocketNetwork>();
builder.Services.AddTransient<Middleware>();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TFG API", Version = "v1" });

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

// Middleware para mover token de query a header
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
app.UseMiddleware<Middleware>();
app.UseHttpsRedirection();
app.MapControllers();


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