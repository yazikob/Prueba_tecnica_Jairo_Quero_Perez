using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Http;
using Newtonsoft.Json;
using PruebaTecnicaJQP.Models;
using System.Drawing;

namespace PruebaTecnicaJQP
{
    public partial class _Default : Page
    {
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) // Si es la primera carga de la página
            {
                // Obtener la hora actual
                DateTime now = DateTime.Now;

                // Almacenar la hora actual en el ViewState
                ViewState["FirstLoadTime"] = now.ToString("HH:mm:ss");

                
            }

            // Configurar la etiqueta con la hora de la primera carga
            if (ViewState["FirstLoadTime"] != null)
            {
                lblFirstLoadTime.Text = $"El sistema corrió por primera vez a las {ViewState["FirstLoadTime"]}";
            }

            if (IsPostBack)
            {
                if (ViewState["ClientesData"] is List<Cliente> clientes)
                {
                    CargarTabla(clientes);
                }
            }

        }

        protected void btnCargaTabla_Click(object sender, EventArgs e)
        {
            try
            {
                HttpClient client = new HttpClient();
                string apiUrl = "https://pos.dermalia.mx/webforms/data";
                HttpResponseMessage response = client.GetAsync(apiUrl).Result;

                if (response.IsSuccessStatusCode)
                {
                    var data = response.Content.ReadAsStringAsync().Result;
                    List<Cliente> clientes = JsonConvert.DeserializeObject<List<Cliente>>(data);
                    ViewState["ClientesData"] = clientes;
                    CargarTabla(clientes);
                    
                    btnCargaTabla.Visible = false;


                }
                else
                {
                    lblError.Text = "Hubo un problema al obtener los datos. Por favor, inténtalo de nuevo más tarde.";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = $"Ocurrió un error: {ex.Message}";
            }
        }

        protected void DdlSexo_SelectedIndexChanged(object sender, EventArgs e)
        {

            DropDownList ddl = sender as DropDownList;

            if (ddl == null)
                return; // Si no se puede convertir el remitente a DropDownList, salimos del método.

            string selectedValue = ddl.SelectedValue;
            string clientId = ddl.ID.Replace("ddlSexo_", "");
            int id = Convert.ToInt32(clientId);

            List<Cliente> clientes = ViewState["ClientesData"] as List<Cliente>;

            if (clientes == null)
                return; // Si no hay clientes en el ViewState, salimos del método.

            Cliente clienteModificado = clientes.FirstOrDefault(c => c.idCliente == id);
            if (clienteModificado != null)
            {
                clienteModificado.sexo = selectedValue;  // Actualizamos el sexo en la lista de clientes en el ViewState.
            }

            // Actualizar el footer con la información del cliente modificado.
            ActualizarFooter(selectedValue, clienteModificado.nombres, clienteModificado.primerApellido, clienteModificado.segundoApellido);
        }
        private void ActualizarFooter(string sexo, string nombreCliente, string PriApellido, string SegApellido)
        {
            TableRow footerRow = tblClientes.Rows[tblClientes.Rows.Count - 1];

            // Si ya existen celdas en el footer, las borramos para añadir las nuevas
            footerRow.Cells.Clear();

            // Primera columna: "Registro Modificado"
            TableCell cell1 = new TableCell { Text = "Registro Modificado", CssClass = sexo == "M" ? "background-blue" : "background-pink" };
            footerRow.Cells.Add(cell1);

            // Segunda columna: Nombre del cliente con el sexo modificado]
            TableCell cell2 = new TableCell { Text = $"{nombreCliente} {PriApellido} {SegApellido} ({sexo})", CssClass = sexo == "M" ? "background-blue" : "background-pink" };
            footerRow.Cells.Add(cell2);

            // Tercera columna: Porcentaje de hombres y mujeres
            decimal porcentajeM = CalcularPorcentaje("M");
            decimal porcentajeF = CalcularPorcentaje("F");
            TableCell cell3 = new TableCell { Text = $"M: {porcentajeM:0.00}% F: {porcentajeF:0.00}%", CssClass = sexo == "M" ? "background-blue" : "background-pink" };
            footerRow.Cells.Add(cell3);

            // Cuarta columna vacía con el color de fondo según el sexo modificado
            TableCell cell4 = new TableCell { CssClass = sexo == "M" ? "background-blue" : "background-pink" };
            footerRow.Cells.Add(cell4);
        }

        private void CargarTabla(List<Cliente> clientes)
        {
            // Limpia la tabla antes de agregar filas
            tblClientes.Rows.Clear();

            // Crear el encabezado de la tabla
            TableRow headerRow = new TableRow();
            TableHeaderCell cell1 = new TableHeaderCell { Text = "Primer Apellido", CssClass = "custom-table-header" };
            TableHeaderCell cell2 = new TableHeaderCell { Text = "Segundo Apellido", CssClass = "custom-table-header" };
            TableHeaderCell cell3 = new TableHeaderCell { Text = "Nombre", CssClass = "custom-table-header" };
            TableHeaderCell cell4 = new TableHeaderCell { Text = "Sexo", CssClass = "custom-table-header" };

          

            headerRow.Cells.AddRange(new TableCell[] { cell1, cell2, cell3, cell4 });
            tblClientes.Rows.Add(headerRow);

            // Llenar la tabla con datos
            foreach (Cliente cliente in clientes)
            {
                TableRow row = new TableRow();
                row.CssClass = "header-row";

                // Primer Apellido
                TableCell primerApellidoCell = new TableCell { Text = cliente.primerApellido };
                row.Cells.Add(primerApellidoCell);

                // Segundo Apellido
                TableCell segundoApellidoCell = new TableCell { Text = cliente.segundoApellido };
                row.Cells.Add(segundoApellidoCell);

                // Nombre
                TableCell nombresCell = new TableCell { Text = cliente.nombres };
                row.Cells.Add(nombresCell);

                // Sexo con DropDownList
                TableCell sexoCell = new TableCell();
                DropDownList ddlSexo = new DropDownList();
                ddlSexo.ID = "ddlSexo_" + cliente.idCliente;  // Esto es útil para identificarlo posteriormente si es necesario
                ddlSexo.Items.Add(new ListItem("Masculino", "M"));
                ddlSexo.Items.Add(new ListItem("Femenino", "F"));
                ddlSexo.SelectedValue = cliente.sexo;
                ddlSexo.SelectedIndexChanged += DdlSexo_SelectedIndexChanged;  // Evento cuando se cambia la selección
                ddlSexo.AutoPostBack = true;  // Habilita el postback al cambiar la opción seleccionada
                ddlSexo.EnableViewState = true;
                sexoCell.Controls.Add(ddlSexo);
                row.Cells.Add(sexoCell);

                tblClientes.Rows.Add(row);

            }

            TableRow footerRow = new TableRow();

            // Añadir celdas al footer, en este caso 4 para que coincida con las columnas de la tabla
            for (int i = 0; i < 4; i++)
            {
                TableCell footerCell = new TableCell();
                footerCell.CssClass = "table-footer";
                footerCell.Text = "&nbsp;";
                footerRow.Cells.Add(footerCell);
            }

            tblClientes.Rows.Add(footerRow);
        }

        private decimal CalcularPorcentaje(string sexo)
        {
            List<Cliente> clientes = ViewState["ClientesData"] as List<Cliente>;

            if (clientes == null || !clientes.Any())
                return 0m;

            int totalClientes = clientes.Count;
            int totalSexo = clientes.Count(c => c.sexo == sexo);

            return ((decimal)totalSexo / totalClientes) * 100;
        }





    }
}