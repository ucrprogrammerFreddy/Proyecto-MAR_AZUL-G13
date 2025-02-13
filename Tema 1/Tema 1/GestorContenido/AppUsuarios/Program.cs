using AppUsuarios.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
// Importa el paquete para trabajar con Entity Framework Core, usado para interactuar con la base de datos.
using Microsoft.EntityFrameworkCore;

// Crea el constructor de la aplicación y configura los servicios.
var builder = WebApplication.CreateBuilder(args);

//
// Configuración de servicios para la aplicación:
//


// ==========================================================================
// Configuración de la autenticación con cookies
// ==========================================================================

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).
    AddCookie(options =>
    {
        // Define el nombre de la cookie que se utilizará para la autenticación
        options.Cookie.Name = "CookieAuthentication";
        // La cookie solo se envía mediante HTTP (previniendo accesos por scripts maliciosos)
        options.Cookie.HttpOnly = true;
        // Tiempo de expiración de la cookie (59 minutos en este caso)
        options.ExpireTimeSpan = TimeSpan.FromMinutes(59);
        // Ruta a la que se redirige el usuario si no está autenticado
        options.LoginPath = "/Usuarios/Login";
        // Ruta a la que se redirige si el acceso es denegado
        options.AccessDeniedPath = "/Usuarios/AccessDenied";
        // Permite que la cookie se renueve si el usuario continúa activo (expiración deslizante)
        options.SlidingExpiration = true;
    });

// ==========================================================================
// Configuración de autorización y políticas de roles
// ==========================================================================
builder.Services.AddAuthorization(options =>
{
    // Política que requiere que el usuario tenga el rol "Admin"
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    // Política que requiere que el usuario tenga el rol "Member"
    options.AddPolicy("MemberPolicy", policy => policy.RequireRole("Member"));

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

// ==========================================================================
// Construcción de la aplicación con la configuración registrada
// ==========================================================================
// Construye la aplicación, aplicando todas las configuraciones definidas anteriormente.
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
