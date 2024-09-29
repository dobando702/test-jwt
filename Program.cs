// Program.cs
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using test_jwt.Services;

var builder = WebApplication.CreateBuilder(args);

// A�adir servicios al contenedor.
builder.Services.AddControllers();

// Configurar Swagger para la documentaci�n de la API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar JwtService como un servicio singleton
builder.Services.AddSingleton<JwtService>();

// Configurar autenticaci�n JWT
var jwtConfig = builder.Configuration.GetSection("JwtConfig");
var secretKey = jwtConfig["Secret"];
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Solo para desarrollo. En producci�n, debe ser true.
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, // Validar la clave de firma
        IssuerSigningKey = new SymmetricSecurityKey(key), // Clave de firma
        ValidateIssuer = false, // No validar el emisor
        ValidateAudience = false, // No validar la audiencia
        ClockSkew = TimeSpan.Zero // Opcional: eliminar el margen de tiempo
    };
});

// A�adir autorizaci�n
builder.Services.AddAuthorization();

builder.Services.AddSwaggerGen(options =>
{
    // Definir el esquema de seguridad para JWT
    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Ingresa tu token JWT en este formato: Bearer {tu_token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    // A�adir la definici�n de seguridad a Swagger
    options.AddSecurityDefinition("Bearer", securityScheme);

    // Definir los requisitos de seguridad para Swagger
    var securityRequirement = new OpenApiSecurityRequirement
    {
        { securityScheme, new[] { "Bearer" } }
    };

    options.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

// Configurar el pipeline HTTP.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}



app.UseHttpsRedirection();

// A�adir autenticaci�n y autorizaci�n al pipeline
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
