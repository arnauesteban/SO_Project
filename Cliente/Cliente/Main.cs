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
        string usuario;

        public Main(Socket socket, string usuario)
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.server = socket;
            this.usuario = usuario;
        }

        private void EnviarServidor(string sentencia)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(sentencia);
            server.Send(msg);
        }
        private void atenderServidor()
        {
            while (true)
            {
                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('$');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];
                string mensajeFinal;
                string[] separado;
                int i;
                int cont;

                switch (codigo)
                {
                    
                    case 4:
                        // Estructura del mensaje recibido: nombre1/nombre2/
                        separado = mensaje.Split('/');
                        i = 0;
                        mensajeFinal = "Los siguientes jugadores han jugado la partida más larga: ";
                        cont = 0;
                        while (separado[i] != null && separado[i] != "")
                        {
                            if (cont == 0)
                            {
                                mensajeFinal = mensajeFinal + "" + separado[i];
                                cont = 1;
                            }
                            else
                                mensajeFinal = mensajeFinal + ", " + separado[i];
                            i++;
                        }
                        MessageBox.Show(mensajeFinal);
                        break;
                    case 5:
                        MessageBox.Show("ID del jugador con más puntos: " + mensaje);
                        break;
                    case 6:
                        separado = mensaje.Split('/');

                        ConectadosGrid.ColumnCount = 1;
                        ConectadosGrid.RowCount = separado.Length;
                        ConectadosGrid.ColumnHeadersVisible = false;
                        ConectadosGrid.RowHeadersVisible = false;
                        ConectadosGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                        ConectadosGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                        //the first row of the grid is filled with this values to help the user to undertand thed data
                        for (int j = 0; j < separado.Length; j++)
                        {
                            ConectadosGrid[0, j].Value = separado[j];
                        }

                        //Sets the alignment of all columns to middle center
                        this.ConectadosGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        break;

                    case 7:
                        // Estructura del mensaje recibido: num/num/num...
                        separado = mensaje.Split('/');
                        i = 0;
                        mensajeFinal = "Records de los jugadores han perdido partidas contra Arnau: ";
                        cont = 0;
                        while (separado[i] != null && separado[i] != "")
                        {
                            if (cont == 0)
                            {
                                mensajeFinal = mensajeFinal + "" + separado[i];
                                cont = 1;
                            }
                            else
                                mensajeFinal = mensajeFinal + ", " + separado[i];
                            i++;
                        }
                        MessageBox.Show(mensajeFinal);
                        break;
                }
            }
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
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            atender.Abort();
            server.Shutdown(SocketShutdown.Both);
            server.Close();

            //Abrimos formulario login
            this.Hide();
            this.Close();
            Login login = new Login();
            login.ShowDialog();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            nombreJugadorLb.Text = this.usuario;

            //Iniciamos el thread de atencion al servidor
            ThreadStart ts = delegate { atenderServidor(); };
            atender = new Thread(ts);
            atender.Start();

            //Pedimos al servidor que nos envie la lista de conectados actualizada
            EnviarServidor("3/");

        }
    }
}
