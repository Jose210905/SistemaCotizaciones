using System;

namespace SistemaCotizaciones.Models
{
    public class Cotizacion
    {
        public int ID { get; set; }
        public int ClienteID { get; set; }
        public DateTime FechaCotizacion { get; set; }
        public int ProductoID { get; set; }
        public int CantidadProducto { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Bruto { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaVencimiento { get; set; }
    }
}