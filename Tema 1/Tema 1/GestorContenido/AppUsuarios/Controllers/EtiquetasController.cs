using AppUsuarios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace AppUsuarios.Controllers
{
    // Este controlador se encarga de consumir el API de Etiquetas.
    // Hereda de Controller, lo que permite retornar vistas.
    public class EtiquetasController : Controller
    {
        // Declaramos una instancia de HttpClient para realizar peticiones al API.
        // Se utiliza una instancia única (obtenida desde la clase Conexion) para evitar problemas de recursos.
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;

        // Constructor del controlador.
        // Se inicializa el HttpClient mediante la clase Conexion.
        public EtiquetasController(IMemoryCache cache)
        {
            // La clase Conexion gestiona la configuración y retorna una instancia de HttpClient.
            _httpClient = new Conexion().Iniciar();
            _cache = cache; 
        }

        /// <summary>
        /// Acción para listar todas las etiquetas.
        /// Realiza una petición GET al endpoint "api/Etiquetas/GetEtiquetas" del API.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {

                if (!_cache.TryGetValue("EtiquetasCache", out List<Etiquetas> etiquetas))
                { 
                
                        // Se realiza una petición GET al API para obtener todas las etiquetas.
                        HttpResponseMessage response = await _httpClient.GetAsync("api/Etiquetas/GetEtiquetas");

                        // Se comprueba si la respuesta del API fue exitosa (código HTTP 200).
                        if (response.IsSuccessStatusCode)
                        {
                            // Se lee el contenido de la respuesta (JSON) como cadena.
                            string json = await response.Content.ReadAsStringAsync();
                           

                            // Se deserializa el JSON en una lista de objetos Etiqueta.
                            etiquetas = JsonConvert.DeserializeObject<List<Etiquetas>>(json);

                            // Se pasa la lista de etiquetas a la vista para ser mostrada.
                                _cache.Set("EtiquetasCache", etiquetas, TimeSpan.FromMinutes(5));
                            return View(etiquetas);
                        
                        }
                        else
                        {

                            // Si el API devuelve un error, se almacena el mensaje en TempData.
                            TempData["Mensaje"] = $"Error al obtener etiquetas: {response.ReasonPhrase}";
                           etiquetas = new List<Etiquetas>();
                        }

                }
                //retorna las etiquetas obtenidas (ya sea desde caché o API
                 return View(etiquetas);
            }
            catch (Exception ex)
            {
                // Se captura cualquier excepción y se guarda el mensaje para mostrarlo al usuario.
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
                return View(new List<Etiquetas>());  // En caso de error, se retorna la vista con una lista vacía.

            }
        }



        // BUSCAR ETIQUETA POR ID
        [HttpGet]
        public async Task<IActionResult> BuscarEtiquetaPorId(int id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"/Etiquetas/GetEtiquetaForId/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Etiqueta no encontrada.";
                    return RedirectToAction("Index");
                }

                // Se lee el contenido de la respuesta (JSON) como cadena.
                string json = await response.Content.ReadAsStringAsync();

                // Se deserializa el JSON en una lista de objetos Etiqueta.
                List<Etiquetas> etiquetas = JsonConvert.DeserializeObject<List<Etiquetas>>(json);

                return View(etiquetas);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al buscar la etiqueta: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        //Pendiente en api si da, en consumo no.


        /// <summary>
        /// Acción para mostrar los detalles de una etiqueta.
        /// Realiza una petición GET al endpoint "api/Etiquetas/GetEtiquetaForId/{idEtiqueta}".
        /// </summary>
        /// <param name="id">Identificador de la etiqueta</param>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Se construye la URL del endpoint con el identificador proporcionado.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Etiquetas/GetEtiquetaForId/{id}");

                // Si la respuesta es exitosa, se procede a deserializar el JSON.
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Etiquetas etiqueta = JsonConvert.DeserializeObject<Etiquetas>(json);

                    // Se retorna la vista Details mostrando la información de la etiqueta.
                    return View(etiqueta);
                }
                else
                {
                    // Si la etiqueta no se encuentra, se almacena un mensaje de error.
                    TempData["Mensaje"] = "Etiqueta no encontrada";
                }
            }
            catch (Exception ex)
            {
                // Capturamos y mostramos cualquier excepción ocurrida.
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // En caso de error, redirige a la acción Index para listar etiquetas.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Acción GET para mostrar el formulario de creación de una nueva etiqueta.
        /// Simplemente retorna la vista donde el usuario puede ingresar datos.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Acción POST para crear una nueva etiqueta.
        /// Envía una petición POST al endpoint "api/Etiquetas/AddEtiqueta" con los datos ingresados.
        /// </summary>
        /// <param name="etiqueta">Objeto Etiqueta con la información a crear</param>
        [HttpPost]
        public async Task<IActionResult> Create(Etiquetas etiqueta)
        {
            // Se verifica que los datos enviados cumplan las validaciones del modelo.
            if (!ModelState.IsValid)
                return View(etiqueta);

            try
            {
                // Se convierte el objeto etiqueta a formato JSON.
                string json = JsonConvert.SerializeObject(etiqueta);

                // Se crea el contenido de la petición con el JSON, indicando el tipo de contenido "application/json".
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                // Se envía la petición POST al API para agregar la nueva etiqueta.
                HttpResponseMessage response = await _httpClient.PostAsync("api/Etiquetas/AddEtiqueta", content);

                if (response.IsSuccessStatusCode)
                {

                    _cache.Remove("EtiquetasCache");
                    // Si la creación es exitosa, se guarda un mensaje y se redirige a la lista de etiquetas.
                    TempData["Mensaje"] = "Etiqueta creada correctamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    // Si ocurre un error, se guarda el mensaje devuelto por el API.
                    TempData["Mensaje"] = $"Error al crear etiqueta: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                // Captura excepciones y guarda el mensaje para el usuario.
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // En caso de error, se retorna la misma vista para corregir los datos ingresados.
            return View(etiqueta);
        }

        /// <summary>
        /// Acción GET para mostrar el formulario de edición de una etiqueta.
        /// Realiza una petición GET al endpoint "api/Etiquetas/GetEtiquetaForId/{id}" para obtener los datos actuales.
        /// </summary>
        /// <param name="id">Identificador de la etiqueta a editar</param>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Se realiza una petición GET para obtener la etiqueta a editar.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Etiquetas/GetEtiquetaForId/{id}");
                if (response.IsSuccessStatusCode)
                {
                 
                    string json = await response.Content.ReadAsStringAsync();
                    Etiquetas etiqueta = JsonConvert.DeserializeObject<Etiquetas>(json);

                    // Se retorna la vista Edit con los datos de la etiqueta.
                    return View(etiqueta);
                }
                else
                {
                    TempData["Mensaje"] = "Etiqueta no encontrada";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // Si ocurre un error, se redirige a la lista de etiquetas.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Acción POST para actualizar una etiqueta.
        /// Envía una petición PUT al endpoint "api/Etiquetas/UpdateEtiqueta/{idEtiqueta}" con los datos modificados.
        /// </summary>
        /// <param name="etiqueta">Objeto Etiqueta con la información actualizada</param>
        [HttpPost]
        public async Task<IActionResult> Edit(Etiquetas etiqueta)
        {
            // Se valida que el modelo cumpla con los requisitos definidos.
            if (!ModelState.IsValid)
                return View(etiqueta);

            try
            {
                // Se serializa el objeto actualizado a formato JSON.
                string json = JsonConvert.SerializeObject(etiqueta);
                if (etiqueta.IdEtiqueta == 0)
                {
                    TempData["Mensaje"] = "Error: El ID de la etiqueta es inválido no puede ser 0.";
                    return View(etiqueta);
                }

                // Se prepara el contenido de la petición PUT.
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                
                // Se envía la petición PUT al API para actualizar la etiqueta.
                HttpResponseMessage response = await _httpClient.PutAsync($"api/Etiquetas/UpdateEtiqueta/{etiqueta.IdEtiqueta}", content);
                if (response.IsSuccessStatusCode)
                {
                    // Si la actualización fue exitosa, se guarda un mensaje y se redirige a Index.
                    TempData["Mensaje"] = "Etiqueta actualizada correctamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensaje"] = $"Error al actualizar etiqueta: {response.ReasonPhrase}";
                }
                
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // Si se produce un error, se retorna la vista Edit con el objeto para corregirlo.
            return View(etiqueta);
        }

        /// <summary>
        /// Acción GET para mostrar la confirmación de eliminación de una etiqueta.
        /// Realiza una petición GET al endpoint "api/Etiquetas/GetEtiquetaForId/{id}" para mostrar los datos.
        /// </summary>
        /// <param name="id">Identificador de la etiqueta a eliminar</param>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Se solicita la etiqueta a eliminar mediante el API.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Etiquetas/GetEtiquetaForId/{id}");
                if (response.IsSuccessStatusCode)
                {
                   
                    string json = await response.Content.ReadAsStringAsync();
                    Etiquetas etiqueta = JsonConvert.DeserializeObject<Etiquetas>(json);

                    // Se retorna la vista Delete para confirmar la eliminación.
                    return View(etiqueta);

                }
                else
                {
                    TempData["Mensaje"] = "Etiqueta no encontrada";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // Si ocurre un error, se redirige a la lista de etiquetas.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Acción POST para confirmar y ejecutar la eliminación de una etiqueta.
        /// Envía una petición DELETE al endpoint "api/Etiquetas/DeleteEtiqueta/{idEtiqueta}".
        /// </summary>
        /// <param name="id">Identificador de la etiqueta a eliminar</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Valida el token anti-CSRF para mayor seguridad
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Se envía la petición DELETE para eliminar la etiqueta especificada.
                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Etiquetas/DeleteEtiqueta/{id}");
                if (response.IsSuccessStatusCode)
                {

                    _cache.Remove("EtiquetasCache");
                    // Si la eliminación fue exitosa, se guarda un mensaje de confirmación.
                    TempData["Mensaje"] = "Etiqueta eliminada correctamente.";
                }
                else
                {
                    TempData["Mensaje"] = $"Error al eliminar etiqueta: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // Se redirige a la acción Index, que muestra la lista actualizada de etiquetas.
            return RedirectToAction("Index");
        }


    }
}
