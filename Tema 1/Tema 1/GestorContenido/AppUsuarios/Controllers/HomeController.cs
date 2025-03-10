using System.Diagnostics;
using System.Net.Http;
using AppUsuarios.Models;
using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppUsuarios.Controllers
{

    //este método responde a solicitudes HTTP GET.
    public class HomeController : Controller
    {  // Declaración de variables para almacenar el registro y el cliente HTTP.
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;

        // Constructor de la clase.
        public HomeController(ILogger<HomeController> logger)
        {
            // Inicializa las variables con los valores proporcionados.
            _httpClient = new Conexion().Iniciar();
            _logger = logger;


            
        
        }
        public IActionResult AboutUs()
        {
            return View();
        }
        public async Task<IActionResult> Index()
        {



            // Listas para almacenar los datos obtenidos de la API.
            List<Articulos> articulos = new List<Articulos>();

            try
            {
                //  Obtener artículos desde la API
                HttpResponseMessage response = await _httpClient.GetAsync("api/Articulos/GetArticuloRelacionado");

                if (response.IsSuccessStatusCode)
                {
                    // Convierte la respuesta en una lista de objetos Articulos.
                    string json = await response.Content.ReadAsStringAsync();
                    articulos = JsonConvert.DeserializeObject<List<Articulos>>(json);
                    articulos = articulos.OrderByDescending(a => a.FechaPublicacion).Take(8).ToList();
                }
                else
                { // Muestra un mensaje de error si la respuesta de la API no fue exitosa.
                    TempData["Mensaje"] = "Error al obtener los artículos de la API.";
                }
            }
            catch (Exception ex)
            { // Muestra un mensaje de error si se produce una excepción.
                TempData["Mensaje"] = $"Excepción al obtener artículos: {ex.Message}";
            }
            //  Devuelve la vista con la lista de artículos.
            return View(articulos);
        }
           
        

        public IActionResult Privacy()
        {
            return View();
        }
        // Método que responde a solicitudes HTTP GET.
        public IActionResult Estadistica() {
            return View();
        }
        //  Método que responde a solicitudes HTTP GET.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {// Devuelve la vista con un modelo de ErrorViewModel.
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        //  Método que responde a solicitudes HTTP GET.
        public async Task<IActionResult> Detalle(int id)
        {
            try
            {//  Obtener el artículo con el ID proporcionado desde la API.  
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Articulos/GetArticuloForId/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Error al obtener el art?culo.";
                    return RedirectToAction("Index");
                }
                // Convierte la respuesta en un objeto ArticuloDTO.
                string json = await response.Content.ReadAsStringAsync();
                ArticuloDTO articuloDTO = JsonConvert.DeserializeObject<ArticuloDTO>(json);

                if (articuloDTO == null)
                {
                    // devuelve un mensaje de error si no se encuentra el artículo.
                    TempData["Mensaje"] = "No se encontr? el art?culo.";
                    return RedirectToAction("Index");
                }

                // Mapear al ViewModel
                ArticuloViewModel model = new ArticuloViewModel
                {
                    IdArticulo = articuloDTO.IdArticulo,
                    Nombre = articuloDTO.Nombre,
                    Descripcion = articuloDTO.Descripcion,
                    Contenido = articuloDTO.Contenido,
                    UrlImagen = articuloDTO.UrlImagen,
                    FechaPublicacion = articuloDTO.FechaPublicacion,
                    Estado = articuloDTO.Estado,

                    // Cargar etiquetas como botones
                    Etiquetas = articuloDTO.IdEtiquetas?.Select(id => new Etiquetas { IdEtiqueta = id, Nombre = $"Etiqueta {id}" }).ToList() ?? new List<Etiquetas>(),

                    // Autores
                    Usuarios = articuloDTO.IdAutores?.Select(id => new Usuario { IdUsuario = id, Nombre = $"Usuario {id}" }).ToList() ?? new List<Usuario>(),

                    // Art?culos relacionados (sin datos completos)
                    ArticulosRelacionadosList = articuloDTO.IdArticulosRelacionados?.Select(id => new Articulos { IdArticulo = id, Nombre = $"Art?culo relacionado {id}" }).ToList() ?? new List<Articulos>()
                };

                return View(model);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error: {ex.Message}";
                return RedirectToAction("Index");
            }
        }
        // Método que responde a solicitudes HTTP GET.
        public IActionResult Dashboard()
        {
            var role = HttpContext.Session.GetString("UserRole");
            // Redirigir a la vista correspondiente según el rol del usuario.
            return role switch
            {
                // Redirigir a la vista correspondiente según el rol del usuario.
                "Administrador" => RedirectToAction("DashboardAdmin", "Home"),
                "Autorizador" => RedirectToAction("DashboardAutorizador", "Home"),
                "Editor" => RedirectToAction("DashboardEditor", "Home"),
                _ => RedirectToAction("Login", "Auth")
            };
        }
    }
}
