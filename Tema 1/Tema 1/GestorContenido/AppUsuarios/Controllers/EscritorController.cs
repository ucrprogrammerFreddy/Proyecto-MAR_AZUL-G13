using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using AppUsuarios.Models;

namespace AppUsuarios.Controllers
{  //este método responde a solicitudes HTTP GET.
    [Authorize(Roles = "Editorial")]
    public class EscritorController : Controller
    {  // Declaración de variables para almacenar la URL de la API y el cliente HTTP.
        private readonly HttpClient _httpClient;
        private readonly string _etiquetasApiUrl;
        // Constructor de la clase.
        public EscritorController(HttpClient httpClient, IConfiguration configuration)
        {// Inicializa las variables con los valores proporcionados.
            _httpClient = httpClient;
            _etiquetasApiUrl = configuration["ApiUrls:EtiquetasApi"]; // API de etiquetas
        }
        // Método que responde a solicitudes HTTP GET.
        [HttpGet]
        [Authorize(Roles = "Escritor")]
        public async Task<IActionResult> Index()
        {
            // Listas para almacenar los datos obtenidos de la API.
            List<Etiquetas>? etiquetas = new List<Etiquetas>();

            try
            {
                // 🔹 Obtener etiquetas desde la API
                Console.WriteLine($"🔹 GET {_etiquetasApiUrl}/GetEtiquetas");
                HttpResponseMessage responseEtiquetas = await _httpClient.GetAsync($"{_etiquetasApiUrl}/GetEtiquetas");

                if (responseEtiquetas.IsSuccessStatusCode)
                {
                    string jsonEtiquetas = await responseEtiquetas.Content.ReadAsStringAsync();
                    etiquetas = JsonConvert.DeserializeObject<List<Etiquetas>>(jsonEtiquetas) ?? new List<Etiquetas>();
                }
                else
                {  // Muestra un mensaje de error si la respuesta de la API no fue exitosa.
                    Console.WriteLine($"⚠️ Error obteniendo etiquetas: {responseEtiquetas.StatusCode} - {responseEtiquetas.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            { // Muestra un mensaje de error si se produce una excepción.
                Console.WriteLine($"❌ Excepción en IndexEditorial: {ex.Message}");
            }

            // 🔹 Crear modelo dinámico para la vista
            dynamic model = new System.Dynamic.ExpandoObject();
            model.Etiquetas = etiquetas;
            //  Devuelve la vista con el modelo dinámico.
            return View("~/Views/Roles/IndexEditorial.cshtml", model);
        }
    }
}
