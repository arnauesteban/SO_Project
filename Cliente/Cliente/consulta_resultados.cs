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
        int consulta;
        public consulta_resultados(Server server)
        {
            InitializeComponent();
            this.server = server;
            this.consulta = -1;
        }

        private void buscarBtn_Click(object sender, EventArgs e)
        {
            if (consulta == 1)
            {
                string mensaje = "9/" + nombreIn.Text;
                server.Enviar(mensaje);
            }
            else if (consulta == 2)
            {
                string mensaje = "10/";
                server.Enviar(mensaje);
            }
            this.Close();
        }

        private void cancelarBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void consulta1Btn_CheckedChanged(object sender, EventArgs e)
        {
            if (consulta1Btn.Checked)
            {
                consulta = 1;
                nombreIn.Enabled = true;
                buscarBtn.Enabled = true;
            }
        }

        private void consulta_resultados_Load(object sender, EventArgs e)
        {
            nombreIn.Enabled = false;
            buscarBtn.Enabled = false;
        }

        private void consulta2Btn_CheckedChanged(object sender, EventArgs e)
        {
            if (consulta2Btn.Checked)
            {
                consulta = 2;
                nombreIn.Enabled = false;
                buscarBtn.Enabled = true;
            }
        }
    }
}
