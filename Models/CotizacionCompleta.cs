using System;

namespace SistemaCotizaciones.Models
{
    public class CotizacionCompleta
    {
        public int ID { get; set; }
        public int ClienteID { get; set; }
        public string NombreCliente { get; set; }
        public string IdentificacionCliente { get; set; }
        public DateTime FechaCotizacion { get; set; }
        public int ProductoID { get; set; }
        public string NombreProducto { get; set; }
        public string CodigoCabys { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Bruto { get; set; }
        public decimal Descuento { get; set; }
        public decimal IVA { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaVencimiento { get; set; }

        // Propiedades adicionales para reportes
        public string TipoCliente { get; set; }
        public string DescripcionProducto { get; set; }
        public int StockDisponible { get; set; }

        // Propiedades calculadas
        public bool EstaVencida => DateTime.Now.Subtract(FechaCotizacion).Days > 30;
        public bool EstaPorVencer => DateTime.Now.Subtract(FechaCotizacion).Days > 23 && DateTime.Now.Subtract(FechaCotizacion).Days <= 30;
        public string EstadoVisual => EstaVencida ? "VENCIDA" : EstaPorVencer ? "POR VENCER" : "VIGENTE";
    }
}