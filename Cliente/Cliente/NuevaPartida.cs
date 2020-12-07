using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace Cliente
{
    public partial class NuevaPartida : Form
    {
        Server server;
        int ID;
        string usuario;

        public NuevaPartida(Server server, string usuario)
        {
            InitializeComponent();
            this.server = server;
            this.usuario = usuario;
            usuarioLbl.Text = this.usuario;
            chatLbl.Text = "¡Bienvenido a una nueva partida de poker! Espera a que tus contrincantes acepten tu invitación o pulsa el botón del centro de la mesa.";
        }

        public NuevaPartida(Server server, string usuario, int ID)
        {
            InitializeComponent();
            this.server = server;
            this.usuario = usuario;
            this.ID = ID;
            usuarioLbl.Text = this.usuario;
            IDLbl.Text = "ID: " + this.ID;
            chatLbl.Text = "¡Bienvenido a una nueva partida de poker! Espera a que otros contrincantes acepten la invitación o a que el host pulse el botón del centro de la mesa.";
        }

        public int getID()
        {
            return this.ID;
        }

        public void TomaRespuesta10(string mensaje)
        {
            chatLbl.Text = chatLbl.Text + Environment.NewLine + mensaje;
        }

        public void TomaRespuesta12(string mensaje)
        {
            this.ID = Convert.ToInt32(mensaje);
            IDLbl.Text = "ID: " + this.ID;
        }

        private void enviar_Btn_Click(object sender, EventArgs e)
        {
            string mensaje = "10/" + this.ID + "/" + this.usuario + ": " + chatTextBox.Text;
            server.Enviar(mensaje);
            chatTextBox.Text = "";
        }

        private void empezarBtn_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Juego no implementado.");
        }

        private void NuevaPartida_FormClosing(object sender, FormClosingEventArgs e)
        {
            string mensaje = "10/" + this.ID + "/" + this.usuario + " se ha ido de la partida.";
            server.Enviar(mensaje);
            mensaje = "11/" + this.ID;
            server.Enviar(mensaje);
        }

        private void chatTextBox_Click(object sender, EventArgs e)
        {
            chatTextBox.Text = "";
        }
    }
}
