using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_1._2_Facturacion
{
    internal class Articulo
    {
        public int IdArt { get; set; }
        public string Nombre { get; set; }
        public double PrecioUnitario { get; set; }

        public bool ProductoActivo { get; set; }

        public Articulo()
        {
            IdArt = 0;
            Nombre = String.Empty;
            PrecioUnitario = 0;
            ProductoActivo = true;
        }

        public Articulo(int id_art, string nombre, double precioUnitario)
        {
            IdArt = id_art;
            Nombre = nombre;
            PrecioUnitario = precioUnitario;
            ProductoActivo = true;
        }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
