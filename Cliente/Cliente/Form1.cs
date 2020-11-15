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

        //Evento que se da cuando el usuario pulsa el botón "Enviar"
        private void enviar_Btn_Click(object sender, EventArgs e)
        {
            if (registrar.Checked)
            {
                // Quiere registrarse en la base de datos
                string mensaje = "1/" + usuario.Text + "/" + clave.Text;
                // Enviamos al servidor el nombre y la contraseña tecleados y el codigo de peticion
                // Estructura del mensaje a enviar: 1/nombre/contraseña
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
                // Quiere iniciar sesión
                string mensaje = "2/" + usuario.Text + "/" + clave.Text;
                // Enviamos al servidor el nombre y la contraseña tecleados y el codigo de peticion
                // Estructura del mensaje a enviar: 2/nombre/contraseña
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show(mensaje);

                if (mensaje == "Se ha iniciado sesion correctamente.")
                {
                    puntosPerdedores.Enabled = true;
                    dameRecord.Enabled = true;
                    nombresPartidaLarga.Enabled = true;
                    nombresConectados.Enabled = true;
                    registrar.Enabled = false;
                    iniciar_sesion.Enabled = false;
                }
            }
            else if (puntosPerdedores.Checked)
            {
                // Quiere obtener las puntuaciones record de los jugadores que han perdido partidas contra Arnau
                string mensaje = "3/";
                // Enviamos al servidor el código de petición
                // Estructura del mensaje a enviar: 3/
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                // Estructura del mensaje recibido: num/num/num...
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
                // Quiere saber el nombre de los jugadores que han jugado la partida más larga
                string mensaje = "4/";
                // Enviamos al servidor el código de petición
                // Estructura del mensaje a enviar: 4/
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                // Estructura del mensaje recibido: nombre1/nombre2/
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
                // Quiere saber el identificador del usuario que ostenta el mayor récord de la base de datos
                string mensaje = "5/";
                // Enviamos al servidor el código de petición
                // Estructura del mensaje a enviar: 5/
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];
                MessageBox.Show("ID del jugador con más puntos: " + mensaje);
            }
            else if (nombresConectados.Checked)
            {
                // Quiere saber los nombres de los usuarios conectados
                string mensaje = "6/";
                // Enviamos al servidor el código de petición
                // Estructura del mensaje a enviar: 6/
                byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
                server.Send(msg);

                //Recibimos la respuesta del servidor
                // Estructura del mensaje recibido: nombre1/nombre2/nombre3...
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                mensaje = Encoding.ASCII.GetString(msg2).Split('\0')[0];

                //Recogemos los nombres de los usuarios conectados
                string[] nombres = mensaje.Split('/');

                string mensajeFinal = "Usuarios conectados: ";
                for (int i = 0; i<nombres.Length - 1; i++)
                {
                    mensajeFinal += nombres[i] += ", ";
                }
                mensajeFinal += nombres[nombres.Length - 1];
                MessageBox.Show(mensajeFinal);

            }
        }

        private void conectar_Btn_Click(object sender, EventArgs e)
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, 9020);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep); //Intentamos conectar el socket
                this.BackColor = Color.Green;
                MessageBox.Show("Conectado correctamente");
                desconectar_Btn.Enabled = true;
                conectar_Btn.Enabled = false;
                registrar.Enabled = true;
                iniciar_sesion.Enabled = true;
                enviar_Btn.Enabled = true;
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
            //Quiere desconectarse del servidor
            string mensaje = "0/" + usuario.Text;

            // Enviamos al servidor el código de petición
            // Estructura del mensaje a enviar: 0/
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(mensaje);
            server.Send(msg);

            // Nos desconectamos
            this.BackColor = Color.Gray;
            server.Shutdown(SocketShutdown.Both);
            server.Close();
            puntosPerdedores.Enabled = false;
            dameRecord.Enabled = false;
            nombresPartidaLarga.Enabled = false;
            nombresConectados.Enabled = false;
            desconectar_Btn.Enabled = false;
            conectar_Btn.Enabled = true;
            registrar.Enabled = false;
            iniciar_sesion.Enabled = false;
            enviar_Btn.Enabled = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Al arrancar el cliente, hay que configurar los objetos del formulario para que el usuario no cometa acciones indebidas que bloqueen el servidor
            puntosPerdedores.Enabled = false;
            dameRecord.Enabled = false;
            nombresPartidaLarga.Enabled = false;
            nombresConectados.Enabled = false;
            desconectar_Btn.Enabled = false;
            conectar_Btn.Enabled = true;
            registrar.Enabled = false;
            iniciar_sesion.Enabled = false;
            enviar_Btn.Enabled = false;
        }
    }
}
