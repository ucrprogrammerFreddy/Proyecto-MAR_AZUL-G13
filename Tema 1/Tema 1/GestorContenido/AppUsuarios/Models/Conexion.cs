namespace AppUsuarios.Models
{
    public class Conexion
    {
        public HttpClient Iniciar()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("https://localhost:5001");

            return client;
        }
    }
}

