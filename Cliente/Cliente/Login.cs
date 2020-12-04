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
        bool conectado = false;

        public Login()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            
        }

        private void Login_Load(object sender, EventArgs e)
        {
            opcionBtn.Text = "Iniciar sesión";
            titleLb.Text = "Registro";
            clave2Lb.Visible = true;
            clave2In.Visible = true;
            enviarBtn.Text = "Registrarse";
            nombreIn.Text = null;
            claveIn.Text = null;
            clave2In.Text = null;

            if(conectado) DesconectarServidor();
        }

        private void ConectarServidor()
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor al que deseamos conectarnos
            
            //Parametros de shiva
            IPAddress direc = IPAddress.Parse("147.83.117.22");
            IPEndPoint ipep = new IPEndPoint(direc, 50084);

            //Parametros de pruebas
            //IPAddress direc = IPAddress.Parse("192.168.56.101");
            //IPEndPoint ipep = new IPEndPoint(direc, 9050);

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
            try
            {
                server.Send(msg);
            }
            catch (SocketException)
            {
                conectado = false;
            }
            catch (NullReferenceException)
            {
                conectado = false;
            }
        }

        private bool ComprobarCaracteres(string cadena, string campo)
        {
            int i = 0;
            bool error = false;
            if (cadena == null || cadena == "")
            {
                error = true;
                MessageBox.Show("Debes rellenar el campo " + campo);
            }
            else
                while (i < cadena.Length && !error)
                {
                    if (cadena[i] == '$' || cadena[i] == '/' || cadena[i] == '|' || cadena[i] == '&' || cadena[i] == '%')
                    {
                        error = true;
                        MessageBox.Show("No se puede usar ninguno de los siguientes carácteres: $ / | % & en el campo " + campo);
                    }
                    if (cadena[i] == 'á' || cadena[i] == 'Á' || cadena[i] == 'à' || cadena[i] == 'À' || cadena[i] == 'â' || cadena[i] == 'Â')
                    {
                        error = true;
                        MessageBox.Show("No se puede usar tildes en el campo " + campo);
                    }
                    if (cadena[i] == 'é' || cadena[i] == 'É' || cadena[i] == 'è' || cadena[i] == 'È' || cadena[i] == 'ê' || cadena[i] == 'Ê')
                    {
                        error = true;
                        MessageBox.Show("No se puede usar tildes en el campo " + campo);
                    }
                    if (cadena[i] == 'í' || cadena[i] == 'Í' || cadena[i] == 'ì' || cadena[i] == 'Ì' || cadena[i] == 'î' || cadena[i] == 'Î')
                    {
                        error = true;
                        MessageBox.Show("No se puede usar tildes en el campo " + campo);
                    }
                    if (cadena[i] == 'ó' || cadena[i] == 'Ó' || cadena[i] == 'ò' || cadena[i] == 'Ò' || cadena[i] == 'ô' || cadena[i] == 'Ô')
                    {
                        error = true;
                        MessageBox.Show("No se puede usar tildes en el campo " + campo);
                    }
                    if (cadena[i] == 'ú' || cadena[i] == 'Ú' || cadena[i] == 'ù' || cadena[i] == 'Ù' || cadena[i] == 'û' || cadena[i] == 'Û')
                    {
                        error = true;
                        MessageBox.Show("No se puede usar tildes en el campo " + campo);
                    }
                    if (cadena[i] == 'ç' || cadena[i] == 'ñ' || cadena[i] == 'Ç' || cadena[i] == 'Ñ')
                    {
                        error = true;
                        MessageBox.Show("No se puede usar las letras 'ç' ni 'ñ' en el campo " + campo);
                    }
                    i++;
                }
            return error;
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
                        }
                        break;
                }
            }
        }

        private void enviarBtn_Click(object sender, EventArgs e)
        {
            if (enviarBtn.Text == "Registrarse")
            {
                bool errorNombre = ComprobarCaracteres(nombreIn.Text, "'Nombre de usuario'");
                bool errorClave = ComprobarCaracteres(claveIn.Text, "'Contraseña'");

                if(claveIn.Text != clave2In.Text)
                    MessageBox.Show("Las contraseñas no coinciden. Inténtalo de nuevo.");
                else if(nombreIn.Text.Length > 20)
                    MessageBox.Show("El nombre de usuario es demasiado largo. Usa uno disinto (máximo 20 carácteres).");
                else if (claveIn.Text.Length > 20)
                    MessageBox.Show("La clave de acceso es demasiado larga. Usa otra disinta (máximo 20 carácteres).");
                else if (errorNombre || errorClave)
                {

                }
                else
                {
                    if (!conectado)
                    {
                        conectado = true;
                        ConectarServidor();
                    }
                    string sentencia = "1/" + nombreIn.Text + "/" + claveIn.Text;
                    EnviarServidor(sentencia);
                }
            }
            else
            {
                bool errorNombre = ComprobarCaracteres(nombreIn.Text, "'Nombre de usuario'");
                bool errorClave = ComprobarCaracteres(claveIn.Text, "'Contraseña'");

                if (nombreIn.Text.Length > 20)
                    MessageBox.Show("El nombre de usuario es demasiado largo. La longitud de este campo tiene que ser más corta (máximo 20 carácteres).");
                else if (claveIn.Text.Length > 20)
                    MessageBox.Show("La clave de acceso es demasiado larga. La longitud de este campo tiene que ser más corta (máximo 20 carácteres).");
                else if (errorNombre || errorClave)
                {

                }
                else
                {
                    if (!conectado)
                    {
                        conectado = true;
                        ConectarServidor();
                    }
                    string sentencia = "2/" + nombreIn.Text + "/" + claveIn.Text;
                    EnviarServidor(sentencia);
                }
            }
        }

        private void opcionBtn_Click(object sender, EventArgs e)
        {
            if (enviarBtn.Text == "Registrarse")
            {
                opcionBtn.Text = "Registrarse";
                titleLb.Text = "Iniciar Sesión";
                clave2Lb.Visible = false;
                clave2In.Visible = false;
                enviarBtn.Text = "Conectarse";
            }
            else
            {
                opcionBtn.Text = "Iniciar sesión";
                titleLb.Text = "Registro";
                clave2Lb.Visible = true;
                clave2In.Visible = true;
                enviarBtn.Text = "Registrarse";
                nombreIn.Text = null;
                claveIn.Text = null;
                clave2In.Text = null;
            }
        }
    }
}
