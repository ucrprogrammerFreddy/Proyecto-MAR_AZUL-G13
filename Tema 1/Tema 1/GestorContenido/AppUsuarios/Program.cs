using AppUsuarios.Models;
using AppUsuarios.Services;
using Castle.Core.Smtp;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
// Importa el paquete para trabajar con Entity Framework Core, usado para interactuar con la base de datos.
using Microsoft.EntityFrameworkCore;

// Crea el constructor de la aplicación y configura los servicios.
var builder = WebApplication.CreateBuilder(args);

//
// Configuración de servicios para la aplicación:
//

builder.Services.AddTransient<IEmailService, EmailService>();

// ==========================================================================
// Configuración de la autenticación con cookies
// ==========================================================================

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "CookieAuthentication";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Strict;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Usuarios/Login";
        options.AccessDeniedPath = "/Usuarios/AccessDenied";
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();


// ==========================================================================
// Configuración de autorización y políticas de roles
// ==========================================================================
builder.Services.AddAuthorization(options =>
{
    // Política que requiere que el usuario tenga el rol "Admin"
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Administrador"));
    // Política que requiere que el usuario tenga el rol "Member"
    options.AddPolicy("MemberPolicy", policy => policy.RequireRole("Autorizador"));
    options.AddPolicy("MemberPolicy", policy => policy.RequireRole("Escritor"));

});

// ==========================================================================
// Configuración del manejo de sesiones
// ==========================================================================
builder.Services.AddSession(options =>
{
    // Tiempo máximo de inactividad antes de que la sesión expire (5 minutos)
    options.IdleTimeout = TimeSpan.FromMinutes(5);
    // La cookie de sesión solo se transmite por HTTP
    options.Cookie.HttpOnly = true;
    // Marca la cookie como esencial para el funcionamiento de la aplicación
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100 MB (ajústalo según necesites)




});





// ==========================================================================
// Registro de controladores con vistas (MVC)
// ==========================================================================
// Habilita el uso de controladores y vistas en la aplicación.
// Esto registra los controladores y el sistema de vistas en el contenedor de servicios.
builder.Services.AddControllersWithViews();


// Optimización 
/*builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = false;
    options.Providers.Add<GzipCompressionProvider>();
    options.MimeTypes = []
     { 
        "image/jpeg",
      "application/font-woff2",
      "image/gif  
     } 
 )
}

*/


// ==========================================================================
// Configuración del DbContext para Entity Framework Core
// ==========================================================================
// Agrega el servicio `DbContext` para trabajar con la base de datos.
// Aquí se define cómo la aplicación se conectará a la base de datos usando SQL Server.
// `AppUsuarios.Models.DbContextGestionContenido` es la clase que define el modelo de la base de datos.
// La cadena de conexión está almacenada en `appsettings.json` y se accede mediante el nombre "StringConexion".
builder.Services.AddDbContext<AppUsuarios.Models.DbContextGestionContenido>(
      options => options.UseSqlServer( // Especifica que se usará SQL Server como proveedor de base de datos.
        builder.Configuration.GetConnectionString("StringConexion") // Obtiene la cadena de conexión desde la configuración.
        )
);


builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTodo",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

// ==========================================================================
// Construcción de la aplicación con la configuración registrada
// ==========================================================================
// Construye la aplicación, aplicando todas las configuraciones definidas anteriormente.

// Registrar HttpClientFactory

builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri("http://marazulapi.somee.com/api/"); // ? Ajusta seg?n Swagger
});

var app = builder.Build();


// ==========================================================================
// Configuración del pipeline de middleware
// ==========================================================================



// Verifica si el entorno NO es de desarrollo.
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // Configura un controlador de errores para redirigir a una página de error personalizada.
    app.UseExceptionHandler("/Home/Error");

    // Habilita HTTP Strict Transport Security (HSTS) para aumentar la seguridad al trabajar con HTTPS.
    // Esto obliga a los navegadores a usar HTTPS en todas las conexiones con el sitio.
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Habilita la redirección automática de HTTP a HTTPS.
// Esto asegura que todas las solicitudes se realicen de forma segura.
app.UseHttpsRedirection();

// Habilita el uso de archivos estáticos en la aplicación.
// Esto incluye archivos como CSS, imágenes, y JavaScript que están en la carpeta `wwwroot`.
app.UseStaticFiles();

// Configura el sistema de enrutamiento de solicitudes.
app.UseRouting();

app.UseCors("PermitirTodo");

// Habilita el middleware de autorización.
// Esto asegura que las partes de la aplicación protegidas por políticas de autorización requieran que los usuarios tengan los permisos adecuados.
app.UseAuthorization();

// Configura el enrutamiento predeterminado de las solicitudes.
// Define la estructura de las URLs esperadas por la aplicación.
// En este caso, la estructura es:
// - `controller`: Controlador que procesará la solicitud (por defecto "Home").
// - `action`: Método dentro del controlador (por defecto "Index").
// - `id`: Parámetro opcional para pasar información adicional.
app.MapControllerRoute(
    name: "default", // Nombre de la ruta.
    pattern: "{controller=Home}/{action=Index}/{id?}");// Ruta predeterminada con parámetros.

// Inicia la aplicación y comienza a escuchar las solicitudes entrantes.
app.Run();
