using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;


namespace Ejercicio_1._2_Facturacion
{
    public partial class FrmFactura : Form
    {
        private HelperDB oHelper;
        private Factura nuevaFactura;
       
        
        public FrmFactura()
        {
            InitializeComponent();
            oHelper = new HelperDB();
            nuevaFactura = new Factura();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CargarCombos();          
            ProximaFactura();
            VolverACero(true);
            this.reportViewer1.RefreshReport();
        }

        private void ProximaFactura()
        {
            int nuevaFactura;
            nuevaFactura = oHelper.ProximaFactura("SP_NUEVA_FACTURA");
            if (nuevaFactura > 0)
            {
                lblNroFactura.Text = "N° Factura: " + nuevaFactura.ToString();
            }
        }

        private void CargarCombos()
        {
            DataTable tabla;

            tabla = oHelper.ConsultaDB("SP_PRODUCTO");

            cboArticulo.DataSource = tabla;
            cboArticulo.DisplayMember = "DESCRIPCION";
            cboArticulo.ValueMember = "ID_ARTICULO";
            cboArticulo.DropDownStyle = ComboBoxStyle.DropDownList;

            DataTable tabla2;

            tabla2 = oHelper.ConsultaDB("SP_FORMAS_PAGO");

            cboFormaPago.DataSource = tabla2;
            cboFormaPago.DisplayMember = "FORMAPAGO";
            cboFormaPago.ValueMember = "ID_FORMAPAGO";
            cboFormaPago.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

            if (dgvDetalle.CurrentCell.ColumnIndex == 4)
            {
                nuevaFactura.QuitarDetalle(dgvDetalle.CurrentRow.Index);
                dgvDetalle.Rows.Remove(dgvDetalle.CurrentRow);
                nuevaFactura.CalcularTotal();
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            if (ValidarDetalle())
            {
                //VALIDAR QUE SE INGRESE UN ARTICULO NUEVO Y NO UNO YA EXISTENTE
                foreach (DataGridViewRow fila in dgvDetalle.Rows)
                {
                    if (fila.Cells["colArticulo"].Value.ToString().Equals(cboArticulo.Text))
                    {
                        MessageBox.Show($"Articulo {cboArticulo.Text} ya agregado", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return;
                    }

                }


                DataRowView item = (DataRowView)cboArticulo.SelectedItem;
                int id_articulo = Convert.ToInt32(item.Row.ItemArray[0]);
                string nombre_articulo = item.Row.ItemArray[1].ToString();
                double precio_articulo = Convert.ToDouble(item.Row.ItemArray[2]);

                Articulo oArticulo = new Articulo(id_articulo,nombre_articulo, precio_articulo);
                
                int cantidad = Convert.ToInt32(txtCantidad.Text);
                DetalleFactura detalle = new DetalleFactura(oArticulo, cantidad);

                nuevaFactura.AgregarDetalle(detalle);
                dgvDetalle.Rows.Add(new object[] { item.Row.ItemArray[0], item.Row.ItemArray[1], item.Row.ItemArray[2],txtCantidad.Text});
                


                txtSubtotal.Text = detalle.CalcularSubTotal().ToString();
                txtTotal.Text = nuevaFactura.CalcularTotal().ToString();
            }         
        }

        private bool ValidarDetalle()
        {
            bool valido = true;

            if (cboArticulo.SelectedIndex ==-1)
            {
                MessageBox.Show("Debe seleccionar un artículo", "Error",MessageBoxButtons.OK);
                valido = false;
            }
            if (txtCantidad.Text == String.Empty)
            {
                MessageBox.Show("Debe ingresar una cantidad", "Error", MessageBoxButtons.OK);
                valido = false;
            }


            return valido;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Seguro que desea cancelar la factura N°"+oHelper.ProximaFactura("SP_NUEVA_FACTURA")+"?","CANCELAR",MessageBoxButtons.YesNo,MessageBoxIcon.Question)==DialogResult.Yes)
            {
                VolverACero(true);
                
                
            }
        }

        private void VolverACero(bool v)
        {
            dtpFecha.Enabled = false;
            dtpFecha.Value = DateTime.Today;
            cboArticulo.SelectedIndex = -1;
            cboFormaPago.SelectedIndex = -1;
            txtCliente.Text = "";
            txtCantidad.Text = "";
            txtSubtotal.Text = "";
            txtTotal.Text = "";
           
        }

        private void btnAceptar_Click(object sender, EventArgs e)
        {
            if (ValidarFactura())
            {
                nuevaFactura.Cliente = txtCliente.Text;
                nuevaFactura.Fecha = dtpFecha.Value;
                nuevaFactura.FormaPago.IdFormaPago =Convert.ToInt32( cboFormaPago.SelectedValue);

                if (oHelper.RegistrarFactura(nuevaFactura))
                {
                    MessageBox.Show("Factura registrada","Exito",MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    if(MessageBox.Show("Desea realizar otra carga de factura?", "SIGUIENTE", MessageBoxButtons.YesNo, MessageBoxIcon.Question)==DialogResult.No)
                    {
                        Close();
                    }
                    else
                    {
                        VolverACero(true);
                    }
                }
                else
                    MessageBox.Show("Error 404", "¡¡Hay Caray!!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

            }
        }

        private bool ValidarFactura()
        {
            bool valido = true;
            if (txtCliente.Text == String.Empty)
            {
                MessageBox.Show("Debe ingresar el cliente", "Error", MessageBoxButtons.OK);
                valido = false;
            }

            if (dtpFecha.Value.Day != DateTime.Now.Day)
            {
                MessageBox.Show("No es posible seleccionar una fecha anterior a hoy", "Error", MessageBoxButtons.OK);
                valido = false;
            }
            if (cboFormaPago.SelectedIndex==-1)
            {
                MessageBox.Show("Debe seleccionar forma de pago", "Error", MessageBoxButtons.OK);
                valido = false;
            }

            if (dgvDetalle.Rows.Count==0)
            {
                MessageBox.Show("Debe ingresar al menos un detalle", "Error", MessageBoxButtons.OK);
                valido = false;
            }



            return valido;
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Desea salir de la aplicacion?", "SALIR", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                Close();
        }
    }
}
