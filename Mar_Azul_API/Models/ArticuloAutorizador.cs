

using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Mar_Azul_API.Models
{
    public class ArticuloAutorizador
    {



        public int IdArticulo { get; set; }
        [ForeignKey("IdArticulo")]
        public Articulos Articulo { get; set; }

        public int IdUsuario { get; set; }
        [ForeignKey("IdUsuario")]
        public Usuario Usuario { get; set; }

        public DateTime FechaAutorizacion { get; set; } // Fecha en la que el usuario autoriza el artículo
        public string Observaciones { get; set; } // Comentarios del autorizador
    }
}








