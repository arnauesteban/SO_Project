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

namespace Cliente
{
    public partial class Form1 : Form
    {
        Socket server;
        public Form1()
        {
            InitializeComponent();
        }

        private void enviar_Btn_Click(object sender, EventArgs e)
        {
            if (registrar.Checked)
            {
                // Quiere saber la longitud
                string mensaje = "1/" + usuario.Text + "/" + clave.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show(mensaje);
            }
            else if (iniciar_sesion.Checked)
            {
                // Quiere saber si el nombre es bonito
                string mensaje = "2/" + usuario.Text + "/" + clave.Text;
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show(mensaje);
            }
            else if (puntosPerdedores.Checked)
            {
                // Quiere saber la longitud
                string mensaje = "3/";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                string[] separado = mensaje.Split('/');
                int i = 0;
                string mensajeFinal = "Records de los jugadores han perdido partidas contra Arnau: ";
                int cont = 0;
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
            }
            else if (nombresPartidaLarga.Checked)
            {
                // Quiere saber la longitud
                string mensaje = "4/";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                string[] separado = mensaje.Split('/');
                int i = 0;
                string mensajeFinal = "Los siguientes jugadores han jugado la partida más larga: ";
                int cont = 0;
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
            }
            else if (dameRecord.Checked)
            {
                // Quiere saber la longitud
                string mensaje = "5/";
                // Enviamos al servidor el nombre tecleado
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show("ID del jugador con más puntos: " + mensaje);
            }
        }

        private void conectar_Btn_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9020);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep);//Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado correctamente");
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
        }

        private void desconectar_Btn_Click(object sender, EventArgs e)
        {
            //Mensaje de desconexión
            string mensaje = "0/";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
        }
    }
}
