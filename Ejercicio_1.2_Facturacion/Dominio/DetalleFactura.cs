using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_1._2_Facturacion
{
    internal class DetalleFactura
    {
        public Articulo Articulo { get; set; }
        public int Cantidad { get; set; }

        public DetalleFactura()
        {
            this.Articulo = new Articulo();
            this.Cantidad = 0;
        }
       public DetalleFactura(Articulo articulo, int cantidad)
        {
            Articulo = articulo;
            Cantidad = cantidad;
        }

        public double CalcularSubTotal()
        {   
            return Articulo.PrecioUnitario*Cantidad;
        }
    }
}
