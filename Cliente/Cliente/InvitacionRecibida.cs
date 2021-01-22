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
    public partial class InvitacionRecibida : Form
    {
        string nombre_host;
        public InvitacionRecibida(string nombre_host)
        {
            InitializeComponent();
            this.nombre_host = nombre_host;
        }

        private void InvitacionRecibida_Load(object sender, EventArgs e)
        {
            Label.Text = "¡" + this.nombre_host + " te ha invitado a una partida de BlackJack! ¿Aceptar?";
        }

        private void YesBtn_Click(object sender, EventArgs e)
        {
            DialogResult = (DialogResult)1;
            this.Close();
        }

        private void NoBtn_Click(object sender, EventArgs e)
        {
            DialogResult = (DialogResult)0;
            this.Close();
        }
    }
}
