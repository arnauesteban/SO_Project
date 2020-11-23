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
    public partial class Login : Form
    {
        Socket server;
        Thread atender;
        bool primeraVez = true;
        public Login()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            
        }

        private void Login_Load(object sender, EventArgs e)
        {
            
        }
        private void ConectarServidor()
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor 
            //al que deseamos conectarnos
            IPAddress direc = IPAddress.Parse("192.168.56.102");
            IPEndPoint ipep = new IPEndPoint(direc, 9080);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep); //Intentamos conectar el socket
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                MessageBox.Show("No he podido conectar con el servidor");
                return;
            }
            ThreadStart ts = delegate { atenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }

        private void DesconectarServidor()
        {
            EnviarServidor("0/");

            // Nos desconectamos
            atender.Abort();
            server.Shutdown(SocketShutdown.Both);
            server.Close();
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
                this.Text = "Login (conectado)";
                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                server.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('$');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];

                switch (codigo)
                {
                    case 1:
                        MessageBox.Show(mensaje);
                        break;
                    case 2:
                        MessageBox.Show(mensaje);
                        if (mensaje == "Se ha iniciado sesion correctamente.")
                        {
                            Main main = new Main(this.server, nombreIn.Text);
                            this.Close();
                            main.ShowDialog();
                            this.Text = "Login";
                            atender.Abort();
                        }
                        break;
                }
            }
        }


        private void registrarseBtn_Click(object sender, EventArgs e)
        {
            loginBtn.Enabled = true;
            registrarseBtn.Enabled = false;
            titleLb.Text = "Registro";
            clave2Lb.Visible = true;
            clave2In.Visible = true;
            enviarBtn.Text = "Registrarse";
            nombreIn.Text = null;
            claveIn.Text = null;
            clave2In.Text = null;
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            loginBtn.Enabled = false;
            registrarseBtn.Enabled = true;
            titleLb.Text = "Iniciar Sesión";
            clave2Lb.Visible = false;
            clave2In.Visible = false;
            enviarBtn.Text = "Conectarse";
        }

        private void enviarBtn_Click(object sender, EventArgs e)
        {
            if (enviarBtn.Text == "Registrarse" && primeraVez && claveIn.Text == clave2In.Text)
            {
                ConectarServidor();
                primeraVez = false;
            }
            else if(enviarBtn.Text == "Conectarse")
            {
                ConectarServidor();
                primeraVez = false;
            }

            //Inicio sesion
            if (enviarBtn.Text == "Conectarse")
            {
                string sentencia = "2/" + nombreIn.Text + "/" + claveIn.Text;
                EnviarServidor(sentencia);
            }

            //Registro
            else if (enviarBtn.Text == "Registrarse" && claveIn.Text == clave2In.Text)
            {
                string sentencia = "1/" + nombreIn.Text + "/" + claveIn.Text;
                EnviarServidor(sentencia);
            }
            else MessageBox.Show("Las contraseñas no coinciden. Inténtalo de nuevo.");
            
        }
    }
}
