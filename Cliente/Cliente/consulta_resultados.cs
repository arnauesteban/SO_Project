using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cliente
{
    public partial class consulta_resultados : Form
    {
        Server server;
        public consulta_resultados(Server server)
        {
            InitializeComponent();
            this.server = server;
        }

        private void buscarBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "9/" + nombreIn.Text;
            server.Enviar(mensaje);
            this.Close();
        }

        private void cancelarBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
