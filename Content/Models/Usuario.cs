using System;

namespace SistemaCotizaciones.Models
{
    public class Usuario
    {
        public int ID { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime UltimaActividad { get; set; }
        public bool Estado { get; set; }
    }
}