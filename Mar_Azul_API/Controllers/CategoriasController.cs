using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Mar_Azul_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : Controller
    {
        private readonly DbContextEditorial _context;


        public CategoriasController(DbContextEditorial context)
        {
            _context = context;
        }

        /// Método auxiliar para normalizar un string:
        /// elimina acentos, espacios y convierte a minúsculas.
        /// Esto permite comparar nombres de forma uniforme.
        /// </summary>
        /// <param name="input">Texto a normalizar</param>
        /// <returns>Texto normalizado</returns>
        private static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Normalizar a FormD para descomponer caracteres con diacríticos
            string normalized = input.Normalize(NormalizationForm.FormD);

            // Usar StringBuilder para eliminar acentos
            var sb = new StringBuilder();
            foreach (char c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(char.ToLowerInvariant(c)); // Convertir a minúsculas directamente
            }

            // Convertir a string y eliminar espacios en blanco
            return Regex.Replace(sb.ToString(), @"\s+", "");
        }



        /// <summary>
        /// GET: api/Categoria/GetCategoria  
        /// Retorna la lista completa de Categoria.
        [HttpGet("GetCategorias")]
        public async Task<ActionResult<IEnumerable<Categorias>>> GetCategorias()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Articulo) // 🔹 Incluye los artículos
                .ToListAsync();

            return Ok(categorias);
        }

        [HttpGet("GetCategoriaForId/{idCategoria}")]
        public async Task<ActionResult<Categorias>> GetCategoriaForId(int idCategoria)
        {
            var categoria = await _context.Categorias
                .Include(c => c.Articulo) // 🔹 Incluye los artículos de la categoría
                .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria);

            if (categoria == null)
            {
                return NotFound(new { message = "Categoría no encontrada." });
            }

            return Ok(categoria);
        }




        /// <summary>
        /// GET: api/Categoria/GetCategoriaForName/{Nombre}  
        /// Retorna las Categoria que tengan el nombre especificado, 
        /// considerando la comparación sin distinguir mayúsculas, acentos y espacios.
        /// </summary>
        [HttpGet("GetCategoriaForName/{nombre}")]
        public async Task<ActionResult<IEnumerable<Categorias>>> GetCategoriaForName(string nombre)
        {
            // Normalizar el nombre de búsqueda
            string normalizedNombre = Normalize(nombre);

            var categoria = (await _context.Categorias
            .AsNoTracking() // Optimización: evita tracking en la consulta
            .ToListAsync()) // Se ejecuta la consulta en la base de datos
            .Where(e => Normalize(e.Nombre) == normalizedNombre) // Filtra en memoria
            .ToList(); // Convierte a lista final

            // En lugar de NotFound(), devuelve una lista vacía para evitar el error en el cliente.
            return Ok(categoria);

        }

        [HttpPost("AddCategoria")]
        public async Task<ActionResult<Categorias>> AddCategoria([FromBody] Categorias categoria)
        {
            categoria.Seccion = null;
            ModelState.Remove("Seccion");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Any())
                    .Select(x => new { Field = x.Key, Errors = x.Value.Errors.Select(e => e.ErrorMessage) })
                    .ToList();

                return BadRequest(new { message = "Errores de validación", errors });
            }

            categoria.IdCategoria = 0;
           

            string normalizedNewName = Normalize(categoria.Nombre);

            bool exists = (await _context.Categorias
                .Select(e => Normalize(e.Nombre))
                .ToListAsync())
                .Contains(normalizedNewName);

            if (exists)
            {
                return Conflict(new { message = "No se pueden repetir nombres de Categorias." });
            }

            _context.Categorias.Add(categoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategoriaForId), new { idCategoria = categoria.IdCategoria }, categoria);
        }



        /// <summary>
        /// PUT: api/Categoria/UpdateCategoria/{idCategoria}  
        /// Actualiza la información de una Categoria existente, validando que el nuevo nombre 
        /// no genere duplicados (ignorando mayúsculas, acentos y espacios).
        /// </summary>
        [HttpPut("UpdateCategoria/{idCategoria}")]
        public async Task<IActionResult> UpdateCategoria(int idCategoria, [FromBody] Categorias categoria)
        {
            if (idCategoria != categoria.IdCategoria)
            {
                return BadRequest(new { message = "El ID de la categoría no coincide." });
            }

            // Cargar la sección de la base de datos para mantener la relación
            var categoriaExistente = await _context.Categorias
                .Include(c => c.Seccion)
                .FirstOrDefaultAsync(c => c.IdCategoria == idCategoria);

            if (categoriaExistente == null)
            {
                return NotFound(new { message = "Categoría no encontrada." });
            }

           
            // Verificar que el nuevo nombre no cause duplicados
            string normalizedUpdatedName = Normalize(categoria.Nombre);
            bool duplicateExists = (await _context.Categorias
                .Where(c => c.IdCategoria != idCategoria)
                .Select(c => Normalize(c.Nombre))
                .ToListAsync())
                .Contains(normalizedUpdatedName);

            if (duplicateExists)
            {
                return Conflict(new { message = "No se pueden repetir nombres de categorías." });
            }

            // Actualizar valores
            categoriaExistente.Nombre = categoria.Nombre;
            categoriaExistente.Descripcion = categoria.Descripcion;
            categoriaExistente.Estado = categoria.Estado;
            categoriaExistente.IdSeccion = categoria.IdSeccion;
            categoriaExistente.UrlImagen = string.IsNullOrEmpty(categoria.UrlImagen)
                                            ? categoriaExistente.UrlImagen
                                            : categoria.UrlImagen;

            _context.Entry(categoriaExistente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                return Conflict(new { message = "Error al actualizar la categoría." });
            }

            return NoContent();
        }



        /// <summary>
        /// DELETE: api/Categoria/DeleteCategoria/{idCategoria}  
        /// Elimina la categoria con el ID especificado.
        /// </summary>
        [HttpDelete("DeleteCategoria/{idCategoria}")]
        public async Task<IActionResult> DeleteCategoria(int idCategoria)
        {
            var categoria = await _context.Categorias.FindAsync(idCategoria);
            if (categoria == null)
            {
                return NotFound(new { message = "Categoria no encontrada." });
            }

            _context.Categorias.Remove(categoria);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Categoria eliminada correctamente." });
        }

        /// <summary>
        /// Verifica si existe una etiqueta con el ID proporcionado.
        /// </summary>
        /// <param name="id">ID de la etiqueta</param>
        /// <returns>True si existe, false en caso contrario</returns>
        private bool CategoriaExists(int id)
        {
            return _context.Categorias.Any(c => c.IdCategoria == id);
        }


    }
}