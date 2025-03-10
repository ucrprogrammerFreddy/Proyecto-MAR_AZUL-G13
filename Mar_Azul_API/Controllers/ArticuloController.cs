//controlador articulo API


using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using Mar_Azul_API.DTO;

namespace Mar_Azul_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArticulosController : Controller
    {
        private readonly DbContextEditorial _context;
        private readonly ILogger<ArticulosController> _logger;

        public ArticulosController(DbContextEditorial context, ILogger<ArticulosController> logger)
        {
            _context = context;
            _logger = logger;
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Método auxiliar para normalizar un string (eliminar acentos, espacios y pasar a minúsculas).
        /// </summary>
        private static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;
            string normalized = input.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(char.ToLowerInvariant(c));
            }
            return Regex.Replace(sb.ToString(), @"\s+", "");
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Retorna la lista de categorías.
        /// </summary>
        [HttpGet("GetCategorias")]
        public async Task<ActionResult<IEnumerable<Categorias>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Articulo)
                .ToListAsync();
            return Ok(categorias);
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Retorna la lista de etiquetas.
        /// </summary>
        [HttpGet("GetEtiquetas")]
        public async Task<ActionResult<IEnumerable<Etiqueta>>> GetEtiquetas()
        {
            var etiquetas = await _context.Etiquetas.ToListAsync();
            return Ok(etiquetas);
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Retorna la lista de usuarios que son autores.
        /// Se asume que existe una propiedad (por ejemplo, EsAutor) en la tabla Usuario.
        /// </summary>
        [HttpGet("GetAutores")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAutores()
        {
            // Devuelve todos los usuarios para luego filtrar en la vista por email
            var autores = await _context.Usuarios.ToListAsync();
            return Ok(autores);
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Retorna la lista de usuarios autorizadores.
        /// Se asume que existe una propiedad (por ejemplo, EsAutorizador) en la tabla Usuario.
        /// </summary>
        [HttpGet("GetAutorizadores")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetAutorizadores()
        {
            // Devuelve todos los usuarios para luego filtrar en la vista por rol
            var autorizadores = await _context.Usuarios.ToListAsync();
            return Ok(autorizadores);
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Retorna la lista de artículos (para seleccionar artículos relacionados).
        /// </summary>
        [HttpGet("GetArticuloRelacionado")]
        public async Task<ActionResult<IEnumerable<Articulos>>> GetArticuloRelacionado()
        {
            var articulos = await _context.Articulos.ToListAsync();
            return Ok(articulos);
        }
        //------------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Obtiene un artículo completo (incluyendo relaciones) a partir de su ID.
        /// </summary>
        [HttpGet("GetArticuloForId/{idArticulo}")]
        public async Task<ActionResult<ArticuloDTO>> GetArticuloForId(int idArticulo)
        {
            var articulo = await _context.Articulos
                .Include(a => a.Categoria) // 🔹 Obtener la categoría asociada
                .Include(a => a.ArticuloEtiquetas).ThenInclude(ae => ae.Etiqueta) // 🔹 Obtener etiquetas asociadas
                .Include(a => a.ArticuloAutores).ThenInclude(aa => aa.Usuario) // 🔹 Obtener autores asociados
                .Include(a => a.ArticuloAutorizadores).ThenInclude(aa => aa.Usuario) // 🔹 Obtener autorizadores asociados
                .FirstOrDefaultAsync(a => a.IdArticulo == idArticulo);

            if (articulo == null)
                return NotFound(new { message = "Artículo no encontrado." });

            // 🔹 Mapeo a ArticuloDTO
            var articuloDTO = new ArticuloDTO
            {
                IdArticulo = articulo.IdArticulo,
                Nombre = articulo.Nombre,
                Descripcion = articulo.Descripcion,
                Contenido = articulo.Contenido,
                UrlImagen = articulo.UrlImagen,
                FechaPublicacion = articulo.FechaPublicacion,
                IdCategoria = articulo.IdCategoria,
                Estado = articulo.Estado,
                Observaciones = articulo.ArticuloAutorizadores.FirstOrDefault()?.Observaciones ?? "", // ✅ Observaciones

                // 🔹 Convertir relaciones en listas de IDs
                IdEtiquetas = articulo.ArticuloEtiquetas.Select(ae => ae.Etiqueta.IdEtiqueta).ToList(),
                IdAutores = articulo.ArticuloAutores.Select(aa => aa.Usuario.IdUsuario).ToList(),
                IdAutorizadores = articulo.ArticuloAutorizadores.Select(aa => aa.Usuario.IdUsuario).ToList()
            };

            return Ok(articuloDTO);
        }

        //-----------------------------------------------------------------------------------------------------------    
        /// <summary>
        /// Crea un artículo y distribuye la información en las tablas intermedias correspondientes.
        /// </summary>

        [HttpPost("CrearArticulo")]
        public async Task<IActionResult> CrearArticulo([FromBody] ArticuloDTO dto)
        {
            // Paso 1: Validamos que el modelo recibido sea válido.
            if (!ModelState.IsValid)
            {
                /*
                var errors = ModelState
                    .Where(x => x.Value.Errors.Any())
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();
                return BadRequest(new { mensaje = "Errores de validación", errors });
                */


                var errors = ModelState
           .Where(x => x.Value.Errors.Any())
           .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
           .ToList();

                _logger.LogWarning("Modelo inválido al crear artículo. Errores: {Errors}", errors);
                return BadRequest(new { mensaje = "Errores de validación", errors });



            }

            try
            {
                // Paso 2: Creamos el objeto principal "Articulos" con los datos básicos.
                var articulo = new Articulos
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Contenido = dto.Contenido,
                    UrlImagen = dto.UrlImagen,
                    FechaPublicacion = dto.FechaPublicacion,
                    IdCategoria = dto.IdCategoria,
                    Estado = dto.Estado
                };

                // Agregamos el artículo al contexto y guardamos para generar el Id.
                _context.Articulos.Add(articulo);
                await _context.SaveChangesAsync(); // Aquí se asigna el IdArticulo
                _logger.LogInformation("Artículo creado exitosamente con Id={IdArticulo}", articulo.IdArticulo);
                // Paso 3: Insertamos las relaciones en las tablas intermedias

                // Relación Artículo - Etiqueta





                if (dto.IdEtiquetas != null && dto.IdEtiquetas.Any())
                {
                    var etiquetas = dto.IdEtiquetas.Select(id => new ArticuloEtiqueta
                    {
                        IdArticulo = articulo.IdArticulo,
                        IdEtiqueta = id
                    }).ToList();

                    _context.ArticuloEtiquetas.AddRange(etiquetas);
                }


                // Relación Artículo - Relacionado
                if (dto.IdArticulosRelacionados != null)
                {
                    foreach (var idArticuloRelacionado in dto.IdArticulosRelacionados)
                    {
                        var articuloRelacion = new ArticuloRelacion
                        {
                            IdArticuloPrincipal = articulo.IdArticulo,
                            IdArticuloRelacionado = idArticuloRelacionado
                        };
                        _context.ArticuloRelacionados.Add(articuloRelacion);
                    }
                }

                // Relación Artículo - Autorizador
                if (dto.IdAutorizadores != null)
                {
                    foreach (var idUsuario in dto.IdAutorizadores)
                    {
                        var autorizador = new ArticuloAutorizador
                        {
                            IdArticulo = articulo.IdArticulo,
                            IdUsuario = idUsuario,
                            FechaAutorizacion = DateTime.Now,
                            Observaciones = dto.Observaciones // Se usa Observaciones para todos los autorizadores
                        };
                        _context.ArticuloAutorizadores.Add(autorizador);
                    }
                }

                // Relación Artículo - Autor
                if (dto.IdAutores != null)
                {
                    foreach (var idUsuario in dto.IdAutores)
                    {
                        var autor = new ArticuloAutor
                        {
                            IdArticulo = articulo.IdArticulo,
                            IdUsuario = idUsuario
                        };
                        _context.ArticuloAutores.Add(autor);
                    }
                }

                // Paso 4: Guardamos todas las relaciones en la base de datos.
                await _context.SaveChangesAsync();

                // Paso 5: Retornamos un mensaje de éxito con el Id del artículo creado.
                return Ok(new { mensaje = "Artículo creado exitosamente.", articuloId = articulo.IdArticulo });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error interno al crear el artículo.");

                // Si ocurre alguna excepción, se retorna un error 500 con el mensaje de error.
                return StatusCode(500, new { mensaje = "Error interno al crear el artículo.", error = ex.Message });
            }
        }

        [HttpPut("UpdateArticulo/{idArticulo}")]
        public async Task<IActionResult> UpdateArticulo(int idArticulo, [FromBody] ArticuloDTO dto)
        {
            if (idArticulo != dto.IdArticulo)
                return BadRequest(new { message = "El ID del artículo no coincide." });

            var articuloExistente = await _context.Articulos
                .Include(a => a.ArticuloEtiquetas)
                .Include(a => a.ArticulosRelacionados)
                .Include(a => a.ArticuloAutorizadores)
                .Include(a => a.ArticuloAutores)
                .FirstOrDefaultAsync(a => a.IdArticulo == idArticulo);

            if (articuloExistente == null)
                return NotFound(new { message = "Artículo no encontrado." });

            try
            {
                // **🔹 Paso 1: Actualizar los datos principales del artículo**
                articuloExistente.Nombre = dto.Nombre;
                articuloExistente.Descripcion = dto.Descripcion;
                articuloExistente.Contenido = dto.Contenido;
                articuloExistente.UrlImagen = dto.UrlImagen;
                articuloExistente.FechaPublicacion = dto.FechaPublicacion;
                articuloExistente.IdCategoria = dto.IdCategoria;
                articuloExistente.Estado = dto.Estado;

                // **🔹 Paso 2: Eliminar relaciones anteriores y agregar las nuevas**

                // **✅ Eliminar y actualizar Etiquetas**
                _context.ArticuloEtiquetas.RemoveRange(articuloExistente.ArticuloEtiquetas);
                if (dto.IdEtiquetas != null && dto.IdEtiquetas.Any())
                {
                    var etiquetas = dto.IdEtiquetas.Select(id => new ArticuloEtiqueta
                    {
                        IdArticulo = articuloExistente.IdArticulo,
                        IdEtiqueta = id
                    }).ToList();
                    _context.ArticuloEtiquetas.AddRange(etiquetas);
                }

                // **✅ Eliminar y actualizar Autores**
                _context.ArticuloAutores.RemoveRange(articuloExistente.ArticuloAutores);
                if (dto.IdAutores != null && dto.IdAutores.Any())
                {
                    var autores = dto.IdAutores.Select(id => new ArticuloAutor
                    {
                        IdArticulo = articuloExistente.IdArticulo,
                        IdUsuario = id
                    }).ToList();
                    _context.ArticuloAutores.AddRange(autores);
                }

                // **✅ Eliminar y actualizar Artículos Relacionados**
                _context.ArticuloRelacionados.RemoveRange(articuloExistente.ArticulosRelacionados);
                if (dto.IdArticulosRelacionados != null && dto.IdArticulosRelacionados.Any())
                {
                    var relacionados = dto.IdArticulosRelacionados.Select(id => new ArticuloRelacion
                    {
                        IdArticuloPrincipal = articuloExistente.IdArticulo,
                        IdArticuloRelacionado = id
                    }).ToList();
                    _context.ArticuloRelacionados.AddRange(relacionados);
                }

                // **✅ Eliminar y actualizar Autorizadores con Observaciones**
                _context.ArticuloAutorizadores.RemoveRange(articuloExistente.ArticuloAutorizadores);
                if (dto.IdAutorizadores != null && dto.IdAutorizadores.Any())
                {
                    var autorizadores = dto.IdAutorizadores.Select(id => new ArticuloAutorizador
                    {
                        IdArticulo = articuloExistente.IdArticulo,
                        IdUsuario = id,
                        FechaAutorizacion = DateTime.Now,
                        Observaciones = dto.Observaciones // ✅ Se actualiza correctamente la observación
                    }).ToList();
                    _context.ArticuloAutorizadores.AddRange(autorizadores);
                }

                // **🔹 Paso 3: Guardar los cambios en la base de datos**
                await _context.SaveChangesAsync();

                return Ok(new { message = "Artículo actualizado correctamente." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error interno al actualizar el artículo.");
                return StatusCode(500, new { mensaje = "Error interno al actualizar el artículo.", error = ex.Message });
            }
        }








        /// <summary>
        /// Elimina un artículo.
        /// </summary>
        /// 

        // Modificado 


        [HttpDelete("DeleteArticulo/{idArticulo}")]
        public async Task<IActionResult> DeleteArticulo(int idArticulo)
        {
            try
            {
                var articulo = await _context.Articulos.FindAsync(idArticulo);
                if (articulo == null)
                {
                    return NotFound();
                }

                // 🔹 1. Eliminar primero las relaciones en la tabla ArticuloRelacionados
                var relaciones = _context.ArticuloRelacionados
                    .Where(ar => ar.IdArticuloPrincipal == idArticulo || ar.IdArticuloRelacionado == idArticulo);

                _context.ArticuloRelacionados.RemoveRange(relaciones);
                await _context.SaveChangesAsync(); // 🔹 Guardar para aplicar los cambios

                // 🔹 2. Ahora sí eliminar el artículo
                _context.Articulos.Remove(articulo);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error al eliminar el artículo: {ex.Message}");
            }
        }





        [HttpGet("GetArticuloRelaciones")]
        public async Task<ActionResult<IEnumerable<ArticuloDetalleDTO>>> GetArticuloRelaciones()
        {
            var articulos = await _context.Articulos
                .Include(a => a.Categoria) // Obtener la categoría asociada
                .Include(a => a.ArticuloEtiquetas).ThenInclude(ae => ae.Etiqueta) // Obtener etiquetas asociadas
                .Include(a => a.ArticuloAutores).ThenInclude(aa => aa.Usuario) // Obtener autor del artículo
                .Select(a => new ArticuloDetalleDTO
                {
                    IdArticulo = a.IdArticulo,
                    Nombre = a.Nombre,
                    Descripcion = a.Descripcion,
                    UrlImagen = a.UrlImagen,
                    FechaPublicacion = a.FechaPublicacion,
                    Estado = a.Estado,


                    // 🔹 Se agregan los datos faltantes
                    Categoria = a.Categoria != null ? a.Categoria.Nombre : "Sin categoría",
                    Autor = a.ArticuloAutores.Select(aa => aa.Usuario.Nombre).FirstOrDefault() ?? "Desconocido",
                    Etiquetas = a.ArticuloEtiquetas.Select(ae => new EtiquetaDTO
                    {
                        IdEtiqueta = ae.Etiqueta.IdEtiqueta,
                        Nombre = ae.Etiqueta.Nombre
                    }).ToList()
                })
                .ToListAsync();

            return Ok(articulos);
        }

        // ELIMINAR ARTICULO POR ID

    }
}
