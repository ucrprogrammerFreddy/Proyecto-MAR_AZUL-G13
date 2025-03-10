
using AppUsuarios.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;


namespace AppUsuarios.Controllers
{
    [Route("Usuarios")]
    public class UsuariosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly string _apiBaseUrl;
        private readonly string _apiLoginBaseUrl; //  Nueva API para autenticación
        private readonly DbContextGestionContenido _context;

        public UsuariosController(IHttpClientFactory httpClientFactory, IMemoryCache cache, IConfiguration configuration, DbContextGestionContenido context)
        {
            _context = context;
            _httpClient = new Conexion().Iniciar();
            _cache = cache;
            _apiBaseUrl = configuration["ApiUrls:UsuariosApi"]?.TrimEnd('/') ?? string.Empty;
            _apiLoginBaseUrl = configuration["ApiUrls:LoginAuthApi"]?.TrimEnd('/') ?? string.Empty; // ✅ Ahora usamos la API de login

            if (string.IsNullOrEmpty(_apiBaseUrl))
                throw new Exception("❌ ERROR: La URL base de la API de gestión de usuarios no está configurada en appsettings.json.");

            if (string.IsNullOrEmpty(_apiLoginBaseUrl))
                throw new Exception("❌ ERROR: La URL base de la API de autenticación no está configurada en appsettings.json.");
        }

        ///  **Mostrar formulario de Login**
        [HttpGet("Login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        ///  **Procesar login**
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string clave, [FromForm] string returnUrl = null)
        {
            Console.WriteLine($"📤 Intentando login con Email: '{email}'");

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(clave))
            {
                Console.WriteLine("⚠️ Email o clave vacíos.");
                ViewData["Error"] = "⚠️ Email y contraseña son obligatorios.";
                return View();
            }

            //  Normalizar email antes de enviarlo
            email = email.Trim().ToLower();

            string apiUrl = $"{_apiLoginBaseUrl}/Login";
            var loginRequest = new { Email = email, Clave = clave };

            try
            {
                var response = await _httpClient.PostAsync(apiUrl,
                    new StringContent(JsonConvert.SerializeObject(loginRequest), Encoding.UTF8, "application/json"));

                string jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"🔹 Respuesta completa de la API: {jsonResponse}");

                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("❌ Usuario o clave incorrectos.");
                    ViewData["Error"] = "❌ Usuario o clave incorrectos.";
                    return View();
                }

                dynamic result = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                if (result == null || result.usuario == null)
                {
                    Console.WriteLine("⚠️ Error: La API no devolvió un usuario válido.");
                    ViewData["Error"] = "⚠️ Error en la respuesta del servidor.";
                    return View();
                }

                if (result.usuario.estado.ToString() != "Activo")
                {
                    Console.WriteLine("❌ Usuario inactivo.");
                    ViewData["Error"] = "❌ Tu cuenta aún no ha sido activada por un administrador.";
                    return View();
                }

                string role = result.usuario.rol.ToString().Trim();
                if (role == "administrador") role = "Administrador";
                if (role == "autorizador") role = "Autorizador";
                if (role == "escritor") role = "Escritor";

                Console.WriteLine($"✅ Rol asignado: {role}");

                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, result.usuario.email.ToString()),
            new Claim(ClaimTypes.Email, result.usuario.email.ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim("UserId", result.usuario.idUsuario.ToString())
        };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                Console.WriteLine($"✅ Usuario autenticado: {result.usuario.email}");

                return role switch
                {
                    "Administrador" => RedirectToAction("Index", "Admin"),
                    "Autorizador" => RedirectToAction("Index", "Autorizador"),
                    "Escritor" => RedirectToAction("Index", "Escritor"),
                    _ => RedirectToAction("Login")
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en Login: {ex.Message}");
                ViewData["Error"] = "❌ Error al conectar con el servidor.";
                return View();
            }
        }






















        [HttpGet("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost("ToggleStatus/{id}")]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            try
            {
                string url = $"{_apiBaseUrl}/{id}";
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode) return BadRequest("Usuario no encontrado.");

                var usuario = JsonConvert.DeserializeObject<Usuario>(await response.Content.ReadAsStringAsync());
                if (usuario == null) return BadRequest("Usuario no encontrado.");

                // Alternar estado
                usuario.Estado = usuario.Estado == "Activo" ? "Inactivo" : "Activo";

                var json = JsonConvert.SerializeObject(new
                {
                    usuario.IdUsuario,
                    usuario.Nombre,
                    usuario.Email,
                    usuario.Clave,
                    usuario.Rol,
                    usuario.Estado
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                response = await _httpClient.PutAsync(url, content);

                if (!response.IsSuccessStatusCode) return BadRequest("Error al actualizar usuario.");

                return Json(new { estado = usuario.Estado, id = usuario.IdUsuario });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en ToggleStatus: {ex.Message}");
                return BadRequest("Error interno.");
            }
        }









        /// ✅ **Cerrar sesión**
        [HttpGet("Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete(".AspNetCore.Cookies");
            TempData["Mensaje"] = "✅ Sesión cerrada correctamente.";
            return RedirectToAction("Login");
        }



      

        public async Task<IActionResult> Index()
        {
            try
            {

                if (!_cache.TryGetValue("UsuariosCache", out List<Usuario> usuarios))
                {

                    HttpResponseMessage response = await _httpClient.GetAsync("api/Usuario");

                    // Se comprueba si la respuesta del API fue exitosa (código HTTP 200).
                    if (response.IsSuccessStatusCode)
                    {
                        // Se lee el contenido de la respuesta (JSON) como cadena.
                        string json = await response.Content.ReadAsStringAsync();


                        // Se deserializa el JSON en una lista de objetos Etiqueta.
                        usuarios = JsonConvert.DeserializeObject<List<Usuario>>(json);

                        // Se pasa la lista de etiquetas a la vista para ser mostrada.
                        _cache.Set("UsuariosCache", usuarios, TimeSpan.FromMinutes(5));
                        return View(usuarios);

                    }
                    else
                    {

                        // Si el API devuelve un error, se almacena el mensaje en TempData.
                        TempData["Mensaje"] = $"Error al obtener Categorias: {response.ReasonPhrase}";
                        usuarios = new List<Usuario>();
                    }

                }
                //retorna las etiquetas obtenidas (ya sea desde caché o API
                return View(usuarios);
            }
            catch (Exception ex)
            {
                // Se captura cualquier excepción y se guarda el mensaje para mostrarlo al usuario.
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
                return View(new List<Usuario>());  // En caso de error, se retorna la vista con una lista vacía.

            }
        }




        /// ✅ **Mostrar formulario de creación**
        [HttpGet("Create")]
        public IActionResult Create() => View();

        /// ✅ **Crear un usuario**
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("⚠️ Error en ModelState: Datos faltantes o inválidos.");
                return View(usuario);
            }

            try
            {
                usuario.Estado = "Inactivo";

                string url = $"{_apiBaseUrl}/PostUsuario";
                var json = JsonConvert.SerializeObject(usuario);

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    _cache.Remove("UsuariosCache");

                    if (User.Identity.IsAuthenticated && User.IsInRole("Administrador"))
                    {
                        TempData["Mensaje"] = "✅ Usuario creado correctamente.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["Mensaje"] = "✅ Solicitud enviada. Un administrador debe aprobar tu cuenta.";
                        return RedirectToAction("Login");
                    }
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"⚠️ Error en Create: {response.StatusCode} - {errorMessage}");
                TempData["Mensaje"] = $"⚠️ Error al solicitar la cuenta: {errorMessage}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en Create: {ex.Message}");
                TempData["Mensaje"] = $"❌ Excepción: {ex.Message}";
            }

            return View(usuario);
        }







        /// ✅ **Eliminar usuario**
        [HttpPost("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            try
            {
                string url = $"{_apiBaseUrl}/{id}";
                Console.WriteLine($"🔹 DELETE {url}");

                HttpResponseMessage response = await _httpClient.DeleteAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _cache.Remove("UsuariosCache");
                    TempData["Mensaje"] = "✅ Usuario eliminado correctamente.";
                }
                else
                {
                    Console.WriteLine($"⚠️ Error en Delete: {response.StatusCode} - {response.ReasonPhrase}");
                    TempData["Mensaje"] = $"⚠️ Error al eliminar usuario: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en Delete: {ex.Message}");
                TempData["Mensaje"] = $"❌ Excepción: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        [HttpGet("Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                string url = $"{_apiBaseUrl}/{id}"; // ✅ URL Final Correcta

                Console.WriteLine($"🔹 GET {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var usuario = JsonConvert.DeserializeObject<Usuario>(json);
                    return usuario != null ? View(usuario) : RedirectToAction("Index");
                }

                Console.WriteLine($"⚠️ Error en Details GET: {response.StatusCode} - {response.ReasonPhrase}");
                TempData["Mensaje"] = "⚠️ Usuario no encontrado.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en Details GET: {ex.Message}");
                TempData["Mensaje"] = $"❌ Excepción: {ex.Message}";
            }

            return RedirectToAction("Index");
        }


        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                string url = $"{_apiBaseUrl}/{id}"; // ✅ URL Final Correcta

                Console.WriteLine($"🔹 GET {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    var usuario = JsonConvert.DeserializeObject<Usuario>(json);
                    return usuario != null ? View(usuario) : RedirectToAction("Index");
                }

                Console.WriteLine($"⚠️ Error en Edit GET: {response.StatusCode} - {response.ReasonPhrase}");
                TempData["Mensaje"] = "⚠️ Usuario no encontrado.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en Edit GET: {ex.Message}");
                TempData["Mensaje"] = $"❌ Excepción: {ex.Message}";
            }

            return RedirectToAction("Index");
        }



        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Usuario usuario)
        {
            if (!ModelState.IsValid)
                return View(usuario);

            try
            {
                string url = $"{_apiBaseUrl}/{id}"; // ✅ URL Final Correcta

                Console.WriteLine($"🔹 PUT {url}");

                var json = JsonConvert.SerializeObject(usuario, new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    _cache.Remove("UsuariosCache");
                    TempData["Mensaje"] = "✅ Usuario actualizado correctamente.";
                    return RedirectToAction("Index");
                }

                Console.WriteLine($"⚠️ Error en Edit POST: {response.StatusCode} - {response.ReasonPhrase}");
                TempData["Mensaje"] = $"⚠️ Error al actualizar usuario: {response.ReasonPhrase}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Excepción en Edit POST: {ex.Message}");
                TempData["Mensaje"] = $"❌ Excepción: {ex.Message}";
            }

            return View(usuario);
        }
    }
}
