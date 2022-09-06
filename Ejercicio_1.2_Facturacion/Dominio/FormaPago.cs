using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ejercicio_1._2_Facturacion
{
    internal class FormaPago
    {
        public int IdFormaPago { get; set; }
        public string TipoFP { get; set; }

        public FormaPago()
        {
            this.IdFormaPago = 0;
            this.TipoFP = string.Empty;
        }
        public FormaPago(int idFp,string nombre)
        {
            IdFormaPago=idFp;
            TipoFP = nombre;
        }
    }
}
