using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AppUsuarios.DTOS;
using AppUsuarios.Models;
using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppUsuarios.Controllers
{
    public class ArticulosController : Controller
    {
        private readonly HttpClient _httpClient; // Cliente HTTP para comunicarse con la API
        private readonly IWebHostEnvironment _webHostEnvironment;// Para acceder a la carpeta wwwroot
        private readonly ILogger<ArticulosController> _logger;  // Para registrar mensajes de log
        public ArticulosController(IWebHostEnvironment webHostEnvironment, ILogger<ArticulosController> logger)
        {
            // La clase Conexion() debe retornar un HttpClient configurado para comunicarte con la API.
            _httpClient = new Conexion().Iniciar();
            _webHostEnvironment = webHostEnvironment;

            _logger = logger;


        }
        //------------------------------------------------------------------------------------------------------------------------------
        // GET: Articulos/Index


        public async Task<IActionResult> Index()
        {

            try
            {    // Llamar a la API para obtener los artículos relacionados
                HttpResponseMessage response = await _httpClient.GetAsync("api/Articulos/GetArticuloRelacionado");
                if (response.IsSuccessStatusCode)
                {
                    string json = await response.Content.ReadAsStringAsync();

                    //  Verificar el JSON recibido
                    System.Diagnostics.Debug.WriteLine("JSON recibido: " + json);

                    //  Deserializar en la estructura correcta
                    List<Articulos> articulos = JsonConvert.DeserializeObject<List<Articulos>>(json) ?? new List<Articulos>();

                    //  Verificar si se deserializaron correctamente
                    System.Diagnostics.Debug.WriteLine("Total artículos deserializados: " + articulos.Count);

                    if (articulos.Any())
                    {
                        return View(articulos);
                    }
                    else
                    {  //  Mostrar mensaje si no se encontraron artículos
                        TempData["Mensaje"] = "No se encontraron artículos.";
                        return View(new List<Articulos>());
                    }
                }
                else
                {
                    //  Mostrar mensaje de error si no se pudo obtener la información
                    TempData["Mensaje"] = "Error al obtener los artículos. Código: " + response.StatusCode;
                    return View(new List<Articulos>());
                }
            }
            catch (Exception ex)
            {
                //  Mostrar mensaje si ocurre una excepción
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
                return View(new List<Articulos>());
            }
        }







        //------------------------------------------------------------------------------------------------------------------------------
        // GET: Articulos/Create
        public async Task<IActionResult> Create()
        {
            ArticuloViewModel model = new ArticuloViewModel();

            try
            {
                // Obtener Categorías desde la API

                HttpResponseMessage responseCategorias = await _httpClient.GetAsync("api/Articulos/GetCategorias");
                if (responseCategorias.IsSuccessStatusCode)
                {
                    string jsonCategorias = await responseCategorias.Content.ReadAsStringAsync();
                    model.Categorias = JsonConvert.DeserializeObject<List<Categorias>>(jsonCategorias);
                }

                // Obtener Etiquetas
                HttpResponseMessage responseEtiquetas = await _httpClient.GetAsync("api/Articulos/GetEtiquetas");
                if (responseEtiquetas.IsSuccessStatusCode)
                {
                    // Deserializar el JSON en una lista de Etiquetas
                    string jsonEtiquetas = await responseEtiquetas.Content.ReadAsStringAsync();
                    model.Etiquetas = JsonConvert.DeserializeObject<List<Etiquetas>>(jsonEtiquetas);
                }

             

                // obtener autores
                HttpResponseMessage responseAutores = await _httpClient.GetAsync("api/Articulos/GetAutores");
                if (responseAutores.IsSuccessStatusCode)
                {
                    string jsonAutores = await responseAutores.Content.ReadAsStringAsync();

                    // Deserializar como UsuarioDTO primero
                    List<UsuarioDTO> usuariosDTO = JsonConvert.DeserializeObject<List<UsuarioDTO>>(jsonAutores);

                    // Convertir UsuarioDTO a Usuario antes de enviarlo a la vista
                    model.Usuarios = usuariosDTO.Select(u => new Usuario
                    {
                        IdUsuario = u.IdUsuario,
                        Nombre = u.Nombre,
                        Email = u.Email,
                    }).ToList();
                }






                // Obtener Autorizadores (se mostrará el rol en la vista create )
                HttpResponseMessage responseAutorizadores = await _httpClient.GetAsync("api/Articulos/GetAutorizadores");
                if (responseAutorizadores.IsSuccessStatusCode)
                {
                    // Deserializar el JSON en una lista de Usuario
                    string jsonAutorizadores = await responseAutorizadores.Content.ReadAsStringAsync();
                    model.Autorizadores = JsonConvert.DeserializeObject<List<Usuario>>(jsonAutorizadores);
                }

                // Obtener Artículos Relacionados (se mostrará el nombre del artículo)
                HttpResponseMessage responseArticulos = await _httpClient.GetAsync("api/Articulos/GetArticuloRelacionado");
                if (responseArticulos.IsSuccessStatusCode)
                {
                    string jsonArticulos = await responseArticulos.Content.ReadAsStringAsync();
                    model.ArticulosRelacionadosList = JsonConvert.DeserializeObject<List<Articulos>>(jsonArticulos);
                }
            }
            catch (Exception ex)
            {
                // si ocurre un error al cargar los datos del formulario
                TempData["Mensaje"] = $"Error al cargar datos del formulario: {ex.Message}";
            }
            //  Mostrar la vista con el modelo
            return View(model);

        }


        //------------------------------------------------------------------------------------------------------------------------------
        // POST: Articulos/Create  modificado

        [HttpPost]
        public async Task<IActionResult> Create(ArticuloViewModel model, IFormFile? ImagenFile)
        {
            //  Log de inicio
            _logger.LogInformation("Iniciando creación de artículo.");

            if (!ModelState.IsValid)
            {  //  Log de modelo inválido
                _logger.LogWarning("Modelo inválido en Create: {Errors}",
                   ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return await Create();
            }
            //  Log de modelo válido
            if (ImagenFile != null)
            {  //  Log de carga de imagen
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagenFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //  Log de guardado de imagen
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagenFile.CopyToAsync(fileStream);
                }
                //  Log de URL de imagen
                model.UrlImagen = "/img/" + uniqueFileName;
            }
            //   de creación de DTO para ser enviado a la API
            var dto = new
            {  
                IdArticulo = model.IdArticulo,
                Nombre = model.Nombre,
                Descripcion = model.Descripcion,
                Contenido = model.Contenido,
                UrlImagen = model.UrlImagen,
                FechaPublicacion = model.FechaPublicacion,
                IdCategoria = model.IdCategoria,
                Estado = model.Estado,
                Observaciones = model.Observaciones,
                //  Log de selección de etiquetas, autores y autorizadores 

                IdEtiquetas = model.SelectedEtiquetas ?? new List<int>(),  //  Asegurar que no sea NULL
                IdArticulosRelacionados = model.SelectedArticulosRelacionados ?? new List<int>(), //  Asegurar que no sea NULL
                IdAutores = model.SelectedAutores ?? new List<int>(),
                IdAutorizadores = model.SelectedAutorizadores ?? new List<int>()

            };
            //  
            try
            {   // serializar el DTO a JSON
                string json = JsonConvert.SerializeObject(dto);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync("api/Articulos/CrearArticulo", content);
                _logger.LogInformation("Respuesta de la API: {StatusCode}", response.StatusCode);
                // si la respuesta es exitosa envía mensaje de éxito
                if (response.IsSuccessStatusCode)
                {  //  Log de éxito
                    TempData["Mensaje"] = "Artículo creado exitosamente.";
                    return RedirectToAction("Index");
                }
                else
                {
                    // leer el cuerpo de la respuesta para obtener el mensaje de error
                    string body = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("Error al crear artículo. StatusCode={StatusCode}, Body={Body}",
                        response.StatusCode, body);
                    TempData["Mensaje"] = "Error al crear el artículo: " + response.ReasonPhrase;
                }
            }
            catch (Exception ex)
            {  //  envia los resultados de la de excepción
                _logger.LogError(ex, "Excepción al crear artículo.");
                TempData["Mensaje"] = "Excepción al crear el artículo: " + ex.Message;
            }


            return await Create();
        }


        // METODO PARA GESTIONAR LOS ARTICULOS
        // metodo asincorno para obtener los articulos con sus relaciones
        public async Task<IActionResult> Gestion()
        {
            try
            {

                // Realiza solicitud HTTP  a la API para obtener los artículos con relaciones
                HttpResponseMessage response = await _httpClient.GetAsync("api/Articulos/GetArticuloRelaciones");
                if (response.IsSuccessStatusCode)
                {
                    // Lee la respuesta de la API en formato Json 
                    string json = await response.Content.ReadAsStringAsync();

                    // Convierte el JSON en una lista de objetos `ArticuloDetalleDTO`.
                    // Si la conversión falla o la API devuelve `null`, se asigna una lista vacía para evitar errores.
                    List<ArticuloDetalleDTO> articulos = JsonConvert.DeserializeObject<List<ArticuloDetalleDTO>>(json) ?? new List<ArticuloDetalleDTO>();
                    return View(articulos);
                }
                else
                {
                    // Si la respuesta de la API no es exitosa, se almacena un mensaje de error en `TempData`
                    // para mostrarlo en la vista.

                    TempData["Mensaje"] = "Error al obtener los artículos. Código: " + response.StatusCode;
                    return View(new List<ArticuloDetalleDTO>());
                }
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Excepción: {ex.Message}";
                // Se devuelve una vista con una lista vacía para evitar fallos en la renderización de la interfaz
                return View(new List<ArticuloDetalleDTO>());
            }
        }

        // Indica que este método responde a solicitudes HTTP POST
        [HttpPost]
        public async Task<IActionResult> EliminarArticulo(int id)
        {
            try
            {
                // Envía una solicitud HTTP DELETE a la API para eliminar el artículo con el ID proporcionado
                HttpResponseMessage response = await _httpClient.DeleteAsync($"api/Articulos/DeleteArticulo/{id}");
                // Verifica si la solicitud fue exitosa
                if (response.IsSuccessStatusCode)
                {
                    // Guarda un mensaje en TempData para notificar que el artículo fue eliminado correctamente
                    TempData["Mensaje"] = "Artículo eliminado correctamente.";
                }
                else
                {
                    // Si la API devuelve un error, se obtiene el mensaje detallado de la respuesta
                    string errorResponse = await response.Content.ReadAsStringAsync();
                    TempData["Mensaje"] = $"Error al eliminar el artículo: {errorResponse}";
                }
            }
            catch (Exception ex)
            {
                // Capturamos cualquier excepción que ocurra durante la ejecución y guarda el mensaje de error en TempData
                TempData["Mensaje"] = $"Excepción al eliminar el artículo: {ex.Message}";
            }

            return RedirectToAction("Gestion"); // Redirige a la vista de gestión de artículos
        }



        // METODOS ADICIONALES PARA OBTENER INFORMACION 

        // metodo para obtener las categorias desde la API
        private async Task<List<Categorias>> ObtenerCategoriasDesdeAPI()
        {
            // Realiza una solicitud HTTP GET a la API para obtener las categorías
            HttpResponseMessage response = await _httpClient.GetAsync("api/Articulos/GetCategorias");
            if (response.IsSuccessStatusCode)
            {
                // Lee la respuesta de la API en formato Json
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Categorias>>(json) ?? new List<Categorias>();
            } // Si la solicitud no es exitosa, se devuelve una lista vacía para evitar errores
            return new List<Categorias>();
        }

        // metodo para obtener las etiquetas desde la API
        private async Task<List<Etiquetas>> ObtenerEtiquetasDesdeAPI()
        {
            // Realiza una solicitud HTTP GET a la API para obtener las etiquetas
            HttpResponseMessage response = await _httpClient.GetAsync("api/Articulos/GetEtiquetas");
            if (response.IsSuccessStatusCode)
            { // Lee la respuesta de la API en formato Json
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Etiquetas>>(json) ?? new List<Etiquetas>();
            }// Si la solicitud no es exitosa, se devuelve una lista vacía para evitar errores
            return new List<Etiquetas>();
        }


        // metodo para obtener los autores desde la API
        private async Task<List<Usuario>> ObtenerUsuariosDesdeAPI()
        {
            // Realiza una solicitud HTTP GET a la API para obtener los autores
            HttpResponseMessage response = await _httpClient.GetAsync("api/Articulos/GetAutores");
            if (response.IsSuccessStatusCode)
            {
                // Lee la respuesta de la API en formato Json
                string json = await response.Content.ReadAsStringAsync();
                List<UsuarioDTO> usuariosDTO = JsonConvert.DeserializeObject<List<UsuarioDTO>>(json);

                // Convierte los objetos UsuarioDTO en objetos Usuario
                return usuariosDTO?.Select(u => new Usuario
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Email = u.Email
                }).ToList() ?? new List<Usuario>();
            }  // Si la solicitud no es exitosa, se devuelve una lista vacía para evitar errores
            return new List<Usuario>();
        }


        // metodo para obtener los autorizadores desde la API
        private async Task<List<Usuario>> ObtenerAutorizadoresDesdeAPI()
        {
            // Realiza una solicitud HTTP GET a la API para obtener los autorizadores
            HttpResponseMessage response = await _httpClient.GetAsync("api/Articulos/GetAutorizadores");
            if (response.IsSuccessStatusCode)
            {
                // Lee la respuesta de la API en formato Json
                string json = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<Usuario>>(json) ?? new List<Usuario>();
            }// Si la solicitud no es exitosa, se devuelve una lista vacía para evitar errores
            return new List<Usuario>();
        }



        // nuevo metodo para editar obtiene un articulo por su id



        // METODO EDIT SIRVE PARA TRAER LA INFORMACION DEL ID ESPECIFICO QUE SE VA A MODIFICAR :
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                // Llamar a la API para obtener los detalles del artículo con relaciones
                HttpResponseMessage response = await _httpClient.GetAsync($"api/Articulos/GetArticuloForId/{id}");
                // Verificar si la respuesta es exitosa
                if (!response.IsSuccessStatusCode)
                {
                    // Mostrar mensaje de error si no se pudo obtener la información
                    TempData["Mensaje"] = "Error al obtener el artículo.";
                    return RedirectToAction("Gestion");
                }

                // Leer el JSON de la respuesta
                string json = await response.Content.ReadAsStringAsync();
                ArticuloDTO articuloDTO = JsonConvert.DeserializeObject<ArticuloDTO>(json);
                // Verificar si se deserializó correctamente
                if (articuloDTO == null)
                {
                    // Mostrar mensaje si no se encontró el artículo
                    TempData["Mensaje"] = "No se encontró el artículo.";
                    return RedirectToAction("Gestion");
                }

                //  Obtener listas de selección desde la API
                var categorias = await ObtenerCategoriasDesdeAPI();
                var etiquetas = await ObtenerEtiquetasDesdeAPI();
                var usuariosDTO = await ObtenerUsuariosDesdeAPI();
                var autorizadoresDTO = await ObtenerAutorizadoresDesdeAPI();

                //  Convertir UsuarioDTO a Usuario
                var usuarios = usuariosDTO.Select(u => new Usuario
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Email = u.Email
                }).ToList();
                //  Convertir UsuarioDTO a Usuario
                var autorizadores = autorizadoresDTO.Select(u => new Usuario
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Email = u.Email
                }).ToList();

                //  Mapear los datos al ViewModel
                ArticuloViewModel model = new ArticuloViewModel
                {
                    IdArticulo = articuloDTO.IdArticulo,
                    Nombre = articuloDTO.Nombre,
                    Descripcion = articuloDTO.Descripcion,
                    Contenido = articuloDTO.Contenido,
                    UrlImagen = articuloDTO.UrlImagen,
                    FechaPublicacion = articuloDTO.FechaPublicacion,
                    IdCategoria = articuloDTO.IdCategoria,
                    Estado = articuloDTO.Estado,
                    Observaciones = articuloDTO.Observaciones ?? "", // ✅ Asegurar que no sea null

                    //  Asignar listas de selección corregidas
                    Categorias = categorias,
                    Etiquetas = etiquetas,
                    Usuarios = usuarios,
                    Autorizadores = autorizadores,

                    //  Asignar listas de IDs para mantener la selección
                    SelectedEtiquetas = articuloDTO.IdEtiquetas ?? new List<int>(),
                    SelectedAutores = articuloDTO.IdAutores ?? new List<int>(),
                    SelectedAutorizadores = articuloDTO.IdAutorizadores ?? new List<int>()
                };

                return View("Edit", model);
            }
            catch (Exception ex)
            {
                TempData["Mensaje"] = $"Error: {ex.Message}";
                return RedirectToAction("Gestion");
            }
        }


        // METODO EDIT PARA GUARDAR LOS CAMBIOS DE UN ARTICULO 
        //metodo para editar un articulo
        [HttpPost]
        public async Task<IActionResult> Edit(ArticuloViewModel model, IFormFile? ImagenFile)
        {
            if (!ModelState.IsValid)
            {
                TempData["Mensaje"] = "Datos inválidos.";
                model.Categorias = await ObtenerCategoriasDesdeAPI();
                model.Etiquetas = await ObtenerEtiquetasDesdeAPI();
                model.Usuarios = await ObtenerUsuariosDesdeAPI();
                model.Autorizadores = await ObtenerAutorizadoresDesdeAPI();
                return View(model);
            }
            // Guardar la imagen si se ha seleccionado una nueva
            if (ImagenFile != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "img");
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImagenFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await ImagenFile.CopyToAsync(fileStream);
                }
                model.UrlImagen = "/img/" + uniqueFileName;
            }
            // Crear DTO para enviar a la API
            var dto = new
            {//  Crear DTO para enviar a la API
                model.IdArticulo,
                model.Nombre,
                model.Descripcion,
                model.Contenido,
                model.UrlImagen,
                model.FechaPublicacion,
                model.IdCategoria,
                model.Estado,
                model.Observaciones,

                IdEtiquetas = model.SelectedEtiquetas ?? new List<int>(),
                IdArticulosRelacionados = model.SelectedArticulosRelacionados ?? new List<int>(),
                IdAutores = model.SelectedAutores ?? new List<int>(),
                IdAutorizadores = model.SelectedAutorizadores ?? new List<int>()
            };
            // Serializar el DTO a JSON
            string json = JsonConvert.SerializeObject(dto);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"api/Articulos/UpdateArticulo/{model.IdArticulo}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Mensaje"] = "Artículo actualizado correctamente.";
                return RedirectToAction("Gestion");
            }
            else
            {
                // Leer el cuerpo de la respuesta para obtener el mensaje de error
                TempData["Mensaje"] = "Error al actualizar el artículo.";
                return View("Edit", model);
            }
        }










    }
}
