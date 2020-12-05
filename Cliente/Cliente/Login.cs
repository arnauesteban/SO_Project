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

        Main main;
        Server server;
        Thread atender;

        bool mainAbierto;

        public Login()
        {
            InitializeComponent();
            
        }

        private void Login_Load(object sender, EventArgs e)
        {
            server = new Server();

            mainAbierto = false;

            opcionBtn.Text = "Iniciar sesión";
            titleLb.Text = "Registro";
            clave2Lb.Visible = true;
            clave2In.Visible = true;
            enviarBtn.Text = "Registrarse";
            nombreIn.Text = null;
            claveIn.Text = null;
            clave2In.Text = null;

            if (server.IsConnected())
            {
                server.Desconectar();

                // Nos desconectamos
                atender.Abort();
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

        public delegate void DelegadoLogin();

        public void CerrarFormulario()
        {
            this.Close();
        }

        private void atenderServidor()
        {
            while (server.IsConnected())
            {
                //Recibimos la respuesta del servidor
                string[] trozos = server.Recibir().Split('$');
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
                            main = new Main(server, nombreIn.Text);
                            mainAbierto = true;
                            DelegadoLogin delegado = new DelegadoLogin(CerrarFormulario);
                            this.Invoke(delegado);
                            main.ShowDialog();
                            
                        }
                        break;

                    case 3:
                        NuevaPartida nueva_partida_form = main.GetFormNuevaPartida();
                        nueva_partida_form.TomaRespuesta9(mensaje);
                        break;

                    case 6:
                        if(main != null)
                            main.TomaRespuesta6(mensaje);
                        break;

                    case 8:
                        main.TomaRespuesta8(mensaje);
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
                    if (!server.IsConnected())
                    {
                        if (server.Conectar() == 1)
                        {
                            ThreadStart ts = delegate { atenderServidor(); };
                            atender = new Thread(ts);
                            atender.Start();
                        }
                    }
                    if (server.IsConnected())
                    {
                        string sentencia = "1/" + nombreIn.Text + "/" + claveIn.Text;
                        server.Enviar(sentencia);
                    }
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
                    if (!server.IsConnected())
                    {
                        if (server.Conectar() == 1)
                        {
                            ThreadStart ts = delegate { atenderServidor(); };
                            atender = new Thread(ts);
                            atender.Start();
                        }
                        else
                            MessageBox.Show("No se ha podido conectar con el servidor");
                    }
                    if (server.IsConnected())
                    {
                        string sentencia = "2/" + nombreIn.Text + "/" + claveIn.Text;
                        server.Enviar(sentencia);
                    }
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

        private void Login_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (atender != null && !mainAbierto)
                if (atender.IsAlive)
                    atender.Abort();
        }
    }
}
