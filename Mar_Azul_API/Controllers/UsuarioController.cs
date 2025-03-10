using Mar_Azul_API.Models;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;


namespace Mar_Azul_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly DbContextEditorial _context;


        public UsuarioController(DbContextEditorial context)
        {
            _context = context;
        }


        [HttpGet]

        public ActionResult<IEnumerable<Usuario>> GetUsuario()
        {
            return _context.Usuarios.ToList();
        }


        //get
        [HttpGet("{id}")]


        public ActionResult<Usuario> GetUsuario(int id)
        {
            var usuario = _context.Usuarios.Find(id);

            if (usuario == null)
            {
                return NotFound();

            }
            return usuario;

        }

        // Realizamos la función de Añadir un Usuario

        [HttpPost("PostUsuario")]
        public ActionResult<Usuario> PostUsuario([FromBody] Usuario usuario)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
        }


        //mediante solicitudes HTTP actualiza un producto que existe en la bd
        [HttpPut("{id}")]
        public IActionResult PutUsuario(int id, [FromBody] Usuario usuario)
        {
            if (id != usuario.IdUsuario)
            {
                return BadRequest();
            }

            _context.Entry(usuario).State = EntityState.Modified; // Indica que la entidad fue modificada
            _context.SaveChanges();

            return NoContent();
        }


        // Eliminar un producto 

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
            if (usuario == null)
            {
                return NotFound(new { message = "El usuario no existe" });
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuario eliminado correctamente" });
        }






    }



}

