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
        //Variable donde se almacena todos los datos relacionados con la conexión al servidor.
        Server server;

        //Variables para almacenar parámetros globales de todo el formulario: Nombre del usuario, lista de usuarios seleccionados
        //en la tabla de conectados y número de seleccionados.
        string usuario;
        string lista_seleccionados = null;
        int num_invitados;

        //Lista de formularios de partidas, threads y contador.
        List<NuevaPartida> lista_forms_partidas = new List<NuevaPartida>();
        Thread ThreadNuevaPartida;
        int cont_forms = 0;

        public delegate void DelegadoMain(NuevaPartida form);
        public delegate void DelegadoRespuesta(string mensaje);

        public Main(Server server, string usuario)
        {
            //Constructor del formulario. Carga los parámetros de entrada en las variables globales propias.
            InitializeComponent();
            this.server = server;
            this.usuario = usuario;
        }

        public void TomaRespuesta(int codigo, string mensaje)
        {
            DelegadoRespuesta delegado;
            switch (codigo)
            {
                case 3:
                    delegado = new DelegadoRespuesta(Accion3);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 4:
                    delegado = new DelegadoRespuesta(Accion4);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 5:
                    delegado = new DelegadoRespuesta(Accion5);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 6:
                    delegado = new DelegadoRespuesta(Accion6);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 7:
                    delegado = new DelegadoRespuesta(Accion7);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 8:
                    delegado = new DelegadoRespuesta(Accion8);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 9:
                    delegado = new DelegadoRespuesta(Accion9);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 10:
                    delegado = new DelegadoRespuesta(Accion10);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 11:
                    delegado = new DelegadoRespuesta(Accion11);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 12:
                    delegado = new DelegadoRespuesta(Accion12);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 13:
                    delegado = new DelegadoRespuesta(Accion13);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 14:
                    delegado = new DelegadoRespuesta(Accion14);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 15:
                    delegado = new DelegadoRespuesta(Accion15);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;

                case 16:
                    delegado = new DelegadoRespuesta(Accion16);
                    this.Invoke(delegado, new object[] { mensaje });
                    break;
            } 
        }

        public void Accion3(string mensaje)
        {
            //Esta función es llamada cada vez que el thread de atención al servidor recibe un mensaje con el código 3,
            //que significa que la lista de usuarios conectados al servidor se ha actualizado. 
            //Por tanto, esta función se encarga de actualizar la lista de conectados del DataGridView del formulario del cliente.

            //Primero de todo, vaciamos la tabla para que no salgan datos anteriores
            ConectadosGrid.ClearSelection();
            for(int j = 0; j < ConectadosGrid.RowCount; j++)
                ConectadosGrid[0, j].Value = "";
            string[] separado = mensaje.Split('/');
            if (separado.Length > 1)
            {
                //Configuración de parámetros de la tabla: Una columna, tantas filas como conectados menos 1 (el propio usuario),
                //sin cabeceras y sin casillas preseleccionadas.
                ConectadosGrid.ColumnCount = 1;
                ConectadosGrid.RowCount = separado.Length - 1;
                ConectadosGrid.ColumnHeadersVisible = false;
                ConectadosGrid.RowHeadersVisible = false;
                ConectadosGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                ConectadosGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
                ConectadosGrid.ClearSelection();

                //Introducimos el nombre de los usuarios conectados en la DataGridView sin incluir al propio usuario
                int j = 0;
                bool encontrado = false;
                while(j < separado.Length && !encontrado)
                {
                    if (separado[j] != this.usuario)
                        ConectadosGrid[0, j].Value = separado[j];
                    else
                        encontrado = true;
                    j++;
                }
                while (j < separado.Length)
                {
                    ConectadosGrid[0, j - 1].Value = separado[j];
                    j++;
                }

                //Configura la alineación de las columnas al centro
                this.ConectadosGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        public void Accion4(string mensaje)
        {
            //Esta función es llamada cada vez que un usuario recibe un mensaje con el código 4, es decir, cada vez que el programa
            //recibe un mensaje para asignar un identificador a una partida que el cliente acaba de crear..
            //Esta función se encarga de reenviar el mensaje al último formulario creado.
            lista_forms_partidas[cont_forms - 1].TomaRespuesta(4, mensaje);
        }

        public void Accion5(string mensaje)
        {
            //Esta función es llamada cada vez que un usuario recibe un mensaje con el código 5, es decir, cada vez que el usuario
            //recibe una invitación de otro jugador.
            //Esta función se encarga de procesar la solicitud y enviar la respuesta que el cliente haya decidido.
            string[] separado = mensaje.Split('/');
            InvitacionRecibida notificacion_form = new InvitacionRecibida(separado[0]);
            DialogResult respuesta = notificacion_form.ShowDialog();

            if (respuesta == (DialogResult)1)
            {
                //Se acepta la partida
                server.Enviar("5/" + separado[1] + "/1");

                //Abrimos el formulario que contiene la sala con la partida a la que se quiere jugar.
                ThreadStart ts = delegate { AbrirFormularioPartidaCreada(Convert.ToInt32(separado[1])); };
                ThreadNuevaPartida = new Thread(ts);
                ThreadNuevaPartida.Start();
            }
            else
            {
                //Se rechaza la partida
                server.Enviar("5/" + separado[1] + "/0");
            }
        }

        public void Accion6(string mensaje)
        {
            //Esta función es llamada cada vez que un usuario recibe un mensaje con el código 6, es decir, cada vez que el programa
            //recibe un mensaje para mostrar en algún chat de partida.
            //Esta función se encarga de averiguar la partida a la cual pertenece el mensaje y lo reenvía al formulario en cuestión.
            string[] separado = mensaje.Split('/');
            for (int j = 0; j < cont_forms; j++)
                if (lista_forms_partidas[j].getID() == Convert.ToInt32(separado[0]))
                    lista_forms_partidas[j].TomaRespuesta(6, separado[1]);
        }

        public void Accion7(string mensaje)
        {
            //Esta función es llamada cada vez que un usuario recibe un mensaje con el código 7, es decir, cuando el host de una partida se
            //ha ido y este cliente debe pasar a ser el nuevo host.
            //Esta función se encarga de averiguar la partida a la cual pertenece el mensaje y lo reenvía al formulario en cuestión.
            string[] separado = mensaje.Split('/');
            for (int j = 0; j < cont_forms; j++)
                if (lista_forms_partidas[j].getID() == Convert.ToInt32(separado[0]))
                    lista_forms_partidas[j].TomaRespuesta(7, separado[1]);
        }

        public void Accion8(string mensaje)
        {
            if (mensaje == "0")
            {
                MessageBox.Show("Tu cuenta se ha eliminado correctamente. ¡Gracias por jugar!");
                this.Close();
            }
            else
            {
                MessageBox.Show("Ha ocurrido algún problema para eliminar tu cuenta. Vuelve a intentarlo.");
            }

        }

        public void Accion9(string mensaje)
        {
            string[] separado = mensaje.Split('/');

            if (separado[0] == "-2")
            {
                MessageBox.Show("Ha sucedido un error. Vuélvalo a intentar");
                this.Close();
            }
            else if (separado[0] == "-1")
            {
                MessageBox.Show("No se han realizado partidas con ese jugador.");
            }

            else if (separado[0] == "0")
            {
                int i = 2;
                string ganadores = null;

                while (i - 2 < Convert.ToInt32(separado[1]))
                {
                    ganadores += separado[i] + ",";
                    i++;
                }
                MessageBox.Show(ganadores);
            }
        }

        public void Accion10(string mensaje)
        {
            
        }

        public void Accion11(string mensaje)
        {
            
        }

        public void Accion12(string mensaje)
        {
            string[] separado = mensaje.Split('/');
            for (int j = 0; j < cont_forms; j++)
                if (lista_forms_partidas[j].getID() == Convert.ToInt32(separado[0]))
                {
                    if (Convert.ToInt32(separado[1]) == 1)
                        lista_forms_partidas[j].TomaRespuesta(12, separado[1] + "/" + separado[2]);
                    else if (Convert.ToInt32(separado[1]) == 2)
                        lista_forms_partidas[j].TomaRespuesta(12, separado[1] + "/" + separado[2] + "/" + separado[3]);
                    else if (Convert.ToInt32(separado[1]) == 3)
                        lista_forms_partidas[j].TomaRespuesta(12, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4]);
                    else if (Convert.ToInt32(separado[1]) == 4)
                        lista_forms_partidas[j].TomaRespuesta(12, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5]);
                    else if (Convert.ToInt32(separado[1]) == 5)
                        lista_forms_partidas[j].TomaRespuesta(12, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6]);
                    else if (Convert.ToInt32(separado[1]) == 6)
                        lista_forms_partidas[j].TomaRespuesta(12, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6] + "/" + separado[7]);
                    else if (Convert.ToInt32(separado[1]) == 7)
                        lista_forms_partidas[j].TomaRespuesta(12, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6] + "/" + separado[7] + "/" + separado[8]);
                    else if (Convert.ToInt32(separado[1]) == 8)
                        lista_forms_partidas[j].TomaRespuesta(12, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6] + "/" + separado[7] + "/" + separado[8] + "/" + separado[9]);
                }
        }

        public void Accion13(string mensaje)
        {

            string[] separado = mensaje.Split('/');
            for (int j = 0; j < cont_forms; j++)
                if (lista_forms_partidas[j].getID() == Convert.ToInt32(separado[0]))
                    lista_forms_partidas[j].TomaRespuesta(13, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4]);
        }

        public void Accion14(string mensaje)
        {
            string[] separado = mensaje.Split('/');
            for (int j = 0; j < cont_forms; j++)
                if (lista_forms_partidas[j].getID() == Convert.ToInt32(separado[0]))
                {
                    //Accion = 0/N cuando al principio apuesta N fichas
                    if (separado[2] == "0")
                        lista_forms_partidas[j].TomaRespuesta(14, separado[1] + "/" + separado[2] + "/" + separado[3]);
                    //-1 cuando se retira
                    else if (separado[2] == "-1")
                        lista_forms_partidas[j].TomaRespuesta(14, separado[1] + "/" + separado[2]);
                    //1 cuando pide carta
                    else if (separado[2] == "1")
                        lista_forms_partidas[j].TomaRespuesta(14, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4]);
                    //3 cuando dobla
                    else if (separado[2] == "3")
                        lista_forms_partidas[j].TomaRespuesta(14, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4]);
                }
        }

        public void Accion15(string mensaje)
        {
            string[] separado = mensaje.Split('/');
            for (int j = 0; j < cont_forms; j++)
                if (lista_forms_partidas[j].getID() == Convert.ToInt32(separado[0]))
                {
                    if(separado[4] == "0")
                        lista_forms_partidas[j].TomaRespuesta(15, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4]);
                    else if (separado[4] == "1")
                        lista_forms_partidas[j].TomaRespuesta(15, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5]);
                    else if (separado[4] == "2")
                        lista_forms_partidas[j].TomaRespuesta(15, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6]);
                    else if (separado[4] == "3")
                        lista_forms_partidas[j].TomaRespuesta(15, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6] + "/" + separado[7]);
                    else if (separado[4] == "4")
                        lista_forms_partidas[j].TomaRespuesta(15, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6] + "/" + separado[7] + "/" + separado[8]);
                    else if (separado[4] == "5")
                        lista_forms_partidas[j].TomaRespuesta(15, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6] + "/" + separado[7] + "/" + separado[8] + "/" + separado[9]);
                    else if (separado[4] == "6")
                        lista_forms_partidas[j].TomaRespuesta(15, separado[1] + "/" + separado[2] + "/" + separado[3] + "/" + separado[4] + "/" + separado[5] + "/" + separado[6] + "/" + separado[7] + "/" + separado[8] + "/" + separado[9] + "/" + separado[10]);
                }
        }

        public void Accion16(string mensaje)
        {
            string[] separado = mensaje.Split('/');
            for (int j = 0; j < cont_forms; j++)
                if (lista_forms_partidas[j].getID() == Convert.ToInt32(separado[0]))
                    lista_forms_partidas[j].TomaRespuesta(16, "");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Evento de carga del formulario.
            nombreJugadorLb.Text = this.usuario;

            //Pedimos al servidor que nos envie exclusivamente a nosotros la lista de conectados actualizada
            server.Enviar("3/");
            ConectadosGrid.ClearSelection();
        }

        private void ConectadosGrid_SelectionChanged(object sender, EventArgs e)
        {
            //Este evento es llamado cada vez que el usuario cambia la selección de casillas de la tabla de usuarios conectados.
            //Guarda el número de invitados seleccionados y la lista de seleccionados en variables globales del formulario.
            num_invitados = ConectadosGrid.SelectedCells.Count;
            lista_seleccionados = "";
            for (int i = 0; i < num_invitados; i++)
                lista_seleccionados = lista_seleccionados + "/" + ConectadosGrid.SelectedCells[i].Value;
        }

        private void NuevaPartidaBtn_Click(object sender, EventArgs e)
        {
            //Este evento es llamado cada vez que el usuario pulsa el botón "Invitar selección". Se envía un mensaje al servidor
            //para que invite a los jugadores seleccionados en la tabla y abre un formulario con la partida nueva.
            if (lista_seleccionados == "" || lista_seleccionados == null)
                //Si no hay ninguna casilla seleccionada, avisa al usuario y no se hace nada más a la espera de que el usuario rectifique.
                MessageBox.Show("Debes seleccionar a jugadores para invitarlos.");
            else
            {
                string mensaje = "4/" + num_invitados + lista_seleccionados;
                //Iniciamos el thread de la partida.
                ThreadStart ts = delegate { AbrirFormularioNuevaPartida(mensaje); };
                ThreadNuevaPartida = new Thread(ts);
                ThreadNuevaPartida.Start();
            }
        }

        public void AnadirFormPartida(NuevaPartida form)
        {
            lista_forms_partidas.Add(form);
            cont_forms++;
        }

        private void AbrirFormularioPartidaCreada(int ID)
        {
            //Esta función es llamada cada vez que se inicia un thread para crear un formulario con una partida creada por otro usuario.
            //Se abre el otro formulario y se añade a la lista de formularios.
            NuevaPartida form = new NuevaPartida(this.server, this.usuario, ID);

            DelegadoMain delegado = new DelegadoMain(AnadirFormPartida);
            this.Invoke(delegado, new object[] {form});

            form.ShowDialog();


            //Eliminar de la lista de formularios, el formulario que se acaba de cerrar (con RemoveAt()?)

        }

        private void AbrirFormularioNuevaPartida(string mensaje)
        {
            //Esta función es llamada cada vez que se inicia un thread para crear un formulario con una partida nueva.
            //Se abre el otro formulario y se añade a la lista de formularios.
            NuevaPartida form = new NuevaPartida(this.server, this.usuario);
            lista_forms_partidas.Add(form);
            cont_forms++;
            server.Enviar(mensaje);
            form.ShowDialog();

            //Eliminar de la lista de formularios, el formulario que se acaba de cerrar (con RemoveAt()?)
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Este evento es llamado cuando el formulario se va a cerrar. Si hay alguna partida en ejecución la aborta y 
            //desconecta al usuario del servidor.
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
            //Este evento es llamado cuando se pulsa el botón "Cerrar sesión". Si hay alguna partida en ejecución la aborta y 
            //desconecta al usuario del servidor. Por último, vuelve a abrir el formulario Login.
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

        private void darseDeBajaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            darse_de_baja form = new darse_de_baja(server);
            form.ShowDialog();
        }

        private void resultadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            consulta_resultados form = new consulta_resultados(server);
            form.ShowDialog();
        }
    }
}
