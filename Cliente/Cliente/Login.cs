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
        Thread thread_main;

        bool mainAbierto;

        public delegate void DelegadoLogin();

        public Login()
        {
            //Constructor del formulario. Inicializa los objetos que contiene.
            InitializeComponent();
            
        }

        private void Login_Load(object sender, EventArgs e)
        {
            //Evento de carga del formulario. Crea un objeto Server y configura parámetros de los objetos del formulario.
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
                //Si estábamos conectados al servidor, nos desconectamos
                MessageBox.Show("Abortamos");
                atender.Abort();
            }

            //Configuramos los textBox de clave para que cuando se escriba en ellos se vea únicamente el carácter '*'.
            claveIn.PasswordChar = '*';
            clave2In.PasswordChar = '*';
        }

        private bool ComprobarCaracteres(string cadena, string campo)
        {
            //Antes de enviar datos introducidos manualmente por el usuario, llamamos a esta función. Se encarga de verificar que 
            //no haya ningún carácter dañino para la ejecución del código del servidor en los campos rellenados.
            //Si se encuentra algún error, devuelve true.
            int i = 0;
            bool error = false;
            //Error en caso de dejar el campo en blanco.
            if (cadena == null || cadena == "")
            {
                error = true;
                MessageBox.Show("Debes rellenar el campo " + campo);
            }
            else
                //Este bucle buscará en cada carácter de la cadena de entrada letras con tildes, ñ, ç 
                //y los carácteres '$', '/', '|', '&' y '%'.
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

        public void CerrarFormulario()
        {
            this.Close();
        }

        public void AbrirMain()
        {
            //Esta función es llamada cuando se inicia sesión correctamente para cerrar este formulario y abrir el menú principal.
            main = new Main(server, nombreIn.Text);
            mainAbierto = true;
            DelegadoLogin delegado = new DelegadoLogin(CerrarFormulario);
            this.Invoke(delegado, new object[] {});
            main.ShowDialog();
        }

        private void atenderServidor()
        {
            //Función que ejecuta continuamente el thread de atención al servidor.
            while (server.IsConnected())
            {
                //Recibimos la respuesta del servidor
                string[] trozos = server.Recibir().Split('$');
                if (trozos.Length > 1)
                {
                    int codigo = Convert.ToInt32(trozos[0]);
                    string mensaje = trozos[1].Split('\0')[0];

                    switch (codigo)
                    {
                        case 1:
                            //Respuesta a petición de registro en la base de datos
                            MessageBox.Show(mensaje);
                            break;
                        case 2:
                            //Respuesta a petición de iniciar sesión
                            MessageBox.Show(mensaje);
                            if (mensaje == "Se ha iniciado sesion correctamente.")
                            {
                                ThreadStart ts = delegate { this.AbrirMain(); };
                                thread_main = new Thread(ts);
                                thread_main.Start();
                            }
                            break;
                        case 3:
                            //Recepción y reenvío de la lista de conectados actualizada.
                            if (main != null)
                                main.TomaRespuesta(codigo, mensaje);
                            break;

                        case 4:
                            //Recepción y reenvío de un mensaje con el identificador que tiene la partida que el usuario acaba de crear
                        case 5:
                            //Recepción y reenvío de una invitación para jugar una partida
                        case 6:
                            //Recepción y reenvío de un mensaje de chat de alguna partida que está jugando el usuario
                        case 7:
                            //Recepción y reenvío de un mensaje que designa a este cliente como host de una partida
                        case 8:
                            //Recepción y reenvío de un mensaje de respuesta a una peticion de baja del juego
                        case 9:
                            //Recepción y reenvío de un mensaje con la respuesta a la petición 1 a la base de datos
                        case 10:
                            //Recepción y reenvío de un mensaje con la respuesta a la petición 2 a la base de datos
                        case 11:
                            //Recepción y reenvío de un mensaje con la respuesta a la petición 3 a la base de datos
                        case 12:
                            //Recepción y reenvío de un mensaje con información de la partida (jugadores, fichas...)
                        case 13:
                            //Recepción y reenvío de un mensaje con cartas que puede ver el jugador
                        case 14:
                            //Recepción y reenvío de un mensaje con la acción que algun jugador ha realizado en la partida
                            main.TomaRespuesta(codigo, mensaje);
                            break;
                    }
                }
            }
        }

        private void enviarBtn_Click(object sender, EventArgs e)
        {
            //Evento que es llamado cada vez que el usuario pulsa el botón inferior del formulario.
            //En caso de que los datos introducidos no tengan errores de formato, envía la petición necesaria al servidor.
            if (enviarBtn.Text == "Registrarse")
            {
                //Petición de registro
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
                //Petición de inicio de sesión.
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
            //Evento llamado cada vez que el usuario quiere cambiar de opción en el formulario. Cambia los objetos del formulario
            //para registrarse o iniciar sesión.
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
            //Evento llamado cuando el formulario se ha cerrado. Si el thread de atención al servidor está en marcha lo detiene.
            if (atender != null && !mainAbierto)
                if (atender.IsAlive)
                {
                    MessageBox.Show("Abortado");
                    atender.Abort();
                }
        }
    }
}
