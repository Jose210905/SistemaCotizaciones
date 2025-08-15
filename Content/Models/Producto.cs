using System;

namespace SistemaCotizaciones.Models
{
    public class Producto
    {
        public int ID { get; set; }
        public string CodigoCabys { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CantidadDisponible { get; set; }
        public decimal Precio { get; set; }
        public bool Estado { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}