using AppUsuarios.DTOs;
using AppUsuarios.Models;  // Importa los modelos definidos en el proyecto.
using Microsoft.AspNetCore.Authentication;  // Maneja la autenticación del usuario.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;  // Proporciona funcionalidades para gestionar controladores y vistas.
using Microsoft.EntityFrameworkCore;  // Permite trabajar con bases de datos usando Entity Framework.
using Microsoft.Extensions.Caching.Memory;  // Gestiona la autorización basada en roles o claims.
using System.Security.Claims;  // Define y administra los claims de usuario para la autorización.

namespace AppUsuarios.Controllers  // Define el espacio de nombres del controlador.
{


    /* si la lista de usuarios no cambia constantemente, se puede cachearla para eivitar consultas repetitivas la base de datos
     Detalles de usuarios : se pueden cachear proque generalmente no cambian con frecuencia*/


    public class UsuariosController : Controller  // Controlador responsable de gestionar los usuarios.
    {
        private readonly DbContextGestionContenido _context;  // Contexto de base de datos para acceder a los usuarios.
        private readonly IMemoryCache _cache;
        private static string EmailRestablecer = "";  // Variable para almacenar temporalmente el email en el proceso de restablecimiento.


        // Constructor del controlador, inicializa el contexto de la base de datos.
        public UsuariosController(DbContextGestionContenido context, IMemoryCache cache)
        {
            _context = context;  // Asigna el contexto proporcionado al controlador.
            _cache = cache;
        }

        // -------------------- LOGIN / LOGOUT --------------------

        [HttpGet]  // Este método responde a peticiones HTTP GET.
        public IActionResult Login()  // Muestra la vista de inicio de sesión.
        {
            return View();  // Devuelve la vista correspondiente.
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] UsuarioDTO userDto)
        {
            if (userDto == null || string.IsNullOrEmpty(userDto.Email) || string.IsNullOrEmpty(userDto.Clave))
            {
                TempData["Mensaje"] = "Debe proporcionar el email y la contraseña.";
                return View(userDto);
            }

            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == userDto.Email);

            if (usuario == null || !usuario.Clave.Equals(userDto.Clave) || usuario.Estado != "Activo")
            {
                TempData["Mensaje"] = "El email o la contraseña son incorrectos o el usuario no está activo.";
                return View(userDto);
            }

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.Nombre),
        new Claim(ClaimTypes.Email, usuario.Email),
        new Claim(ClaimTypes.Role, usuario.Rol)
    };

            var identity = new ClaimsIdentity(claims, "Login");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(principal);
            TempData["Mensaje"] = "Inicio de sesión exitoso.";

            return RedirectToAction("Index");
        }

        [HttpGet]  // Este método responde a peticiones HTTP GET.
        public async Task<IActionResult> Logout()  // Finaliza la sesión del usuario.
        {
            await HttpContext.SignOutAsync();  // Cierra la sesión actual.
            TempData["Mensaje"] = "Sesión cerrada correctamente.";  // Mensaje de cierre de sesión.
            return RedirectToAction("Login");  // Redirige a la vista de login.
        }

        // -------------------- CRUD: INDEX, CREATE, EDIT, DETAILS, DELETE --------------------


        [HttpGet]
        public JsonResult GetUsuarios(int pagina = 1)
        {
            int cantidadPorPagina = 5;  // Definimos 5 usuarios por página

            var usuarios = _context.Usuarios
                .OrderBy(u => u.IdUsuario)  // Ordenamos por ID o el campo que necesites
                .Skip((pagina - 1) * cantidadPorPagina)  // Desplazamos según la página actual
                .Take(cantidadPorPagina)  // Tomamos solo los 5 usuarios de esta página
                .Select(u => new { u.IdUsuario, u.Nombre, u.Email, u.Estado, u.Restablecer })  // Seleccionamos solo los campos necesarios
                .ToList();

            return Json(usuarios);  // Devolvemos los usuarios como JSON
        }



        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var cacheKey = "ListaUsuarios";
            if (!_cache.TryGetValue(cacheKey, out List<Usuario> usuarios))
            {
                usuarios = await _context.Usuarios.ToListAsync();

                // Cache por 5 minutos
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

                _cache.Set(cacheKey, usuarios, cacheOptions);
            }
            return View(usuarios);
        }

        [HttpGet]  // Este método responde a peticiones HTTP GET.
        public IActionResult Create()  // Muestra el formulario para crear un nuevo usuario.
        {
            return View();  // Retorna la vista correspondiente.
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] Usuario usuario)
        {
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                TempData["Mensaje"] = "El email ya está en uso.";
                return View(usuario);
            }

            usuario.Estado = "Activo";
            usuario.Restablecer = "Pendiente";

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            // Invalidar cache
            _cache.Remove("ListaUsuarios");

            TempData["Mensaje"] = "Usuario creado correctamente.";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var cacheKey = $"Usuario_{id}";
            if (!_cache.TryGetValue(cacheKey, out Usuario usuario))
            {
                usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
                if (usuario == null)
                {
                    TempData["Mensaje"] = "Usuario no encontrado.";
                    return RedirectToAction("Index");
                }

                // Cache por 10 minutos
                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

                _cache.Set(cacheKey, usuario, cacheOptions);
            }
            return View(usuario);
        }

        [HttpGet]  // Este método responde a peticiones HTTP GET.
        [Authorize(Roles = "Admin")]  // Restringe el acceso a usuarios con rol de administrador.
        public async Task<IActionResult> Edit(int id)  // Muestra el formulario para editar un usuario.
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);  // Busca el usuario por su ID.
            if (usuario == null)  // Verifica si el usuario existe.
            {
                TempData["Mensaje"] = "Usuario no encontrado.";  // Mensaje de error si no se encuentra.
                return RedirectToAction("Index");  // Redirige a la lista de usuarios.
            }

            return View(usuario);  // Retorna la vista con el modelo del usuario.
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind] Usuario usuario)
        {
            var temp = await _context.Usuarios.FirstOrDefaultAsync(m => m.IdUsuario == id);
            if (temp != null)
            {
                _context.Usuarios.Remove(temp);
                usuario.IdUsuario = id;
                usuario.Clave = temp.Clave;
                usuario.Restablecer = temp.Restablecer;
                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                // Invalidar cache
                _cache.Remove("ListaUsuarios");
                _cache.Remove($"Usuario_{id}");

                return RedirectToAction("Index", "Usuarios");
            }
            return RedirectToAction("Index", "Usuarios");
        }


        [HttpGet]  // Este método responde a peticiones HTTP GET.
        [Authorize(Roles = "Admin")]  // Restringe el acceso a usuarios con rol de administrador.
        public async Task<IActionResult> Delete(int id)  // Muestra la confirmación para eliminar un usuario.
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);  // Busca el usuario por su ID.
            if (usuario == null)  // Verifica si el usuario existe.
            {
                TempData["Mensaje"] = "Usuario no encontrado.";  // Mensaje de error si no se encuentra.
                return RedirectToAction("Index");  // Redirige a la lista de usuarios.
            }

            return View(usuario);  // Retorna la vista de confirmación.
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                // Invalidar cache
                _cache.Remove("ListaUsuarios");
                _cache.Remove($"Usuario_{id}");
            }

            return RedirectToAction("Index");
        }


        // -------------------- RESTABLECIMIENTO DE CONTRASEÑA --------------------

        [HttpGet]  // Este método responde a peticiones HTTP GET.
        public async Task<IActionResult> Restablecer(string? _email)  // Muestra la vista para restablecer la contraseña.
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == _email);  // Busca el usuario por su email.
            if (usuario == null)  // Verifica si el usuario existe.
            {
                TempData["Mensaje"] = "El usuario no existe.";  // Mensaje de error si no se encuentra.
                return RedirectToAction("Login");  // Redirige a la vista de login.
            }

            SeguridadRestablecer restablecer = new SeguridadRestablecer  // Crea el modelo para la vista.
            {
                EmailRestablecer = usuario.Email  // Asigna el email del usuario al modelo.
            };
            EmailRestablecer = usuario.Email;  // Asigna el email a la variable estática.
            return View(restablecer);  // Retorna la vista con el modelo.
        }

        [HttpPost]  // Este método responde a peticiones HTTP POST.
        [ValidateAntiForgeryToken]  // Protege contra ataques CSRF.
        public async Task<IActionResult> Restablecer([Bind] SeguridadRestablecer pRestablecer)  // Procesa el restablecimiento de la contraseña.
        {
            if (pRestablecer == null)  // Verifica si los datos proporcionados son válidos.
            {
                TempData["Mensaje"] = "Datos incorrectos.";  // Mensaje de error si los datos son incorrectos.
                return View(pRestablecer);  // Retorna la vista con el modelo.
            }

            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.Equals(EmailRestablecer));  // Busca el usuario por su email.
            if (usuario == null || !usuario.Clave.Equals(pRestablecer.Clave))  // Verifica si el usuario existe y la contraseña es correcta.
            {
                TempData["Mensaje"] = "La contraseña actual es incorrecta o el usuario no existe.";  // Mensaje de error si la validación falla.
                return View(pRestablecer);  // Retorna la vista con el modelo.
            }

            if (!pRestablecer.NuevaClave.Equals(pRestablecer.ConfirmarClave))  // Verifica que la nueva contraseña y la confirmación coincidan.
            {
                TempData["Mensaje"] = "La confirmación de la nueva contraseña no coincide.";  // Mensaje de error si no coinciden.
                return View(pRestablecer);  // Retorna la vista con el modelo.
            }

            usuario.Clave = pRestablecer.ConfirmarClave;  // Actualiza la contraseña del usuario.
            usuario.Restablecer = "Realizado";  // Actualiza el estado del restablecimiento.
            _context.Usuarios.Update(usuario);  // Actualiza el usuario en la base de datos.
            await _context.SaveChangesAsync();  // Guarda los cambios de forma asíncrona.

            TempData["Mensaje"] = "Contraseña restablecida correctamente.";  // Mensaje de éxito.
            return RedirectToAction("Login");  // Redirige a la vista de login.
        }
    }

    /*
     Uso de IMemoryCache Resumen

        Se inyecta en el constructor y se usa para almacenar datos en memoria.
        Aplicación de cache en Index y Details

        Index: Se cachea la lista de usuarios por 5 minutos.
        Details: Se cachea cada usuario individual por 10 minutos.
        por último lo que hacemos es la Invalidación del cache

        Cuando se crea, edita o elimina un usuario, se borra el cache correspondiente para mantener los datos actualizados.
     
     */
}