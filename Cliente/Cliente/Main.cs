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
        Socket server;
        Thread atender;
        bool conectado = true;
        string usuario;

        NuevaPartida nueva_partida_form;
        Thread ThreadNuevaPartida;

        public NuevaPartida GetFormNuevaPartida()
        {
            return this.nueva_partida_form;
        }
        public Main(Socket socket, string usuario)
        {
            InitializeComponent();
            this.server = socket;
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
                EnviarServidor("9/" + separado[1] + "/1");
            }
            else
            {
                //Se rechaza la partida
                EnviarServidor("9/" + separado[1] + "/0");
            }
        }

        private void EnviarServidor(string sentencia)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(sentencia);
            server.Send(msg);
        }


        //Evento que se da cuando el usuario pulsa el botón "Enviar"
        private void enviar_Btn_Click(object sender, EventArgs e)
        {
            if (puntosPerdedores.Checked)
            {
                // Quiere obtener las puntuaciones record de los jugadores que han perdido partidas contra Arnau
                string mensaje = "7/";
                // Enviamos al servidor el código de petición
                // Estructura del mensaje a enviar: 7/
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else if (nombresPartidaLarga.Checked)
            {
                // Quiere saber el nombre de los jugadores que han jugado la partida más larga
                string mensaje = "4/";
                // Enviamos al servidor el código de petición
                // Estructura del mensaje a enviar: 4/
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
            else if (dameRecord.Checked)
            {
                // Quiere saber el identificador del usuario que ostenta el mayor récord de la base de datos
                string mensaje = "5/";
                // Enviamos al servidor el código de petición
                // Estructura del mensaje a enviar: 5/
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);
            }
        }
        private void desconectar_Btn_Click(object sender, EventArgs e)
        {
            //Quiere desconectarse del servidor
            string mensaje = "0/" + this.usuario;

            // Enviamos al servidor el código de petición
            // Estructura del mensaje a enviar: 0/
            EnviarServidor(mensaje);

            DesconectarServidor();

            if (ThreadNuevaPartida.IsAlive) ThreadNuevaPartida.Abort() ;

            conectado = false;

            //Abrimos formulario login
            this.Hide();
            this.Close();
            Login login = new Login();
            login.ShowDialog();
        }

        private void DesconectarServidor()
        {
            // Nos desconectamos
            atender.Abort();
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            nombreJugadorLb.Text = this.usuario;

            //Pedimos al servidor que nos envie la lista de conectados actualizada
            EnviarServidor("3/");

        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            string mensaje = "0/" + this.usuario;

            // Enviamos al servidor el código de petición
            // Estructura del mensaje a enviar: 0/
            EnviarServidor(mensaje);

            if(ThreadNuevaPartida.IsAlive)  ThreadNuevaPartida.Abort();

            if(conectado) DesconectarServidor();
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
    }
}
