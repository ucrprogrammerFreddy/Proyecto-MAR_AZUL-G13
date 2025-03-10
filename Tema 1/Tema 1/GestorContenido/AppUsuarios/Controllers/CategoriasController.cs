using System.Text;
using AppUsuarios.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using static System.Collections.Specialized.BitVector32;

namespace AppUsuarios.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IMemoryCache _cache;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoriasController(IMemoryCache cache, IWebHostEnvironment webHostEnvironment)
        {
            // La clase Conexion gestiona la configuración y retorna una instancia de HttpClient.
            _httpClient = new Conexion().Iniciar();
            _cache = cache;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: CategoriasController
        public async Task<IActionResult> MainPage(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"api/Categorias/GetCategoriaForId/{id}");

            if (response.IsSuccessStatusCode)
            {
                // Se lee el contenido de la respuesta (JSON) como cadena.
                string json = await response.Content.ReadAsStringAsync();

                // 🔹 Se deserializa el JSON en un solo objeto Categoriaes en lugar de una lista.
                Categorias categoria = JsonConvert.DeserializeObject<Categorias>(json);

                return View(categoria);
            }

            TempData["Mensaje"] = "Categoria no encontrada";
            return RedirectToAction("Index");
        }

        // GET: CategoriasController
        public async Task<IActionResult> Index()
        {
            try
            {
                // Se intenta obtener la lista de etiquetas desde la caché.
                if (!_cache.TryGetValue("CategoriasCache", out List<Categorias> categorias))
                {

                    HttpResponseMessage response = await _httpClient.GetAsync("api/Categorias/GetCategorias");

                    // Se comprueba si la respuesta del API fue exitosa (código HTTP 200).
                    if (response.IsSuccessStatusCode)
                    {
                        // Se lee el contenido de la respuesta (JSON) como cadena.
                        string json = await response.Content.ReadAsStringAsync();


                        // Se deserializa el JSON en una lista de objetos Etiqueta.
                        categorias = JsonConvert.DeserializeObject<List<Categorias>>(json);

                        // Se pasa la lista de etiquetas a la vista para ser mostrada.
                        _cache.Set("CategoriasCache", categorias, TimeSpan.FromMinutes(5));
                        return View(categorias);

                    }
                    else
                    {

                        // Si el API devuelve un error, se almacena el mensaje en TempData.
                        TempData["Mensaje"] = $"Error al obtener Categorias: {response.ReasonPhrase}";
                        categorias = new List<Categorias>();
                    }

                }
                //retorna las etiquetas obtenidas (ya sea desde caché o API
                return View(categorias);
            }
            catch (Exception ex)
            {
                // Se captura cualquier excepción y se guarda el mensaje para mostrarlo al usuario.
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
                return View(new List<Categorias>());  // En caso de error, se retorna la vista con una lista vacía.

            }
        }

        
        // BUSCAR Categoria POR ID
        [HttpGet]
        public async Task<IActionResult> BuscarCategoriaPorId(int id)
        {
            try
            {  // Se construye la URL del endpoint con el identificador proporcionado.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Categorias/GetCategoriaForId/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Mensaje"] = "Categoria no encontrada.";
                    return RedirectToAction("Index");
                }

                // Se lee el contenido de la respuesta (JSON) como cadena.
                string json = await response.Content.ReadAsStringAsync();

                // Se deserializa el JSON en una lista de objetos Etiqueta.
                List<Categorias> Categorias = JsonConvert.DeserializeObject<List<Categorias>>(json);

                return View(Categorias);
            }
            catch (Exception ex)
            {  // Se captura cualquier excepción y se guarda el mensaje para mostrarlo al usuario.
                TempData["Mensaje"] = "Error al buscar la etiqueta: " + ex.Message;
                return RedirectToAction("Index");
            }
        }
        // GET: CategoriasController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                // Se construye la URL del endpoint con el identificador proporcionado.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/categorias/GetCategoriaForId/{id}");

                // Si la respuesta es exitosa, se procede a deserializar el JSON.
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Categorias categoria = JsonConvert.DeserializeObject<Categorias>(json);

                    // Se retorna la vista Details mostrando la información de la etiqueta.
                    return View(categoria);
                }
                else
                {
                    // Si la etiqueta no se encuentra, se almacena un mensaje de error.
                    TempData["Mensaje"] = "categoria no encontrada";
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
        /// Acción GET para mostrar el formulario de creación de una nueva categoria.
        /// Simplemente retorna la vista donde el usuario puede ingresar datos.
        /// </summary>
        /// 


        // responde a solicitudes HTTP GET.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {  // Se obtienen las secciones desde la API.
                HttpResponseMessage response = await _httpClient.GetAsync("api/Secciones/GetSecciones");
                // Se verifica si la respuesta fue exitosa.
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    List<Secciones> secciones = JsonConvert.DeserializeObject<List<Secciones>>(json);

                    // Enviamos la lista de secciones a la vista usando ViewBag
                    ViewBag.IdSeccion = new SelectList(secciones, "IdSeccion", "Nombre");
                }
                else
                { // Si la respuesta no fue exitosa, se envía una lista vacía.
                    ViewBag.IdSeccion = new SelectList(new List<Secciones>(), "IdSeccion", "Nombre");
                    TempData["Mensaje"] = "Error al obtener las secciones.";
                }
            }// Si ocurre una excepción, se muestra un mensaje de error.
            catch (Exception ex)
            {
                // Se muestra un mensaje de error en TempData.
                ViewBag.IdSeccion = new SelectList(new List<Secciones>(), "IdSeccion", "Nombre");
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            return View();
        }
        // POST: CategoriasController/Create
        [HttpPost]
        public async Task<IActionResult> Create(Categorias categoria)
        {
            if (!ModelState.IsValid)
                return View(categoria);
            // Se intenta enviar la nueva categoria a la API.
            try
            {
                if (categoria.ImagenFile != null && categoria.ImagenFile.Length > 0)
                {
                    // Directorio de subida
                    string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                    //  Crear carpeta si no existe
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    //  Generar un nombre único para evitar colisiones
                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(categoria.ImagenFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    //  Guardar la imagen en el servidor
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await categoria.ImagenFile.CopyToAsync(fileStream);
                    }

                    //  Asignar la URL relativa para enviar a la API
                    categoria.UrlImagen = $"/uploads/{uniqueFileName}";
                }

                //  Crear JSON para enviar a la API
                string json = JsonConvert.SerializeObject(categoria);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                //  Enviar la petición POST a la API
                HttpResponseMessage response = await _httpClient.PostAsync("api/Categorias/AddCategoria", content);

                if (response.IsSuccessStatusCode)
                {
                    _cache.Remove("CategoriasCache");
                    TempData["Mensaje"] = "Categoría creada correctamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensaje"] = $"Error al crear categoría: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            return View(categoria);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Categorias/GetCategoriaForId/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();
                    Categorias categoria = JsonConvert.DeserializeObject<Categorias>(json);

                    await CargarSecciones(); // Recargar las secciones antes de enviar la vista

                    return View(categoria);
                }
                else
                {
                    TempData["Mensaje"] = "Categoría no encontrada";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Categorias categoria, IFormFile? ImagenFile)
        {
            if (!ModelState.IsValid)
            {
                await CargarSecciones();
                return View(categoria);
            }

            try
            {
                if (categoria.IdCategoria == 0)
                {
                    TempData["Mensaje"] = "Error: El ID de la categoría no puede ser 0.";
                    await CargarSecciones();
                    return View(categoria);
                }

                if (ImagenFile != null)
                {
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagenFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImagenFile.CopyToAsync(fileStream);
                    }

                    categoria.UrlImagen = $"/uploads/{uniqueFileName}";
                }
                else
                {
                    // Si no se subió nueva imagen, mantener la anterior
                    HttpResponseMessage getResponse = await _httpClient.GetAsync($"api/Categorias/GetCategoriaForId/{categoria.IdCategoria}");
                    if (getResponse.IsSuccessStatusCode)
                    {
                        string jsonresult = await getResponse.Content.ReadAsStringAsync();
                        Categorias categoriaActual = JsonConvert.DeserializeObject<Categorias>(jsonresult);
                        categoria.UrlImagen = categoriaActual.UrlImagen; // Mantener la imagen existente
                    }
                }
                // Serializar la categoria a JSON
                string json = JsonConvert.SerializeObject(categoria);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync($"api/Categorias/UpdateCategoria/{categoria.IdCategoria}", content);
                if (response.IsSuccessStatusCode)
                {
                    // 🔹 INVALIDAR CACHÉ DESPUÉS DE LA ACTUALIZACIÓN
                    _cache.Remove("CategoriasCache");
                    // Si la actualización fue exitosa, se muestra un mensaje de confirmación.
                    TempData["Mensaje"] = "Categoría actualizada correctamente.";
                    return RedirectToAction("Index");
                }
                else
                {  // Si la API devuelve un error, se muestra el mensaje de error.
                    TempData["Mensaje"] = $"Error al actualizar categoría: {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {// Si se produce una excepción, se muestra el mensaje de error.
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            await CargarSecciones();
            return View(categoria);
        }


        // Método para cargar las secciones en el ViewBag
        private async Task CargarSecciones()
        {  // Se obtienen las secciones desde la API.
            HttpResponseMessage response = await _httpClient.GetAsync("api/Secciones/GetSecciones");
            if (response.IsSuccessStatusCode)
            { // Se lee el contenido de la respuesta (JSON) como cadena.
                string json = await response.Content.ReadAsStringAsync();
                List<Secciones> secciones = JsonConvert.DeserializeObject<List<Secciones>>(json);
                ViewBag.IdSeccion = new SelectList(secciones, "IdSeccion", "Nombre");
            }
            else
            {// Si la respuesta no fue exitosa, se envía una lista vacía.
                ViewBag.IdSeccion = new SelectList(new List<Secciones>(), "IdSeccion", "Nombre");
            }
        }


        // GET: CategoriasController/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                // Se solicita la  a eliminar mediante el API.
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Categorias/GetCategoriaForId/{id}");
                if (response.IsSuccessStatusCode)
                {

                    string json = await response.Content.ReadAsStringAsync();
                    Categorias seccion = JsonConvert.DeserializeObject<Categorias>(json);

                    // Se retorna la vista Delete para confirmar la eliminación.
                    return View(seccion);

                }
                else
                {
                    TempData["Mensaje"] = "Categoria no encontrada";
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
            }

            // Si ocurre un error, se redirige a la lista de Categorias.
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
                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Categorias/DeleteCategoria/{id}");
                if (response.IsSuccessStatusCode)
                {

                    _cache.Remove("CategoriasCache");
                    // Si la eliminación fue exitosa, se guarda un mensaje de confirmación.
                    TempData["Mensaje"] = "categoria eliminada correctamente.";
                }
                else
                {// Si la API devuelve un error, se muestra el mensaje de error.
                    TempData["Mensaje"] = $"Error al eliminar categoria: {response.ReasonPhrase}";
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
