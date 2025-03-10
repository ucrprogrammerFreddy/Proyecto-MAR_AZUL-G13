using System.Text;
using AppUsuarios.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace AppUsuarios.Controllers
{
    public class SeccionesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SeccionesController(IMemoryCache cache, IWebHostEnvironment webHostEnvironment)
        {
            // La clase Conexion gestiona la configuración y retorna una instancia de HttpClient.
            _httpClient = new Conexion().Iniciar();
            _cache = cache;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> MainPage(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Secciones/GetSecciosForId/{id}");

            if (response.IsSuccessStatusCode)
            {
                // Se lee el contenido de la respuesta (JSON) como cadena.
                string json = await response.Content.ReadAsStringAsync();

                // 🔹 Se deserializa el JSON en un solo objeto Secciones en lugar de una lista.
                Secciones seccion = JsonConvert.DeserializeObject<Secciones>(json);

                return View(seccion);
            }

            TempData["Mensaje"] = "Sección no encontrada";
            return RedirectToAction("Index");
        }
        /// <summary>
        public async Task<IActionResult> CargarSecciones()
        {
            try
            {  // Intenta obtener las secciones desde la caché.
                if (!_cache.TryGetValue("SeccionesCache", out List<Secciones> secciones))
                {
                    HttpResponseMessage response = await _httpClient.GetAsync("api/Secciones/GetSecciones");
                    // Verifica si la respuesta de la API fue exitosa.
                    if (response.IsSuccessStatusCode)
                    {// Convierte la respuesta en una lista de objetos Secciones.
                        string json = await response.Content.ReadAsStringAsync();
                        secciones = JsonConvert.DeserializeObject<List<Secciones>>(json);
                        // Almacena las secciones en la caché por 5 minutos.
                        _cache.Set("SeccionesCache", secciones, TimeSpan.FromMinutes(5));
                    }
                    else
                    {
                        // Muestra un mensaje de error si la respuesta de la API no fue exitosa.
                        TempData["Mensaje"] = $"Error al obtener secciones: {response.ReasonPhrase}";
                        secciones = new List<Secciones>();
                    }
                }
                // Devuelve una vista parcial con las secciones.
                return PartialView("_SeccionesPartial", secciones); // Devuelve una vista parcial con las secciones
            }
            catch (Exception ex)
            {
                // Muestra un mensaje de error si se produce una excepción.
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
                return PartialView("_SeccionesPartial", new List<Secciones>());
            }
        }
        // index
        public async Task<IActionResult> Index()
        {
            try
            {
                // Intenta obtener las secciones desde la caché.

                if (!_cache.TryGetValue("SeccionesCache", out List<Secciones> secciones))
                {

                    HttpResponseMessage response = await _httpClient.GetAsync("api/Secciones/GetSecciones");

                    // Se comprueba si la respuesta del API fue exitosa (código HTTP 200).
                    if (response.IsSuccessStatusCode)
                    {
                        // Se lee el contenido de la respuesta (JSON) como cadena.
                        string json = await response.Content.ReadAsStringAsync();


                        // Se deserializa el JSON en una lista de objetos Etiqueta.
                        secciones = JsonConvert.DeserializeObject<List<Secciones>>(json);

                        // Se pasa la lista de etiquetas a la vista para ser mostrada.
                        _cache.Set("SeccionesCache", secciones, TimeSpan.FromMinutes(5));
                        return View(secciones);

                    }
                    else
                    {

                        // Si el API devuelve un error, se almacena el mensaje en TempData.
                        TempData["Mensaje"] = $"Error al obtener etiquetas: {response.ReasonPhrase}";
                        secciones = new List<Secciones>();
                    }

                }
                //retorna las etiquetas obtenidas (ya sea desde caché o API
                return View(secciones);
            }
            catch (Exception ex)
            {
                // Se captura cualquier excepción y se guarda el mensaje para mostrarlo al usuario.
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
                return View(new List<Secciones>());  // En caso de error, se retorna la vista con una lista vacía.

            }
        }

        // BUSCAR Seccion POR ID
        [HttpGet]
        public async Task<IActionResult> BuscarSeccionPorId(int id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"/Secciones/GetSeccionForId/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Seccion no encontrada.";
                    return RedirectToAction("Index");
                }

                // Se lee el contenido de la respuesta (JSON) como cadena.
                string json = await response.Content.ReadAsStringAsync();

                // Se deserializa el JSON en una lista de objetos Etiqueta.
                List<Secciones> secciones = JsonConvert.DeserializeObject<List<Secciones>>(json);

                return View(secciones);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = "Error al buscar la etiqueta: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        /// <summary>
        /// Acción para mostrar los detalles de una seccion.
        /// Realiza una petición GET al endpoint "api/Secciones/GetSeccionesForId/{idSecciones}".
        /// </summary>
        /// <param name="id">Identificador de la etiqueta</param>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Se construye la URL del endpoint con el identificador proporcionado.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Secciones/GetSecciosForId/{id}");

                // Si la respuesta es exitosa, se procede a deserializar el JSON.
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Secciones seccion = JsonConvert.DeserializeObject<Secciones>(json);

                    // Se retorna la vista Details mostrando la información de la etiqueta.
                    return View(seccion);
                }
                else
                {
                    // Si la etiqueta no se encuentra, se almacena un mensaje de error.
                    TempData["Mensaje"] = "Seccion no encontrada";
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
        /// Acción GET para mostrar el formulario de creación de una nueva seccion.
        /// Simplemente retorna la vista donde el usuario puede ingresar datos.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Acción POST para crear una nueva seccion.
        /// Envía una petición POST al endpoint "api/secciones/AddSecciones" con los datos ingresados.
        /// </summary>
        /// <param name="seccion">Objeto seccion con la información a crear</param>
       

        [HttpPost]
        public async Task<IActionResult> Create(Secciones seccion)
        {
            if (!ModelState.IsValid)
                return View(seccion);

            try
            {
                if (seccion.ImagenFile != null && seccion.ImagenFile.Length > 0)
                {
                    // ✅ Directorio de subida
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    // ✅ Crear carpeta si no existe
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    // ✅ Generar un nombre único para evitar colisiones
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(seccion.ImagenFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // ✅ Guardar la imagen en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await seccion.ImagenFile.CopyToAsync(fileStream);
                    }

                    // ✅ Asignar la URL relativa para enviar a la API
                    seccion.ImagenURL = $"/uploads/{uniqueFileName}";
                }

                // ✅ Crear JSON para enviar a la API
                string json = JsonConvert.SerializeObject(seccion);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                // ✅ Enviar la petición POST a la API
                HttpResponseMessage response = await _httpClient.PostAsync("api/Secciones/AddSeccion", content);

                if (response.IsSuccessStatusCode)
                {
                    _cache.Remove("SeccionesCache");
                    TempData["Mensaje"] = "Seccion creada correctamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensaje"] = $"Error al crear seccion: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            return View(seccion);
        }

        /// <summary>
        /// Acción GET para mostrar el formulario de edición de una seccion.
        /// Realiza una petición GET al endpoint "api/Secciones/GetSeccionForId/{id}" para obtener los datos actuales.
        /// </summary>
        /// <param name="id">Identificador de la seccion a editar</param>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Se realiza una petición GET para obtener la seccion a editar.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Secciones/GetSecciosForId/{id}");
                if (response.IsSuccessStatusCode)
                {

                    string json = await response.Content.ReadAsStringAsync();
                    Secciones seccion = JsonConvert.DeserializeObject<Secciones>(json);

                    // Se retorna la vista Edit con los datos de la seccion.
                    return View(seccion);
                }
                else
                {
                    TempData["Mensaje"] = "seccion no encontrada";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // Si ocurre un error, se redirige a la lista de Secciones.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Acción POST para actualizar una seccion.
        /// Envía una petición PUT al endpoint "api/Secciones/Updateseccion/{idseccion}" con los datos modificados.
        /// </summary>
        /// <param name="seccion">Objeto seccion con la información actualizada</param>
        [HttpPost]
        public async Task<IActionResult> Edit(Secciones seccion, IFormFile? ImagenFile)
        {
            if (!ModelState.IsValid)
                return View(seccion);

            try
            {
                if (seccion.IdSeccion == 0)
                {
                    TempData["Mensaje"] = "Error: El ID de la sección no puede ser 0.";
                    return View(seccion);
                }

                // Manejo de la imagen
                if (ImagenFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder); // Asegurar que la carpeta `uploads` exista

                    // Crear un nombre único para la imagen
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagenFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Guardar el archivo en la carpeta `wwwroot/uploads/`
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImagenFile.CopyToAsync(fileStream);
                    }

                    // Asignar la nueva URL de la imagen
                    seccion.ImagenURL = uniqueFileName;
                }

                // Serializar a JSON y enviarlo a la API
                string json = JsonConvert.SerializeObject(seccion);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync($"api/Secciones/UpdateSeccion/{seccion.IdSeccion}", content);
                if (response.IsSuccessStatusCode)
                {
                    _cache.Remove("SeccionesCache");
                    TempData["Mensaje"] = "Sección actualizada correctamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensaje"] = $"Error al actualizar sección: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            return View(seccion);
        }


        /// <summary>
        /// Acción GET para mostrar la confirmación de eliminación de una seccion.
        /// Realiza una petición GET al endpoint "api/Secciones/GetseccionForId/{id}" para mostrar los datos.
        /// </summary>
        /// <param name="id">Identificador de la seccion a eliminar</param>
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Se solicita la  a eliminar mediante el API.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Secciones/GetSecciosForId/{id}");
                if (response.IsSuccessStatusCode)
                {

                    string json = await response.Content.ReadAsStringAsync();
                    Secciones seccion = JsonConvert.DeserializeObject<Secciones>(json);

                    // Se retorna la vista Delete para confirmar la eliminación.
                    return View(seccion);

                }
                else
                {
                    TempData["Mensaje"] = "seccion no encontrada";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // Si ocurre un error, se redirige a la lista de Secciones.
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Acción POST para confirmar y ejecutar la eliminación de una seccion.
        /// Envía una petición DELETE al endpoint "api/seccions/Deleteseccion/{idseccion}".
        /// </summary>
        /// <param name="id">Identificador de la seccion a eliminar</param>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken] // Valida el token anti-CSRF para mayor seguridad
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                // Se envía la petición DELETE para eliminar la seccion especificada.
                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Secciones/DeleteSeccion/{id}");
                if (response.IsSuccessStatusCode)
                {

                    _cache.Remove("SeccionesCache");
                    // Si la eliminación fue exitosa, se guarda un mensaje de confirmación.
                    TempData["Mensaje"] = "seccion eliminada correctamente.";
                }
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"Error al eliminar sección: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // Se redirige a la acción Index, que muestra la lista actualizada de seccions.
            return RedirectToAction("Index");
        }


    }
}
