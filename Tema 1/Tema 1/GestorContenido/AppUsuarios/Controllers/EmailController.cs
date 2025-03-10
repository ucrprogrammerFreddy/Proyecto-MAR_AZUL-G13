using AppUsuarios.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AppUsuarios.Controllers
{
    public class EmailController : Controller
    {
        private readonly IEmailService emailService;

        public EmailController(IEmailService emailService)
        {
            this.emailService = emailService;
        }
        // Método que responde a solicitudes HTTP POST.
        [HttpPost]
        public async Task<IActionResult> EnviarCorreo(string email, string asunto, string mensaje)
        {  // Verifica si el correo electrónico está vacío.
            if (string.IsNullOrEmpty(email))
            {  // Devuelve un mensaje de error si el correo electrónico está vacío.
                return Json(new { success = false, message = "El correo electrónico no puede estar vacío." });
            }

            try
            {   // Dirección de correo electrónico del receptor.
                string receptor = "tuemail@dominio.com";  // Tu correo receptor

                // Aquí verificamos que el email esté bien formateado antes de enviarlo
                if (!IsValidEmail(email))
                {
                    return Json(new { success = false, message = "El correo electrónico no tiene un formato válido." });
                }
                // Envía el correo electrónico.
                await emailService.EnviarCorreo(receptor, asunto, $"Mensaje de: {email}\n\n{mensaje}");
                // Devuelve un mensaje de éxito si el correo electrónico se envió correctamente.
                return Json(new { success = true, message = "Mensaje enviado con éxito." });
            }// Devuelve un mensaje de error si se produce una excepción.
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Hubo un error: {ex.Message}" });
            }
        }
        // Método para verificar si un correo electrónico tiene un formato válido.
        private bool IsValidEmail(string email)
        {
            try
            {  // Intenta crear una instancia de la clase MailAddress con la dirección de correo electrónico.
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {  // Devuelve falso si se produce una excepción al crear la instancia.
                return false;
            }
        }
    }
}
