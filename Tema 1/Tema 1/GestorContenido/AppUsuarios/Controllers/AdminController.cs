using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppUsuarios.Models;
using System.Text;

namespace AppUsuarios.Controllers
{
    [Authorize(Roles = "Administrador")] //este método responde a solicitudes HTTP GET.
    public class AdminController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly string _usuariosApiUrl;
        private readonly string _etiquetasApiUrl;

        public AdminController(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _usuariosApiUrl = configuration["ApiUrls:UsuariosApi"];   //  Obtener desde appsettings.json
            _etiquetasApiUrl = configuration["ApiUrls:EtiquetasApi"]; //  Obtener desde appsettings.json
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {  // // Listas para almacenar los datos obtenidos de la API.
            List<Usuario>? usuarios = new List<Usuario>();
            List<Etiquetas>? etiquetas = new List<Etiquetas>();

            try
            {
                //  Obtener usuarios desde la API (ahora con la URL correcta)
                Console.WriteLine($" GET {_usuariosApiUrl}");
                HttpResponseMessage responseUsuarios = await _httpClient.GetAsync(_usuariosApiUrl);


                // Verifica si la respuesta de la API fue exitosa .
                if (responseUsuarios.IsSuccessStatusCode)
                {
                    string jsonUsuarios = await responseUsuarios.Content.ReadAsStringAsync();
                    usuarios = JsonConvert.DeserializeObject<List<Usuario>>(jsonUsuarios) ?? new List<Usuario>();
                }
                else
                {
                    Console.WriteLine($" Error obteniendo usuarios: {responseUsuarios.StatusCode} - {responseUsuarios.ReasonPhrase}");
                }

                //  Obtener etiquetas desde la API 
                Console.WriteLine($" GET {_etiquetasApiUrl}/GetEtiquetas");
                HttpResponseMessage responseEtiquetas = await _httpClient.GetAsync($"{_etiquetasApiUrl}/GetEtiquetas");

                // Verifica si la respuesta de la API fue exitosa.
                if (responseEtiquetas.IsSuccessStatusCode)
                {
                    string jsonEtiquetas = await responseEtiquetas.Content.ReadAsStringAsync();
                    etiquetas = JsonConvert.DeserializeObject<List<Etiquetas>>(jsonEtiquetas) ?? new List<Etiquetas>();
                }
                else
                {  //    // Si la solicitud falla, muestra un mensaje con el código de error y la razón del fallo.
                    Console.WriteLine($" Error obteniendo etiquetas: {responseEtiquetas.StatusCode} - {responseEtiquetas.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Excepción en IndexAdmin: {ex.Message}");
            }

            //  Crear modelo dinámico para la vista
            dynamic model = new System.Dynamic.ExpandoObject();
            model.Usuarios = usuarios;
            model.Etiquetas = etiquetas;

            return View("~/Views/Roles/IndexAdmin.cshtml", model);
        }

    }
}
