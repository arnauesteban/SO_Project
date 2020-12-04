using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Cliente
{
    public partial class Main : Form
    {
        Server server;
        Thread atender;
        string usuario;

        NuevaPartida nueva_partida_form;
        Thread ThreadNuevaPartida;

        public NuevaPartida GetFormNuevaPartida()
        {
            return this.nueva_partida_form;
        }
        public Main(Server server, string usuario)
        {
            InitializeComponent();
            this.server = server;
            this.usuario = usuario;
        }

        public void TomaRespuesta6(string mensaje)
        {
            string[] separado = mensaje.Split('/');

            ConectadosGrid.ColumnCount = 1;
            ConectadosGrid.RowCount = separado.Length;
            ConectadosGrid.ColumnHeadersVisible = false;
            ConectadosGrid.RowHeadersVisible = false;
            ConectadosGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            ConectadosGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            //Introducimos el nombre de los usuarios conectados en la DataGridView
            for (int j = 0; j < separado.Length; j++)
            {
                ConectadosGrid[0, j].Value = separado[j];
            }

            //Sets the alignment of all columns to middle center
            this.ConectadosGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public void TomaRespuesta8(string mensaje)
        {
            //Recepcion de invitacion
            string[] separado = mensaje.Split('/');
            InvitacionRecibida notificacion_form = new InvitacionRecibida(separado[0]);
            DialogResult respuesta = notificacion_form.ShowDialog();

            if (respuesta == (DialogResult)1)
            {
                //Se acepta la partida
                server.Enviar("9/" + separado[1] + "/1");
            }
            else
            {
                //Se rechaza la partida
                server.Enviar("9/" + separado[1] + "/0");
            }
        }

        private void desconectar_Btn_Click(object sender, EventArgs e)
        {
            //Quiere desconectarse del servidor
            string mensaje = "0/" + this.usuario;

            // Enviamos al servidor el código de petición
            // Estructura del mensaje a enviar: 0/
            server.Enviar(mensaje);

            server.Desconectar();

            if (ThreadNuevaPartida.IsAlive) ThreadNuevaPartida.Abort() ;

            //Abrimos formulario login
            this.Hide();
            this.Close();
            Login login = new Login();
            login.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nombreJugadorLb.Text = this.usuario;

            //Pedimos al servidor que nos envie la lista de conectados actualizada
            server.Enviar("3/");

        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            string mensaje = "0/" + this.usuario;

            // Enviamos al servidor el código de petición
            // Estructura del mensaje a enviar: 0/
            server.Enviar(mensaje);

            if(ThreadNuevaPartida.IsAlive)  ThreadNuevaPartida.Abort();

            if(server.IsConnected()) server.Desconectar();
        }

        private void NuevaPartidaBtn_Click(object sender, EventArgs e)
        {
            //Iniciamos el thread de atencion al servidor
            ThreadStart ts = delegate { AbrirFormularioNuevaPartida(); };
            ThreadNuevaPartida = new Thread(ts);
            ThreadNuevaPartida.Start();
            
        }

        private void AbrirFormularioNuevaPartida()
        {
            nueva_partida_form = new NuevaPartida(this.server);
            this.Close();
            nueva_partida_form.ShowDialog();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }
    }
}
