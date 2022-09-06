using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Ejercicio_1._2_Facturacion
{
    internal class HelperDB
    {
        private SqlConnection cnn;

        public HelperDB()
        {
            cnn = new SqlConnection(Properties.Resources.String1);
        }


        public DataTable ConsultaDB(string nombre_sp)
        { 
            DataTable tabla = new DataTable();            
            SqlCommand cmd = new SqlCommand(nombre_sp,cnn);
            cmd.CommandType = CommandType.StoredProcedure;

            cnn.Open();
            tabla.Load(cmd.ExecuteReader());
            cnn.Close();

            return tabla;        
        }

        internal int ProximaFactura(string nombre_sp)
        {
            
            SqlCommand cmd = new SqlCommand(nombre_sp, cnn);
            cmd.CommandType = CommandType.StoredProcedure;
            SqlParameter pOut = new SqlParameter();
            pOut.ParameterName = "@NUEVA_FACTURA";
            pOut.DbType = DbType.Int32;
            pOut.Direction = ParameterDirection.Output;
            cmd.Parameters.Add(pOut);

            cnn.Open();
            cmd.ExecuteNonQuery();
            cnn.Close();

            return (int)pOut.Value;
        }

        public bool RegistrarFactura(Factura nuevaFactura)
        {
            bool ok = true;
            SqlTransaction t = null;
            SqlCommand cmd = new SqlCommand();
            SqlCommand cmdDetalle = new SqlCommand();
            try
            {
                // INSERT MAESTRO //
                cnn.Open();
                t = cnn.BeginTransaction(); // Iniciar transacción
                cmd.Connection = cnn;
                cmd.Transaction = t;
                cmd.CommandText = "SP_INSERTAR_MAESTRO";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@CLIENTE", nuevaFactura.Cliente);
                cmd.Parameters.AddWithValue("@FORMA_PAGO", nuevaFactura.FormaPago.IdFormaPago);

                SqlParameter nroFacturaOut = new SqlParameter();
                nroFacturaOut.ParameterName = "@NRO_FACTURA";
                nroFacturaOut.DbType = DbType.Int32;
                nroFacturaOut.Direction = ParameterDirection.Output;
                cmd.Parameters.Add(nroFacturaOut);
                cmd.ExecuteNonQuery();
                int nroFactura = (int)nroFacturaOut.Value;
                // INSERT DETALLE //
                foreach (DetalleFactura fila in nuevaFactura.ListDetalles)
                {
                    cmdDetalle.Connection = cnn;
                    cmdDetalle.Transaction = t;
                    cmdDetalle.CommandText = "SP_INSERTAR_DETALLE";
                    cmdDetalle.CommandType = CommandType.StoredProcedure;                   
                    cmdDetalle.Parameters.AddWithValue("@NRO_FACTURA", nroFactura);
                    cmdDetalle.Parameters.AddWithValue("@ID_ARTICULO", fila.Articulo.IdArt);
                    cmdDetalle.Parameters.AddWithValue("@CANTIDAD", fila.Cantidad);

                    SqlParameter nroDetalle = new SqlParameter();
                    nroDetalle.ParameterName = "@DETALLE_NRO";
                    nroDetalle.DbType = DbType.Int32;
                    nroDetalle.Direction = ParameterDirection.Output;
                    cmdDetalle.Parameters.Add(nroDetalle);

                    cmdDetalle.ExecuteNonQuery();
                    
                }             
                t.Commit(); // Consolidar/confirmar
            }
            catch (Exception)
            {
                if (t != null)
                {
                    t.Rollback();
                    ok = false;
                }               
            }
            finally
            {
                if (cnn != null && cnn.State == ConnectionState.Open)
                {
                    
                    cnn.Close();
                }
                
            }

            return ok;
        }
    }
}
