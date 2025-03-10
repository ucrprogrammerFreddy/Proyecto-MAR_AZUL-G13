using System.Globalization;
using System.Text.RegularExpressions;
using System.Text;
using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static System.Collections.Specialized.BitVector32;

namespace Mar_Azul_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SeccionesController : Controller
    {
        private readonly DbContextEditorial _context;


        public SeccionesController(DbContextEditorial context)
        {
            _context = context;
        }
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


        [HttpGet("GetSecciones")]
        public async Task<ActionResult<IEnumerable<Secciones>>> GetSecciones()
        {
            var secciones = await _context.Secciones
                .Include(s => s.Categoria) // 🔹 Asegura que cargue las categorías
                .ToListAsync();

            return Ok(secciones);
        }

        [HttpGet("GetSecciosForId/{idSeccion}")]
        public async Task<ActionResult<Secciones>> GetSecciosForId(int idSeccion)
        {
            var seccion = await _context.Secciones
                .Include(s => s.Categoria) // 🔹 Incluye las categorías de la sección
                .FirstOrDefaultAsync(s => s.IdSeccion == idSeccion);

            if (seccion == null)
            {
                return NotFound(new { message = "Sección no encontrada." });
            }

            return Ok(seccion);
        }





        [HttpGet("GetSeccionForName/{nombre}")]
        public async Task<ActionResult<IEnumerable<Secciones>>> GetSeccionForName(string nombre)
        {
            // Normalizar el nombre de búsqueda
            string normalizedNombre = Normalize(nombre);

            // Obtener los datos de la base de datos primero y luego filtrar en memoria
            var secciones = new List<Secciones>();
            secciones = (await _context.Secciones.AsNoTracking() // Optimización: evita tracking en la consulta
            .ToListAsync()) // Se ejecuta la consulta en la base de datos
            .Where(e => Normalize(e.Nombre) == normalizedNombre) // Filtra en memoria
            .ToList(); // Convierte a lista final

            // En lugar de `NotFound()`, devuelve una lista vacía para evitar el error en el cliente.
            return Ok(secciones);

        }
       
        [HttpPost("AddSeccion")]
        //FromBody
        //Sirve para que la api 
        public async Task<ActionResult<Secciones>> AddSeccion([FromBody] Secciones seccion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            seccion.IdSeccion = 0;

            // Normalizar el nombre de la nueva Seccion
            string normalizedNewName = Normalize(seccion.Nombre);

            // Obtener solo los nombres normalizados en memoria y verificar si existe duplicado
            bool exists = (await _context.Secciones
                .Select(e => Normalize(e.Nombre)) // Solo obtener nombres normalizados
                .ToListAsync()) // Ejecutar en memoria
                .Contains(normalizedNewName); // Comparar con el nuevo nombre

            if (exists)
            {
                return Conflict(new { message = "No se pueden repetir nombres de Seccions." });
            }

            _context.Secciones.Add(seccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSecciosForId), new { idSeccion = seccion.IdSeccion }, seccion);
        }
        /// <summary>
        /// PUT: api/Secciones/UpdateSeccion/{idSeccion}  
        /// Actualiza la información de una seccion existente, validando que el nuevo nombre 
        /// no genere duplicados (ignorando mayúsculas, acentos y espacios).
        /// </summary>
        [HttpPut("UpdateSeccion/{idSeccion}")]
        public async Task<IActionResult> UpdateSeccion(int idSeccion, [FromBody] Secciones seccion)
        {
            if (idSeccion != seccion.IdSeccion)
            {
                return BadRequest(new { message = "El ID de la Seccion no coincide." });
            }

            // Verificar que no exista otra Seccion (con distinto id) con el mismo nombre normalizado
            // Normalizar el nombre de la seccion que se va a actualizar
            string normalizedUpdatedName = Normalize(seccion.Nombre);

            // Obtener todas las secciones y hacer la verificación en memoria
            bool duplicateExists = (await _context.Secciones
                .Where(e => e.IdSeccion != idSeccion) // Filtrar antes de traer datos a memoria
                .Select(e => Normalize(e.Nombre)) // Obtener solo nombres normalizados
                .ToListAsync()) // Ejecutar en la base de datos antes de aplicar Normalize()
                .Contains(normalizedUpdatedName); // Comparar en memoria

            if (duplicateExists)
            {
                return Conflict(new { message = "No se pueden repetir nombres de secciones." });
            }


            _context.Entry(seccion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SeccionExists(idSeccion))
                {
                    return NotFound(new { message = "Sección no encontrada." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        /// <summary>
        /// DELETE: api/Secciones/DeleteSeccion/{idseccion}  
        /// Elimina la seccion con el ID especificado.
        /// </summary>
        [HttpDelete("DeleteSeccion/{idSeccion}")]
        public async Task<IActionResult> DeleteSeccion(int idSeccion)
        {
            var seccion = await _context.Secciones
                .Include(s => s.Categoria) // Cargar categorías relacionadas
                .FirstOrDefaultAsync(s => s.IdSeccion == idSeccion);

            if (seccion == null)
            {
                return NotFound(new { message = "Sección no encontrada." });
            }

            if (seccion.Categoria.Any()) 
            {
                return BadRequest(new { message = "No se puede eliminar la sección porque tiene categorías asociadas." });
            }

            _context.Secciones.Remove(seccion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sección eliminada correctamente." });
        }


        /// <summary>
        /// Verifica si existe una seccion con el ID proporcionado.
        /// </summary>
        /// <param name="id">ID de la SECCION</param>
        /// <returns>True si existe, false en caso contrario</returns>
        private bool SeccionExists(int id)
        {
            return _context.Secciones.Any(e => e.IdSeccion == id);
        }

    }
}
