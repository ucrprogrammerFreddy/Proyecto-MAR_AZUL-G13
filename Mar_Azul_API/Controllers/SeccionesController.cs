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
    public class SeccionesController : Controller
    {
        private readonly DbContextEditorial _context;


        public SeccionesController(DbContextEditorial context)
        {
            _context = context;
        }

        /// <summary>
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
        //GET: api/secciones/GetSecciones  
        /// Retorna la lista completa de secciones.
        /// </summary>
        [HttpGet("GetSecciones")]
        public async Task<ActionResult<IEnumerable<Secciones>>> GetSecciones()
        {
            var secciones = await _context.Secciones.ToListAsync();
            return Ok(secciones);
        }

        /// <summary>
        /// GET: api/secciones/GetSeccionForId/{idSeccion}  
        /// Retorna la sección cuyo ID coincide con el proporcionado.
        /// </summary>
        [HttpGet("GetSeccionForId/{idSeccion}")]
        public async Task<ActionResult<Secciones>> GetSeccionForId(int idSeccion)
        {
            var seccion = await _context.Secciones.FindAsync(idSeccion);
            if (seccion == null)
            {
                return NotFound(new { message = "Sección no encontrada." });
            }
            return Ok(seccion);
        }

        /// <summary>
        /// GET: api/secciones/GetSeccionForName/{nombre}  
        /// Retorna las secciones que tengan el nombre especificado, 
        /// considerando la comparación sin distinguir mayúsculas, acentos y espacios.
        /// </summary>
        [HttpGet("GetSeccionForName/{nombre}")]
        public async Task<ActionResult<IEnumerable<Secciones>>> GetSeccionForName(string nombre)
        {
            // Normalizar el nombre de búsqueda
            string normalizedNombre = Normalize(nombre);

            // Obtener los datos de la base de datos primero y luego filtrar en memoria
            var secciones = (await _context.Secciones
                .AsNoTracking() // Optimización: evita tracking en la consulta
                .ToListAsync()) // Se ejecuta la consulta en la base de datos
                .Where(s => Normalize(s.Nombre) == normalizedNombre) // Filtra en memoria
                .ToList(); // Convierte a lista final

            return Ok(secciones);
        }

        /// <summary>
        /// POST: api/secciones/AddSeccion  
        /// Crea una nueva sección en la base de datos, validando que no exista 
        /// otra sección con el mismo nombre (ignorando mayúsculas, acentos y espacios).
        /// </summary>
        [HttpPost("AddSeccion")]
        public async Task<ActionResult<Secciones>> AddSeccion([FromBody] Secciones seccion)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            seccion.IdSeccion = 0;

            // Normalizar el nombre de la nueva sección
            string normalizedNewName = Normalize(seccion.Nombre);

            // Verificar si ya existe una sección con el mismo nombre normalizado
            bool exists = (await _context.Secciones
                .Select(s => Normalize(s.Nombre)) // Solo obtener nombres normalizados
                .ToListAsync()) // Ejecutar en memoria
                .Contains(normalizedNewName); // Comparar con el nuevo nombre

            if (exists)
            {
                return Conflict(new { message = "No se pueden repetir nombres de Secciones." });
            }

            _context.Secciones.Add(seccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSeccionForId), new { idSeccion = seccion.IdSeccion }, seccion);
        }

        /// <summary>
        /// PUT: api/secciones/UpdateSeccion/{idSeccion}  
        /// Actualiza la información de una sección existente, validando que el nuevo nombre 
        /// no genere duplicados (ignorando mayúsculas, acentos y espacios).
        /// </summary>
        [HttpPut("UpdateSeccion/{idSeccion}")]
        public async Task<IActionResult> UpdateSeccion(int idSeccion, [FromBody] Secciones seccion)
        {
            if (idSeccion != seccion.IdSeccion)
            {
                return BadRequest(new { message = "El ID de la sección no coincide." });
            }

            // Normalizar el nombre de la sección que se va a actualizar
            string normalizedUpdatedName = Normalize(seccion.Nombre);

            // Verificar que no exista otra sección con el mismo nombre normalizado
            bool duplicateExists = (await _context.Secciones
                .Where(s => s.IdSeccion != idSeccion) // Excluir la actual
                .Select(s => Normalize(s.Nombre)) // Obtener solo nombres normalizados
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
        /// DELETE: api/secciones/DeleteSeccion/{idSeccion}  
        /// Elimina la sección con el ID especificado.
        /// </summary>
        [HttpDelete("DeleteSeccion/{idSeccion}")]
        public async Task<IActionResult> DeleteSeccion(int idSeccion)
        {
            var seccion = await _context.Secciones.FindAsync(idSeccion);
            if (seccion == null)
            {
                return NotFound(new { message = "Sección no encontrada." });
            }

            _context.Secciones.Remove(seccion);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Sección eliminada correctamente." });
        }

        /// <summary>
        /// Verifica si existe una sección con el ID proporcionado.
        /// </summary>
        /// <param name="id">ID de la sección</param>
        /// <returns>True si existe, false en caso contrario</returns>
        private bool SeccionExists(int id)
        {
            return _context.Secciones.Any(s => s.IdSeccion == id);
        }



    }


}

