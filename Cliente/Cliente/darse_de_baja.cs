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
    public partial class darse_de_baja : Form
    {
        Server server;

        public darse_de_baja(Server server)
        {
            InitializeComponent();
            this.server = server;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string mensaje = "8/" + claveIn.Text;
            server.Enviar(mensaje);
            this.Close();
        }
    }
}
