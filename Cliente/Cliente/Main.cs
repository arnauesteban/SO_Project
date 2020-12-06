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

        string usuario;
        string lista_seleccionados = null;
        int num_invitados;

        List<NuevaPartida> lista_forms_partidas = new List<NuevaPartida>();
        Thread ThreadNuevaPartida;
        int cont_forms = 0;

        public delegate void DelegadoMain();

        /*public NuevaPartida GetFormNuevaPartida()
        {
            return this.nueva_partida_form;
        }*/

        public Main(Server server, string usuario)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.server = server;
            this.usuario = usuario;
        }

        public void TomaRespuesta3(string mensaje)
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
                ThreadStart ts = delegate { AbrirFormularioPartidaCreada(Convert.ToInt32(separado[1])); };
                ThreadNuevaPartida = new Thread(ts);
                ThreadNuevaPartida.Start();
            }
            else
            {
                //Se rechaza la partida
                server.Enviar("9/" + separado[1] + "/0");
            }
        }

        public void TomaRespuesta10(string mensaje)
        {
            string[] separado = mensaje.Split('/');
            for (int j = 0; j < cont_forms; j++)
                if (lista_forms_partidas[j].getID() == Convert.ToInt32(separado[0]))
                {
                    //lista_forms_partidas[cont_forms - 1].TomaRespuesta10(separado[1]);
                }
        }

        public void TomaRespuesta12(string mensaje)
        {
            //lista_forms_partidas[cont_forms - 1].TomaRespuesta12(mensaje);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nombreJugadorLb.Text = this.usuario;

            //Pedimos al servidor que nos envie la lista de conectados actualizada
            server.Enviar("3/");

        }

        private void ConectadosGrid_SelectionChanged(object sender, EventArgs e)
        {
            num_invitados = ConectadosGrid.SelectedCells.Count;
            for (int i = 0; i < num_invitados; i++)
            {
                lista_seleccionados = lista_seleccionados + "/" + ConectadosGrid.SelectedCells[i].Value;
            }
        }

        private void NuevaPartidaBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "8/" + num_invitados + lista_seleccionados;
            //Iniciamos el thread de atencion al servidor
            ThreadStart ts = delegate { AbrirFormularioNuevaPartida(mensaje); };
            ThreadNuevaPartida = new Thread(ts);
            ThreadNuevaPartida.Start();
        }

        private void AbrirFormularioPartidaCreada(int ID)
        {
            NuevaPartida form = new NuevaPartida(this.server, this.usuario, ID);
            lista_forms_partidas.Add(form);
            cont_forms++;
            form.ShowDialog();
        }

        private void AbrirFormularioNuevaPartida(string mensaje)
        {
            NuevaPartida form = new NuevaPartida(this.server, this.usuario, mensaje);
            lista_forms_partidas.Add(form);
            cont_forms++;
            form.ShowDialog();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ThreadNuevaPartida != null)
                if (ThreadNuevaPartida.IsAlive)
                {
                    MessageBox.Show("Abortamos nueva partida");
                    ThreadNuevaPartida.Abort();
                }

            if (server.IsConnected())
                server.Desconectar();
        }

        private void desconectar_Btn_Click(object sender, EventArgs e)
        {

            server.Desconectar();

            if (ThreadNuevaPartida != null)
                if (ThreadNuevaPartida.IsAlive)
                {
                    MessageBox.Show("Aborto partida");
                    ThreadNuevaPartida.Abort();
                }

            //Abrimos formulario login
            this.Hide();
            this.Close();
            Login login = new Login();
            login.ShowDialog();
        }
    }
}
