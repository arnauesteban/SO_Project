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
            fichasNum.Enabled = true;
            ciegaNum.Enabled = true;
            string mensaje = "8/" + this.ID + "/0/" + fichasNum.Value + "/" + ciegaNum.Value;
            server.Enviar(mensaje);
            noIrBtn.Enabled = false;
            noIrBtn.Visible = false;
            irBtn.Enabled = false;
            irBtn.Visible = false;
            subirBtn.Enabled = false;
            subirBtn.Visible = false;
            subirNum.Enabled = false;
            subirNum.Visible = false;

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
            fichasNum.Enabled = false;
            ciegaNum.Enabled = false;
            string mensaje = "8/" + this.ID + "/2";
            server.Enviar(mensaje);
            noIrBtn.Enabled = false;
            noIrBtn.Visible = false;
            irBtn.Enabled = false;
            irBtn.Visible = false;
            subirBtn.Enabled = false;
            subirBtn.Visible = false;
            subirNum.Enabled = false;
            subirNum.Visible = false;
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
            chatLbl.Text = chatLbl.Text + Environment.NewLine + mensaje;
        }

        public void Accion7(string mensaje)
        {
            empezarBtn.Enabled = true;
            fichasNum.Enabled = true;
            ciegaNum.Enabled = true;
        }

        public void Accion8(string mensaje)
        {
            string[] separado = mensaje.Split('/');
            if (Convert.ToInt32(separado[0]) != 1)
            {
                fichasNum.Value = Convert.ToInt32(separado[1]);
                ciegaNum.Value = Convert.ToInt32(separado[2]);
            }
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
                    jugador8JugadoLbl.Text = "";*/
                }
            }
        }

        public void Accion9(string mensaje)
        {
            empezarBtn.Visible = false;
            fichasNum.Visible = false;
            fichasLbl.Visible = false;
            ciegaLbl.Visible = false;
            ciegaNum.Visible = false;
            string[] separado = mensaje.Split('/');
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
            else if (Convert.ToInt32(separado[0]) == 1)
            {
                carta1Lbl.Text = separado[1];
                carta2Lbl.Text = separado[2];
                carta3Lbl.Text = separado[3];
            }
            else if (Convert.ToInt32(separado[0]) == 2)
            {
                carta4Lbl.Text = separado[1];
            }
            else if (Convert.ToInt32(separado[0]) == 3)
            {
                carta5Lbl.Text = separado[1];
            }

        }

        public void Accion10(string mensaje)
        {
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
            if (mensaje == this.usuario)
            {
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

                if (jugador1NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador1FichasLbl.Text))
                    {
                        subirNum.Minimum = maxJugado - Convert.ToInt32(jugador1JugadoLbl.Text);
                        subirBtn.Text = "Subir";
                    }
                    else
                    {
                        subirNum.Minimum = Convert.ToInt32(jugador1FichasLbl.Text) - Convert.ToInt32(jugador1JugadoLbl.Text);
                        subirBtn.Text = "Jugarlo todo";
                    }
                    subirNum.Maximum = Convert.ToInt32(jugador1FichasLbl.Text) - Convert.ToInt32(jugador1JugadoLbl.Text);
                    if(maxJugado == Convert.ToInt32(jugador1JugadoLbl.Text))
                        irBtn.Text = "Pasar";
                    else
                        irBtn.Text = "ir";

                }
                else if (jugador2NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador2FichasLbl.Text))
                    {
                        subirNum.Minimum = maxJugado - Convert.ToInt32(jugador2JugadoLbl.Text);
                        subirBtn.Text = "Subir";
                    }
                    else
                    {
                        subirNum.Minimum = Convert.ToInt32(jugador2FichasLbl.Text) - Convert.ToInt32(jugador2JugadoLbl.Text);
                        subirBtn.Text = "Jugarlo todo";
                    }
                    subirNum.Maximum = Convert.ToInt32(jugador2FichasLbl.Text) - Convert.ToInt32(jugador2JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador2JugadoLbl.Text))
                        irBtn.Text = "Pasar";
                    else
                        irBtn.Text = "ir";
                }
                else if (jugador3NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador3FichasLbl.Text))
                    {
                        subirNum.Minimum = maxJugado - Convert.ToInt32(jugador3JugadoLbl.Text);
                        subirBtn.Text = "Subir";
                    }
                    else
                    {
                        subirNum.Minimum = Convert.ToInt32(jugador3FichasLbl.Text) - Convert.ToInt32(jugador3JugadoLbl.Text);
                        subirBtn.Text = "Jugarlo todo";
                    }
                    subirNum.Maximum = Convert.ToInt32(jugador3FichasLbl.Text) - Convert.ToInt32(jugador3JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador3JugadoLbl.Text))
                        irBtn.Text = "Pasar";
                    else
                        irBtn.Text = "ir";
                }
                else if (jugador4NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador4FichasLbl.Text))
                    {
                        subirNum.Minimum = maxJugado - Convert.ToInt32(jugador4JugadoLbl.Text);
                        subirBtn.Text = "Subir";
                    }
                    else
                    {
                        subirNum.Minimum = Convert.ToInt32(jugador4FichasLbl.Text) - Convert.ToInt32(jugador4JugadoLbl.Text);
                        subirBtn.Text = "Jugarlo todo";
                    }
                    subirNum.Maximum = Convert.ToInt32(jugador4FichasLbl.Text) - Convert.ToInt32(jugador4JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador4JugadoLbl.Text))
                        irBtn.Text = "Pasar";
                    else
                        irBtn.Text = "ir";
                }
                else if (jugador5NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador5FichasLbl.Text))
                    {
                        subirNum.Minimum = maxJugado - Convert.ToInt32(jugador5JugadoLbl.Text);
                        subirBtn.Text = "Subir";
                    }
                    else
                    {
                        subirNum.Minimum = Convert.ToInt32(jugador5FichasLbl.Text) - Convert.ToInt32(jugador5JugadoLbl.Text);
                        subirBtn.Text = "Jugarlo todo";
                    }
                    subirNum.Maximum = Convert.ToInt32(jugador5FichasLbl.Text) - Convert.ToInt32(jugador5JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador5JugadoLbl.Text))
                        irBtn.Text = "Pasar";
                    else
                        irBtn.Text = "ir";
                }
                else if (jugador6NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador6FichasLbl.Text))
                    {
                        subirNum.Minimum = maxJugado - Convert.ToInt32(jugador6JugadoLbl.Text);
                        subirBtn.Text = "Subir";
                    }
                    else
                    {
                        subirNum.Minimum = Convert.ToInt32(jugador6FichasLbl.Text) - Convert.ToInt32(jugador6JugadoLbl.Text);
                        subirBtn.Text = "Jugarlo todo";
                    }
                    subirNum.Maximum = Convert.ToInt32(jugador6FichasLbl.Text) - Convert.ToInt32(jugador6JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador6JugadoLbl.Text))
                        irBtn.Text = "Pasar";
                    else
                        irBtn.Text = "ir";
                }
                else if (jugador7NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador7FichasLbl.Text))
                    {
                        subirNum.Minimum = maxJugado - Convert.ToInt32(jugador7JugadoLbl.Text);
                        subirBtn.Text = "Subir";
                    }
                    else
                    {
                        subirNum.Minimum = Convert.ToInt32(jugador7FichasLbl.Text) - Convert.ToInt32(jugador7JugadoLbl.Text);
                        subirBtn.Text = "Jugarlo todo";
                    }
                    subirNum.Maximum = Convert.ToInt32(jugador7FichasLbl.Text) - Convert.ToInt32(jugador7JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador7JugadoLbl.Text))
                        irBtn.Text = "Pasar";
                    else
                        irBtn.Text = "ir";
                }
                else if (jugador8NombreLbl.Text == this.usuario)
                {
                    if (maxJugado < Convert.ToInt32(jugador8FichasLbl.Text))
                    {
                        subirNum.Minimum = maxJugado - Convert.ToInt32(jugador8JugadoLbl.Text);
                        subirBtn.Text = "Subir";
                    }
                    else
                    {
                        subirNum.Minimum = Convert.ToInt32(jugador8FichasLbl.Text) - Convert.ToInt32(jugador8JugadoLbl.Text);
                        subirBtn.Text = "Jugarlo todo";
                    }
                    subirNum.Maximum = Convert.ToInt32(jugador8FichasLbl.Text) - Convert.ToInt32(jugador8JugadoLbl.Text);
                    if (maxJugado == Convert.ToInt32(jugador8JugadoLbl.Text))
                        irBtn.Text = "Pasar";
                    else
                        irBtn.Text = "ir";
                }

                noIrBtn.Enabled = true;
                noIrBtn.Visible = true;
                irBtn.Enabled = true;
                irBtn.Visible = true;
                subirBtn.Enabled = true;
                subirBtn.Visible = true;
                subirNum.Enabled = true;
                subirNum.Visible = true;
            }
        }

        private void enviar_Btn_Click(object sender, EventArgs e)
        {
            //Evento llamado cuando el usuario pulsa el botón "Enviar". Envía el mensaje que ha escrito en el textBox al servidor.
            string mensaje = "6/" + this.ID + "/" + this.usuario + ": " + chatTextBox.Text;
            server.Enviar(mensaje);
            chatTextBox.Text = "";
        }

        private void empezarBtn_Click(object sender, EventArgs e)
        {
            //Evento llamado cuando el hsot quiere comenzar a jugar la partida.
            //MessageBox.Show("Juego no implementado.");
            empezarBtn.Enabled = false;
            empezarBtn.Visible = false;
            fichasLbl.Visible = false;
            fichasNum.Visible = false;
            ciegaLbl.Visible = false;
            ciegaNum.Visible = false;
            string mensaje = "9/" + this.ID;
            server.Enviar(mensaje);
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
            string mensaje = "8/" + this.ID + "/0/" + fichasNum.Value + "/" + ciegaNum.Value;
            server.Enviar(mensaje);
        }

        private void ciegaNum_ValueChanged(object sender, EventArgs e)
        {
            //Evento llamado cuando el usuario cambia el valor de ciega inicial. Envía los parametros de esta partida configurados.
            string mensaje = "8/" + this.ID + "/0/" + fichasNum.Value + "/" + ciegaNum.Value;
            server.Enviar(mensaje);
        }

        private void noIrBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "10/" + this.ID + "/-1";
            server.Enviar(mensaje);
            noIrBtn.Enabled = false;
            noIrBtn.Visible = false;
            irBtn.Enabled = false;
            irBtn.Visible = false;
            subirBtn.Enabled = false;
            subirBtn.Visible = false;
            subirNum.Enabled = false;
            subirNum.Visible = false;
        }

        private void irBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "10/" + this.ID + "/" + subirNum.Minimum;
            server.Enviar(mensaje);
            noIrBtn.Enabled = false;
            noIrBtn.Visible = false;
            irBtn.Enabled = false;
            irBtn.Visible = false;
            subirBtn.Enabled = false;
            subirBtn.Visible = false;
            subirNum.Enabled = false;
            subirNum.Visible = false;
        }

        private void subirBtn_Click(object sender, EventArgs e)
        {
            string mensaje = "10/" + this.ID + "/" + subirNum.Value;
            server.Enviar(mensaje);
            noIrBtn.Enabled = false;
            noIrBtn.Visible = false;
            irBtn.Enabled = false;
            irBtn.Visible = false;
            subirBtn.Enabled = false;
            subirBtn.Visible = false;
            subirNum.Enabled = false;
            subirNum.Visible = false;
        }
    }
}
