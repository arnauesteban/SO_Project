using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace Cliente
{
    public partial class NuevaPartida : Form
    {
        //Variables globales del formulario: datos de conexión con el servidor, identificador de partida, nombre del usuario y dealer de la ronda.
        Server server;
        int ID;
        string usuario;
        string dealer;
        int maxJugado;
        int lineasChat;

        public delegate void DelegadoRespuesta(string mensaje);

        public NuevaPartida(Server server, string usuario)
        {
            //Constructor del formulario usado cuando el usuario es el host de la partida.
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.server = server;
            this.usuario = usuario;
            usuarioLbl.Text = this.usuario;
            chatLbl.Text = "¡Bienvenido a una nueva partida de poker! Espera a que tus contrincantes acepten tu invitación o pulsa el botón del centro de la mesa.";
            lineasChat = 4;
            /*fichasNum.Enabled = true;
            ciegaNum.Enabled = true;
            string mensaje = "8/" + this.ID + "/0/" + fichasNum.Value + "/" + ciegaNum.Value;
            server.Enviar(mensaje);
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarNum.Enabled = false;
            apostarNum.Visible = false;*/

            /*
            jugador2NombreLbl.Text = "";
            jugador2Carta1Lbl.Text = "";
            jugador2Carta2Lbl.Text = "";
            jugador2FichasLbl.Text = "";
            jugador2JugadoLbl.Text = "";
            jugador3NombreLbl.Text = "";
            jugador3Carta1Lbl.Text = "";
            jugador3Carta2Lbl.Text = "";
            jugador3FichasLbl.Text = "";
            jugador3JugadoLbl.Text = "";
            jugador4NombreLbl.Text = "";
            jugador4Carta1Lbl.Text = "";
            jugador4Carta2Lbl.Text = "";
            jugador4FichasLbl.Text = "";
            jugador4JugadoLbl.Text = "";
            jugador5NombreLbl.Text = "";
            jugador5Carta1Lbl.Text = "";
            jugador5Carta2Lbl.Text = "";
            jugador5FichasLbl.Text = "";
            jugador5JugadoLbl.Text = "";
            jugador6NombreLbl.Text = "";
            jugador6Carta1Lbl.Text = "";
            jugador6Carta2Lbl.Text = "";
            jugador6FichasLbl.Text = "";
            jugador6JugadoLbl.Text = "";
            jugador7NombreLbl.Text = "";
            jugador7Carta1Lbl.Text = "";
            jugador7Carta2Lbl.Text = "";
            jugador7FichasLbl.Text = "";
            jugador7JugadoLbl.Text = "";
            jugador8NombreLbl.Text = "";
            jugador8Carta1Lbl.Text = "";
            jugador8Carta2Lbl.Text = "";
            jugador8FichasLbl.Text = "";
            jugador8JugadoLbl.Text = "";
            */
        }

        public NuevaPartida(Server server, string usuario, int ID)
        {
            //Constructor del formulario usado cuando el usuario acepta una invitación de otro jugador.
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.server = server;
            this.usuario = usuario;
            this.ID = ID;
            usuarioLbl.Text = this.usuario;
            empezarBtn.Enabled = false;
            this.Text = "Partida " + this.ID;
            chatLbl.Text = "¡Bienvenido a una nueva partida de poker! Espera a que otros contrincantes acepten la invitación o a que el host pulse el botón del centro de la mesa.";
            lineasChat = 4;
            /*fichasNum.Enabled = false;
            ciegaNum.Enabled = false;
            string mensaje = "8/" + this.ID + "/2";
            server.Enviar(mensaje);
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarNum.Enabled = false;
            apostarNum.Visible = false;*/
        }

        public int getID()
        {
            //Método que devuelve el identificador de partida.
            return this.ID;
        }

        public void TomaRespuesta(int codigo, string mensaje)
        {
            DelegadoRespuesta delegado;
            switch (codigo)
            {
                case 4:
                    delegado = new DelegadoRespuesta(Accion4);
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
                    //delegado = new DelegadoRespuesta(Accion8);
                    //this.Invoke(delegado, new object[] { mensaje });
                    Accion8(mensaje);
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
            }
        }

        public void Accion4(string mensaje)
        {
            //Función que es llamada cuando el usuario host recibe el identificador de la partida que acaba de crear.
            this.ID = Convert.ToInt32(mensaje);
            this.Text = "Partida " + this.ID;
        }

        public void Accion6(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje para el chat de esta partida. 
            //Añade el mensaje en una nueva línea del chat.
            lineasChat += (mensaje.Length / 32 + 1);
            if(lineasChat > 28)
            {
                string[] separado = chatLbl.Text.Split('\n');
                chatLbl.Text = separado[1];
                for(int i = 2; i < separado.Length; i++)
                    chatLbl.Text = chatLbl.Text + '\n' + separado[i];
            }
            chatLbl.Text = chatLbl.Text + '\n' + mensaje;
        }

        public void Accion7(string mensaje)
        {
            //Función que es llamada cuando se recibe un mensaje para que este jugador pase a ser el host de la partida. 
            //Permite usar los objetos del formulario reservados al host
            /*empezarBtn.Enabled = true;
            fichasNum.Enabled = true;
            ciegaNum.Enabled = true;*/
        }

        public void Accion8(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje para configurar los parámetros iniciales de esta partida
            //Guarda los nuevos valores y los muestra al usuario
            /*string[] separado = mensaje.Split('/');
            //Si numConfig vale 0, solo se nos ha enviado las fichas y ciega iniciales
            if (Convert.ToInt32(separado[0]) != 1)
            {
                fichasNum.Value = Convert.ToInt32(separado[1]);
                ciegaNum.Value = Convert.ToInt32(separado[2]);
            }
            //Si numConfig vale 2, además de las fichas y ciega también se nos envia la lista de jugadores de la partida
            if (Convert.ToInt32(separado[0]) != 0)
            {
                int n = Convert.ToInt32(separado[3]);
                jugador1NombreLbl.Text = separado[4];
                jugador1FichasLbl.Text = fichasNum.Value.ToString();
                jugador1JugadoLbl.Text = "0";
                jugador2NombreLbl.Text = separado[5];
                jugador2FichasLbl.Text = fichasNum.Value.ToString();
                jugador2JugadoLbl.Text = "0";
                if (n > 2)
                {
                    jugador3NombreLbl.Text = separado[6];
                    jugador3FichasLbl.Text = fichasNum.Value.ToString();
                    jugador3JugadoLbl.Text = "0";
                    if (n > 3)
                    {
                        jugador4NombreLbl.Text = separado[7];
                        jugador4FichasLbl.Text = fichasNum.Value.ToString();
                        jugador4JugadoLbl.Text = "0";
                        if (n > 4)
                        {
                            jugador5NombreLbl.Text = separado[8];
                            jugador5FichasLbl.Text = fichasNum.Value.ToString();
                            jugador5JugadoLbl.Text = "0";
                            if (n > 5)
                            {
                                jugador6NombreLbl.Text = separado[9];
                                jugador6FichasLbl.Text = fichasNum.Value.ToString();
                                jugador6JugadoLbl.Text = "0";
                                if (n > 6)
                                {
                                    jugador7NombreLbl.Text = separado[10];
                                    jugador7FichasLbl.Text = fichasNum.Value.ToString();
                                    jugador7JugadoLbl.Text = "0";
                                    if (n > 7)
                                    {
                                        jugador8NombreLbl.Text = separado[11];
                                        jugador8FichasLbl.Text = fichasNum.Value.ToString();
                                        jugador8JugadoLbl.Text = "0";
                                    }
                                    else
                                    {
                                        jugador8NombreLbl.Text = "";
                                        jugador8Carta1Lbl.Text = "";
                                        jugador8Carta2Lbl.Text = "";
                                        jugador8FichasLbl.Text = "";
                                        jugador8JugadoLbl.Text = "";
                                    }
                                }
                                else
                                {
                                    jugador7NombreLbl.Text = "";
                                    jugador7Carta1Lbl.Text = "";
                                    jugador7Carta2Lbl.Text = "";
                                    jugador7FichasLbl.Text = "";
                                    jugador7JugadoLbl.Text = "";
                                    jugador8NombreLbl.Text = "";
                                    jugador8Carta1Lbl.Text = "";
                                    jugador8Carta2Lbl.Text = "";
                                    jugador8FichasLbl.Text = "";
                                    jugador8JugadoLbl.Text = "";
                                }
                            }
                            else
                            {
                                jugador6NombreLbl.Text = "";
                                jugador6Carta1Lbl.Text = "";
                                jugador6Carta2Lbl.Text = "";
                                jugador6FichasLbl.Text = "";
                                jugador6JugadoLbl.Text = "";
                                jugador7NombreLbl.Text = "";
                                jugador7Carta1Lbl.Text = "";
                                jugador7Carta2Lbl.Text = "";
                                jugador7FichasLbl.Text = "";
                                jugador7JugadoLbl.Text = "";
                                jugador8NombreLbl.Text = "";
                                jugador8Carta1Lbl.Text = "";
                                jugador8Carta2Lbl.Text = "";
                                jugador8FichasLbl.Text = "";
                                jugador8JugadoLbl.Text = "";
                            }
                        }
                        else
                        {
                            jugador5NombreLbl.Text = "";
                            jugador5Carta1Lbl.Text = "";
                            jugador5Carta2Lbl.Text = "";
                            jugador5FichasLbl.Text = "";
                            jugador5JugadoLbl.Text = "";
                            jugador6NombreLbl.Text = "";
                            jugador6Carta1Lbl.Text = "";
                            jugador6Carta2Lbl.Text = "";
                            jugador6FichasLbl.Text = "";
                            jugador6JugadoLbl.Text = "";
                            jugador7NombreLbl.Text = "";
                            jugador7Carta1Lbl.Text = "";
                            jugador7Carta2Lbl.Text = "";
                            jugador7FichasLbl.Text = "";
                            jugador7JugadoLbl.Text = "";
                            jugador8NombreLbl.Text = "";
                            jugador8Carta1Lbl.Text = "";
                            jugador8Carta2Lbl.Text = "";
                            jugador8FichasLbl.Text = "";
                            jugador8JugadoLbl.Text = "";
                        }
                    }
                    else
                    {
                        jugador4NombreLbl.Text = "";
                        jugador4Carta1Lbl.Text = "";
                        jugador4Carta2Lbl.Text = "";
                        jugador4FichasLbl.Text = "";
                        jugador4JugadoLbl.Text = "";
                        jugador5NombreLbl.Text = "";
                        jugador5Carta1Lbl.Text = "";
                        jugador5Carta2Lbl.Text = "";
                        jugador5FichasLbl.Text = "";
                        jugador5JugadoLbl.Text = "";
                        jugador6NombreLbl.Text = "";
                        jugador6Carta1Lbl.Text = "";
                        jugador6Carta2Lbl.Text = "";
                        jugador6FichasLbl.Text = "";
                        jugador6JugadoLbl.Text = "";
                        jugador7NombreLbl.Text = "";
                        jugador7Carta1Lbl.Text = "";
                        jugador7Carta2Lbl.Text = "";
                        jugador7FichasLbl.Text = "";
                        jugador7JugadoLbl.Text = "";
                        jugador8NombreLbl.Text = "";
                        jugador8Carta1Lbl.Text = "";
                        jugador8Carta2Lbl.Text = "";
                        jugador8FichasLbl.Text = "";
                        jugador8JugadoLbl.Text = "";
                    }
                }
                else
                {
                    /*jugador3NombreLbl.Text = "";
                    jugador3Carta1Lbl.Text = "";
                    jugador3Carta2Lbl.Text = "";
                    jugador3FichasLbl.Text = "";
                    jugador3JugadoLbl.Text = "";
                    jugador4NombreLbl.Text = "";
                    jugador4Carta1Lbl.Text = "";
                    jugador4Carta2Lbl.Text = "";
                    jugador4FichasLbl.Text = "";
                    jugador4JugadoLbl.Text = "";
                    jugador5NombreLbl.Text = "";
                    jugador5Carta1Lbl.Text = "";
                    jugador5Carta2Lbl.Text = "";
                    jugador5FichasLbl.Text = "";
                    jugador5JugadoLbl.Text = "";
                    jugador6NombreLbl.Text = "";
                    jugador6Carta1Lbl.Text = "";
                    jugador6Carta2Lbl.Text = "";
                    jugador6FichasLbl.Text = "";
                    jugador6JugadoLbl.Text = "";
                    jugador7NombreLbl.Text = "";
                    jugador7Carta1Lbl.Text = "";
                    jugador7Carta2Lbl.Text = "";
                    jugador7FichasLbl.Text = "";
                    jugador7JugadoLbl.Text = "";
                    jugador8NombreLbl.Text = "";
                    jugador8Carta1Lbl.Text = "";
                    jugador8Carta2Lbl.Text = "";
                    jugador8FichasLbl.Text = "";
                    jugador8JugadoLbl.Text = "";
                }
            }*/
        }

        public void Accion9(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje para mostrar cartas
            //Quita los objetos de los parámetros iniciales y muestra las cartas que lleguen
            /*empezarBtn.Visible = false;
            fichasNum.Visible = false;
            fichasLbl.Visible = false;
            ciegaLbl.Visible = false;
            ciegaNum.Visible = false;
            string[] separado = mensaje.Split('/');
            //Si el estado vale 0, la ronda acaba de comenzar y se nos ha enviado ls cartas de la mano
            if (Convert.ToInt32(separado[0]) == 0)
            {
                this.dealer = separado[3];
                if (jugador1NombreLbl.Text == this.usuario)
                {
                    jugador1Carta1Lbl.Text = separado[1];
                    jugador1Carta2Lbl.Text = separado[2];
                }
                else if (jugador2NombreLbl.Text == this.usuario)
                {
                    jugador2Carta1Lbl.Text = separado[1];
                    jugador2Carta2Lbl.Text = separado[2];
                }
                else if (jugador3NombreLbl.Text == this.usuario)
                {
                    jugador3Carta1Lbl.Text = separado[1];
                    jugador3Carta2Lbl.Text = separado[2];
                }
                else if (jugador4NombreLbl.Text == this.usuario)
                {
                    jugador4Carta1Lbl.Text = separado[1];
                    jugador4Carta2Lbl.Text = separado[2];
                }
                else if (jugador5NombreLbl.Text == this.usuario)
                {
                    jugador5Carta1Lbl.Text = separado[1];
                    jugador5Carta2Lbl.Text = separado[2];
                }
                else if (jugador6NombreLbl.Text == this.usuario)
                {
                    jugador6Carta1Lbl.Text = separado[1];
                    jugador6Carta2Lbl.Text = separado[2];
                }
                else if (jugador7NombreLbl.Text == this.usuario)
                {
                    jugador7Carta1Lbl.Text = separado[1];
                    jugador7Carta2Lbl.Text = separado[2];
                }
                else if (jugador8NombreLbl.Text == this.usuario)
                {
                    jugador8Carta1Lbl.Text = separado[1];
                    jugador8Carta2Lbl.Text = separado[2];
                }
            }
            //Si el estado vale 1, se nos está enviando las tres primeras cartas del centro
            else if (Convert.ToInt32(separado[0]) == 1)
            {
                carta1Lbl.Text = separado[1];
                carta2Lbl.Text = separado[2];
                carta3Lbl.Text = separado[3];
            }
            //Si el estado vale 2, se nos está enviando la cuarta carta del centro
            else if (Convert.ToInt32(separado[0]) == 2)
            {
                carta4Lbl.Text = separado[1];
            }
            //Si el estado vale 3, se nos está enviando la quinta carta del centro
            else if (Convert.ToInt32(separado[0]) == 3)
            {
                carta5Lbl.Text = separado[1];
            }
            //Si el estado vale 5, se nos está enviando el número del ganador de la ronda. Se le debe sumar a las fichas lo jugado por todos
            else if (Convert.ToInt32(separado[0]) == 5)
            {
                int suma = Convert.ToInt32(jugador1JugadoLbl.Text) + Convert.ToInt32(jugador2JugadoLbl.Text) + Convert.ToInt32(jugador3JugadoLbl.Text) +
                    Convert.ToInt32(jugador4JugadoLbl.Text) + Convert.ToInt32(jugador5JugadoLbl.Text) + Convert.ToInt32(jugador6JugadoLbl.Text) + 
                    Convert.ToInt32(jugador7JugadoLbl.Text) + Convert.ToInt32(jugador8JugadoLbl.Text);
                int n = Convert.ToInt32(separado[1]);
                if (n == 0)
                    jugador1FichasLbl.Text = (Convert.ToInt32(jugador1FichasLbl.Text) + suma).ToString();
                else
                    jugador1FichasLbl.Text = (Convert.ToInt32(jugador1FichasLbl.Text) - (Convert.ToInt32(jugador1JugadoLbl.Text))).ToString();
                if (n == 1)
                    jugador2FichasLbl.Text = (Convert.ToInt32(jugador2FichasLbl.Text) + suma).ToString();
                else
                    jugador2FichasLbl.Text = (Convert.ToInt32(jugador2FichasLbl.Text) - (Convert.ToInt32(jugador2JugadoLbl.Text))).ToString();
                if (n == 2)
                    jugador3FichasLbl.Text = (Convert.ToInt32(jugador3FichasLbl.Text) + suma).ToString();
                else
                    jugador3FichasLbl.Text = (Convert.ToInt32(jugador3FichasLbl.Text) - (Convert.ToInt32(jugador3JugadoLbl.Text))).ToString();
                if (n == 3)
                    jugador4FichasLbl.Text = (Convert.ToInt32(jugador4FichasLbl.Text) + suma).ToString();
                else
                    jugador4FichasLbl.Text = (Convert.ToInt32(jugador4FichasLbl.Text) - (Convert.ToInt32(jugador4JugadoLbl.Text))).ToString();
                if (n == 4)
                    jugador5FichasLbl.Text = (Convert.ToInt32(jugador5FichasLbl.Text) + suma).ToString();
                else
                    jugador5FichasLbl.Text = (Convert.ToInt32(jugador5FichasLbl.Text) - (Convert.ToInt32(jugador5JugadoLbl.Text))).ToString();
                if (n == 5)
                    jugador6FichasLbl.Text = (Convert.ToInt32(jugador6FichasLbl.Text) + suma).ToString();
                else
                    jugador6FichasLbl.Text = (Convert.ToInt32(jugador6FichasLbl.Text) - (Convert.ToInt32(jugador6JugadoLbl.Text))).ToString();
                if (n == 6)
                    jugador7FichasLbl.Text = (Convert.ToInt32(jugador7FichasLbl.Text) + suma).ToString();
                else
                    jugador7FichasLbl.Text = (Convert.ToInt32(jugador7FichasLbl.Text) - (Convert.ToInt32(jugador7JugadoLbl.Text))).ToString();
                if (n == 7)
                    jugador8FichasLbl.Text = (Convert.ToInt32(jugador8FichasLbl.Text) + suma).ToString();
                else
                    jugador8FichasLbl.Text = (Convert.ToInt32(jugador8FichasLbl.Text) - (Convert.ToInt32(jugador8JugadoLbl.Text))).ToString();
                jugador1JugadoLbl.Text = "0";
                jugador1Carta1Lbl.Text = "Carta 1";
                jugador1Carta2Lbl.Text = "Carta 2";
                jugador2JugadoLbl.Text = "0";
                jugador2Carta1Lbl.Text = "Carta 1";
                jugador2Carta2Lbl.Text = "Carta 2";
                jugador3JugadoLbl.Text = "0";
                jugador3Carta1Lbl.Text = "Carta 1";
                jugador3Carta2Lbl.Text = "Carta 2";
                jugador4JugadoLbl.Text = "0";
                jugador4Carta1Lbl.Text = "Carta 1";
                jugador4Carta2Lbl.Text = "Carta 2";
                jugador5JugadoLbl.Text = "0";
                jugador5Carta1Lbl.Text = "Carta 1";
                jugador5Carta2Lbl.Text = "Carta 2";
                jugador6JugadoLbl.Text = "0";
                jugador6Carta1Lbl.Text = "Carta 1";
                jugador6Carta2Lbl.Text = "Carta 2";
                jugador7JugadoLbl.Text = "0";
                jugador7Carta1Lbl.Text = "Carta 1";
                jugador7Carta2Lbl.Text = "Carta 2";
                jugador8JugadoLbl.Text = "0";
                jugador8Carta1Lbl.Text = "Carta 1";
                jugador8Carta2Lbl.Text = "Carta 2";
                carta1Lbl.Text = "Carta 1";
                carta2Lbl.Text = "Carta 2";
                carta3Lbl.Text = "Carta 3";
                carta4Lbl.Text = "Carta 4";
                carta5Lbl.Text = "Carta 5";
            }*/

        }

        public void Accion10(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje para mostrar la acción de algún jugador
            //Actualiza las etiquetas del jugador que ha hecho la acción
            string[] separado = mensaje.Split('/');
            int n = Convert.ToInt32(separado[0]);
            int accion = Convert.ToInt32(separado[1]);
            if (n == 0)
            {
                if (accion == -1)
                {
                    jugador1Carta1Lbl.Text = separado[1];
                    jugador1Carta2Lbl.Text = separado[1];
                }
                else
                    jugador1JugadoLbl.Text = (Convert.ToInt32(jugador1JugadoLbl.Text) + accion).ToString();
            }
            else if (n == 1)
            {
                if (accion == -1)
                {
                    jugador2Carta1Lbl.Text = separado[1];
                    jugador2Carta2Lbl.Text = separado[1];
                }
                else
                    jugador2JugadoLbl.Text = (Convert.ToInt32(jugador2JugadoLbl.Text) + accion).ToString();
            }
            else if (n == 2)
            {
                if (accion == -1)
                {
                    jugador3Carta1Lbl.Text = separado[1];
                    jugador3Carta2Lbl.Text = separado[1];
                }
                else
                    jugador3JugadoLbl.Text = (Convert.ToInt32(jugador3JugadoLbl.Text) + accion).ToString();
            }
            else if (n == 3)
            {
                if (accion == -1)
                {
                    jugador4Carta1Lbl.Text = separado[1];
                    jugador4Carta2Lbl.Text = separado[1];
                }
                else
                    jugador4JugadoLbl.Text = (Convert.ToInt32(jugador4JugadoLbl.Text) + accion).ToString();
            }
            else if (n == 4)
            {
                if (accion == -1)
                {
                    jugador5Carta1Lbl.Text = separado[1];
                    jugador5Carta2Lbl.Text = separado[1];
                }
                else
                    jugador5JugadoLbl.Text = (Convert.ToInt32(jugador5JugadoLbl.Text) + accion).ToString();
            }
            else if (n == 5)
            {
                if (accion == -1)
                {
                    jugador6Carta1Lbl.Text = separado[1];
                    jugador6Carta2Lbl.Text = separado[1];
                }
                else
                    jugador6JugadoLbl.Text = (Convert.ToInt32(jugador6JugadoLbl.Text) + accion).ToString();
            }
            else if (n == 6)
            {
                if (accion == -1)
                {
                    jugador7Carta1Lbl.Text = separado[1];
                    jugador7Carta2Lbl.Text = separado[1];
                }
                else
                    jugador7JugadoLbl.Text = (Convert.ToInt32(jugador7JugadoLbl.Text) + accion).ToString();
            }
            else if (n == 7)
            {
                if (accion == -1)
                {
                    jugador8Carta1Lbl.Text = separado[1];
                    jugador8Carta2Lbl.Text = separado[1];
                }
                else
                    jugador8JugadoLbl.Text = (Convert.ToInt32(jugador8JugadoLbl.Text) + accion).ToString();
            }
        }

        public void Accion11(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje para dar el turno a algún jugador
            //Habilita los botones de acciones si es el turno de este jugador
            if (mensaje == this.usuario)
            {
                //Buscamos cual es la puja máxima que se ha ehcho hasta el momento en las ronda
                if (Convert.ToInt32(jugador1JugadoLbl.Text) > maxJugado)
                    maxJugado = Convert.ToInt32(jugador1JugadoLbl.Text);
                if (Convert.ToInt32(jugador2JugadoLbl.Text) > maxJugado)
                    maxJugado = Convert.ToInt32(jugador1JugadoLbl.Text);
                if (Convert.ToInt32(jugador3JugadoLbl.Text) > maxJugado)
                    maxJugado = Convert.ToInt32(jugador1JugadoLbl.Text);
                if (Convert.ToInt32(jugador4JugadoLbl.Text) > maxJugado)
                    maxJugado = Convert.ToInt32(jugador1JugadoLbl.Text);
                if (Convert.ToInt32(jugador5JugadoLbl.Text) > maxJugado)
                    maxJugado = Convert.ToInt32(jugador1JugadoLbl.Text);
                if (Convert.ToInt32(jugador6JugadoLbl.Text) > maxJugado)
                    maxJugado = Convert.ToInt32(jugador1JugadoLbl.Text);
                if (Convert.ToInt32(jugador7JugadoLbl.Text) > maxJugado)
                    maxJugado = Convert.ToInt32(jugador1JugadoLbl.Text);
                if (Convert.ToInt32(jugador8JugadoLbl.Text) > maxJugado)
                    maxJugado = Convert.ToInt32(jugador1JugadoLbl.Text);

                //Editamos los textos de los botones según la situación de fichas del jugador
                if (jugador1NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador1FichasLbl.Text))
                    {
                        apostarNum.Minimum = maxJugado - Convert.ToInt32(jugador1JugadoLbl.Text);
                        apostarBtn.Text = "Subir";
                    }
                    else
                    {
                        apostarNum.Minimum = Convert.ToInt32(jugador1FichasLbl.Text) - Convert.ToInt32(jugador1JugadoLbl.Text);
                        apostarBtn.Text = "Jugarlo todo";
                    }
                    apostarNum.Maximum = Convert.ToInt32(jugador1FichasLbl.Text) - Convert.ToInt32(jugador1JugadoLbl.Text);
                    if(maxJugado == Convert.ToInt32(jugador1JugadoLbl.Text))
                        plantarseBtn.Text = "Pasar";
                    else
                        plantarseBtn.Text = "ir";
                }
                else if (jugador2NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador2FichasLbl.Text))
                    {
                        apostarNum.Minimum = maxJugado - Convert.ToInt32(jugador2JugadoLbl.Text);
                        apostarBtn.Text = "Subir";
                    }
                    else
                    {
                        apostarNum.Minimum = Convert.ToInt32(jugador2FichasLbl.Text) - Convert.ToInt32(jugador2JugadoLbl.Text);
                        apostarBtn.Text = "Jugarlo todo";
                    }
                    apostarNum.Maximum = Convert.ToInt32(jugador2FichasLbl.Text) - Convert.ToInt32(jugador2JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador2JugadoLbl.Text))
                        plantarseBtn.Text = "Pasar";
                    else
                        plantarseBtn.Text = "ir";
                }
                else if (jugador3NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador3FichasLbl.Text))
                    {
                        apostarNum.Minimum = maxJugado - Convert.ToInt32(jugador3JugadoLbl.Text);
                        apostarBtn.Text = "Subir";
                    }
                    else
                    {
                        apostarNum.Minimum = Convert.ToInt32(jugador3FichasLbl.Text) - Convert.ToInt32(jugador3JugadoLbl.Text);
                        apostarBtn.Text = "Jugarlo todo";
                    }
                    apostarNum.Maximum = Convert.ToInt32(jugador3FichasLbl.Text) - Convert.ToInt32(jugador3JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador3JugadoLbl.Text))
                        plantarseBtn.Text = "Pasar";
                    else
                        plantarseBtn.Text = "ir";
                }
                else if (jugador4NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador4FichasLbl.Text))
                    {
                        apostarNum.Minimum = maxJugado - Convert.ToInt32(jugador4JugadoLbl.Text);
                        apostarBtn.Text = "Subir";
                    }
                    else
                    {
                        apostarNum.Minimum = Convert.ToInt32(jugador4FichasLbl.Text) - Convert.ToInt32(jugador4JugadoLbl.Text);
                        apostarBtn.Text = "Jugarlo todo";
                    }
                    apostarNum.Maximum = Convert.ToInt32(jugador4FichasLbl.Text) - Convert.ToInt32(jugador4JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador4JugadoLbl.Text))
                        plantarseBtn.Text = "Pasar";
                    else
                        plantarseBtn.Text = "ir";
                }
                else if (jugador5NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador5FichasLbl.Text))
                    {
                        apostarNum.Minimum = maxJugado - Convert.ToInt32(jugador5JugadoLbl.Text);
                        apostarBtn.Text = "Subir";
                    }
                    else
                    {
                        apostarNum.Minimum = Convert.ToInt32(jugador5FichasLbl.Text) - Convert.ToInt32(jugador5JugadoLbl.Text);
                        apostarBtn.Text = "Jugarlo todo";
                    }
                    apostarNum.Maximum = Convert.ToInt32(jugador5FichasLbl.Text) - Convert.ToInt32(jugador5JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador5JugadoLbl.Text))
                        plantarseBtn.Text = "Pasar";
                    else
                        plantarseBtn.Text = "ir";
                }
                else if (jugador6NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador6FichasLbl.Text))
                    {
                        apostarNum.Minimum = maxJugado - Convert.ToInt32(jugador6JugadoLbl.Text);
                        apostarBtn.Text = "Subir";
                    }
                    else
                    {
                        apostarNum.Minimum = Convert.ToInt32(jugador6FichasLbl.Text) - Convert.ToInt32(jugador6JugadoLbl.Text);
                        apostarBtn.Text = "Jugarlo todo";
                    }
                    apostarNum.Maximum = Convert.ToInt32(jugador6FichasLbl.Text) - Convert.ToInt32(jugador6JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador6JugadoLbl.Text))
                        plantarseBtn.Text = "Pasar";
                    else
                        plantarseBtn.Text = "ir";
                }
                else if (jugador7NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador7FichasLbl.Text))
                    {
                        apostarNum.Minimum = maxJugado - Convert.ToInt32(jugador7JugadoLbl.Text);
                        apostarBtn.Text = "Subir";
                    }
                    else
                    {
                        apostarNum.Minimum = Convert.ToInt32(jugador7FichasLbl.Text) - Convert.ToInt32(jugador7JugadoLbl.Text);
                        apostarBtn.Text = "Jugarlo todo";
                    }
                    apostarNum.Maximum = Convert.ToInt32(jugador7FichasLbl.Text) - Convert.ToInt32(jugador7JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador7JugadoLbl.Text))
                        plantarseBtn.Text = "Pasar";
                    else
                        plantarseBtn.Text = "ir";
                }
                else if (jugador8NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador8FichasLbl.Text))
                    {
                        apostarNum.Minimum = maxJugado - Convert.ToInt32(jugador8JugadoLbl.Text);
                        apostarBtn.Text = "Subir";
                    }
                    else
                    {
                        apostarNum.Minimum = Convert.ToInt32(jugador8FichasLbl.Text) - Convert.ToInt32(jugador8JugadoLbl.Text);
                        apostarBtn.Text = "Jugarlo todo";
                    }
                    apostarNum.Maximum = Convert.ToInt32(jugador8FichasLbl.Text) - Convert.ToInt32(jugador8JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador8JugadoLbl.Text))
                        plantarseBtn.Text = "Pasar";
                    else
                        plantarseBtn.Text = "ir";
                }

                //Habilitamos los objetos de acciones de la ronda para que el usuario decida qué hacer
                pedirBtn.Enabled = true;
                pedirBtn.Visible = true;
                plantarseBtn.Enabled = true;
                plantarseBtn.Visible = true;
                apostarBtn.Enabled = true;
                apostarBtn.Visible = true;
                apostarNum.Enabled = true;
                apostarNum.Visible = true;
            }
        }

        private void enviar_Btn_Click(object sender, EventArgs e)
        {
            //Evento llamado cuando el usuario pulsa el botón "Enviar". Envía el mensaje que ha escrito en el textBox al servidor.
            string mensaje = "6/" + this.ID + "/" + this.usuario + ": " + chatTextBox.Text;
            server.Enviar(mensaje);
            chatTextBox.Text = "Escribe algo";
        }

        private void empezarBtn_Click(object sender, EventArgs e)
        {
            //Evento llamado cuando el host quiere comenzar a jugar la partida.
            /*empezarBtn.Enabled = false;
            empezarBtn.Visible = false;
            fichasLbl.Visible = false;
            fichasNum.Visible = false;
            ciegaLbl.Visible = false;
            ciegaNum.Visible = false;
            string mensaje = "9/" + this.ID;
            server.Enviar(mensaje);*/
        }

        private void NuevaPartida_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Evento llamado cuando el formulario se va a cerrar. Envía mensajes de desconexión al servidor para que tenga constancia.
            string mensaje = "6/" + this.ID + "/" + this.usuario + " se ha ido de la partida.";
            server.Enviar(mensaje);
            mensaje = "7/" + this.ID;
            server.Enviar(mensaje);
            this.ID = -1;
        }

        private void chatTextBox_Click(object sender, EventArgs e)
        {
            //Evento llamado cuando se pulsa en el textBox del chat. Elimina el contenido predeterminado del textBox.
            if(chatTextBox.Text == "Escribe algo")
                chatTextBox.Text = "";
        }

        private void fichasNum_ValueChanged(object sender, EventArgs e)
        {
            //Evento llamado cuando el usuario cambia el valor de fichas iniciales. Envía los parametros de esta partida configurados.
            /*string mensaje = "8/" + this.ID + "/0/" + fichasNum.Value + "/" + ciegaNum.Value;
            server.Enviar(mensaje);*/
        }

        private void ciegaNum_ValueChanged(object sender, EventArgs e)
        {
            //Evento llamado cuando el usuario cambia el valor de ciega inicial. Envía los parametros de esta partida configurados.
            /*string mensaje = "8/" + this.ID + "/0/" + fichasNum.Value + "/" + ciegaNum.Value;
            server.Enviar(mensaje);*/
        }

        private void noIrBtn_Click(object sender, EventArgs e)
        {
            //En su turno, el jugador decide dejar las cartas y no hacer la apuesta que se le exige. 
            //Se envia la decisión al servidor y bloqueamos los objetos relacionados con acciones en la ronda
            string mensaje = "10/" + this.ID + "/-1";
            server.Enviar(mensaje);
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarNum.Enabled = false;
            apostarNum.Visible = false;
        }

        private void irBtn_Click(object sender, EventArgs e)
        {
            //En su turno, el jugador decide hacer la apuesta que se le exige. 
            //Se envia la decisión al servidor y bloqueamos los objetos relacionados con acciones en la ronda
            string mensaje = "10/" + this.ID + "/" + apostarNum.Minimum;
            server.Enviar(mensaje);
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarNum.Enabled = false;
            apostarNum.Visible = false;
        }

        private void subirBtn_Click(object sender, EventArgs e)
        {
            //En su turno, el jugador decide subir la puja con la cantidad indicada en el numericUpDown. 
            //Se envia la decisión al servidor y bloqueamos los objetos relacionados con acciones en la ronda
            string mensaje = "10/" + this.ID + "/" + apostarNum.Value;
            server.Enviar(mensaje);
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarNum.Enabled = false;
            apostarNum.Visible = false;
        }
    }
}
