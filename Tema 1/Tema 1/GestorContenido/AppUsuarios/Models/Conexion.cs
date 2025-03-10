namespace AppUsuarios.Models
{
    public class Conexion
    {
        public HttpClient Iniciar()
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri("http://marazulapi.somee.com/");

            return client;
        }
    }
}

