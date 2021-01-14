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
        int lineasChat;
        bool banca;
        int numJugadores;

        public delegate void DelegadoRespuesta(string mensaje);

        public NuevaPartida(Server server, string usuario)
        {
            //Constructor del formulario usado cuando el usuario es el host de la partida.
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.server = server;
            this.usuario = usuario;
            this.numJugadores = 1;
            usuarioLbl.Text = this.usuario;
            chatLbl.Text = "¡Bienvenido a una nueva partida de poker! Espera a que tus contrincantes acepten tu invitación o pulsa el botón del centro de la mesa.";
            lineasChat = 4;
            
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarNum.Enabled = false;
            apostarNum.Visible = false;
            rendirseBtn.Enabled = false;
            rendirseBtn.Visible = false;

            jugador2NombreLbl.Text = "";
            jugador2Carta1Lbl.Text = "";
            jugador2Carta2Lbl.Text = "";
            jugador2Carta3Lbl.Text = "";
            jugador2Carta4Lbl.Text = "";
            jugador2PuntosLbl.Text = "";
            jugador2FichasLbl.Text = "";
            jugador2JugadoLbl.Text = "";
            jugador3NombreLbl.Text = "";
            jugador3Carta1Lbl.Text = "";
            jugador3Carta2Lbl.Text = "";
            jugador3Carta3Lbl.Text = "";
            jugador3Carta4Lbl.Text = "";
            jugador3PuntosLbl.Text = "";
            jugador3FichasLbl.Text = "";
            jugador3JugadoLbl.Text = "";
            jugador4NombreLbl.Text = "";
            jugador4Carta1Lbl.Text = "";
            jugador4Carta2Lbl.Text = "";
            jugador4Carta3Lbl.Text = "";
            jugador4Carta4Lbl.Text = "";
            jugador4PuntosLbl.Text = "";
            jugador4FichasLbl.Text = "";
            jugador4JugadoLbl.Text = "";
            jugador5NombreLbl.Text = "";
            jugador5Carta1Lbl.Text = "";
            jugador5Carta2Lbl.Text = "";
            jugador5Carta3Lbl.Text = "";
            jugador5Carta4Lbl.Text = "";
            jugador5PuntosLbl.Text = "";
            jugador5FichasLbl.Text = "";
            jugador5JugadoLbl.Text = "";
            jugador6NombreLbl.Text = "";
            jugador6Carta1Lbl.Text = "";
            jugador6Carta2Lbl.Text = "";
            jugador6Carta3Lbl.Text = "";
            jugador6Carta4Lbl.Text = "";
            jugador6PuntosLbl.Text = "";
            jugador6FichasLbl.Text = "";
            jugador6JugadoLbl.Text = "";
            jugador7NombreLbl.Text = "";
            jugador7Carta1Lbl.Text = "";
            jugador7Carta2Lbl.Text = "";
            jugador7Carta3Lbl.Text = "";
            jugador7Carta4Lbl.Text = "";
            jugador7PuntosLbl.Text = "";
            jugador7FichasLbl.Text = "";
            jugador7JugadoLbl.Text = "";
            jugador8NombreLbl.Text = "";
            jugador8Carta1Lbl.Text = "";
            jugador8Carta2Lbl.Text = "";
            jugador8Carta3Lbl.Text = "";
            jugador8Carta4Lbl.Text = "";
            jugador8PuntosLbl.Text = "";
            jugador8FichasLbl.Text = "";
            jugador8JugadoLbl.Text = "";
        }

        public NuevaPartida(Server server, string usuario, int ID)
        {
            //Constructor del formulario usado cuando el usuario acepta una invitación de otro jugador.
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;
            this.server = server;
            this.usuario = usuario;
            this.ID = ID;
            this.numJugadores = 1;
            usuarioLbl.Text = this.usuario;
            empezarBtn.Enabled = false;
            this.Text = "Partida " + this.ID;
            chatLbl.Text = "¡Bienvenido a una nueva partida de poker! Espera a que otros contrincantes acepten la invitación o a que el host pulse el botón del centro de la mesa.";
            lineasChat = 4;
            string mensaje = "12/" + this.ID;
            server.Enviar(mensaje);
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarNum.Enabled = false;
            apostarNum.Visible = false;
            rendirseBtn.Enabled = false;
            rendirseBtn.Visible = false;
        }

        public int getID()
        {
            //Método que devuelve el identificador de partida.
            return this.ID;
        }

        public void resetLabels()
        {
            if (numJugadores > 0)
            {
                jugador1Carta1Lbl.Text = "Carta 1";
                jugador1Carta2Lbl.Text = "Carta 2";
                jugador1Carta3Lbl.Text = "Carta 3";
                jugador1Carta4Lbl.Text = "Carta 4";
                jugador1JugadoLbl.Text = "0";
                jugador1PuntosLbl.Text = "0";
            }
            if (numJugadores > 1)
            {
                jugador2Carta1Lbl.Text = "Carta 1";
                jugador2Carta2Lbl.Text = "Carta 2";
                jugador2Carta3Lbl.Text = "Carta 3";
                jugador2Carta4Lbl.Text = "Carta 4";
                jugador2JugadoLbl.Text = "0";
                jugador2PuntosLbl.Text = "0";
            }
            if (numJugadores > 2)
            {
                jugador3Carta1Lbl.Text = "Carta 1";
                jugador3Carta2Lbl.Text = "Carta 2";
                jugador3Carta3Lbl.Text = "Carta 3";
                jugador3Carta4Lbl.Text = "Carta 4";
                jugador3JugadoLbl.Text = "0";
                jugador3PuntosLbl.Text = "0";
            }
            if (numJugadores > 3)
            {
                jugador4Carta1Lbl.Text = "Carta 1";
                jugador4Carta2Lbl.Text = "Carta 2";
                jugador4Carta3Lbl.Text = "Carta 3";
                jugador4Carta4Lbl.Text = "Carta 4";
                jugador4JugadoLbl.Text = "0";
                jugador4PuntosLbl.Text = "0";
            }
            if (numJugadores > 4)
            {
                jugador5Carta1Lbl.Text = "Carta 1";
                jugador5Carta2Lbl.Text = "Carta 2";
                jugador5Carta3Lbl.Text = "Carta 3";
                jugador5Carta4Lbl.Text = "Carta 4";
                jugador5JugadoLbl.Text = "0";
                jugador5PuntosLbl.Text = "0";
            }
            if (numJugadores > 5)
            {
                jugador6Carta1Lbl.Text = "Carta 1";
                jugador6Carta2Lbl.Text = "Carta 2";
                jugador6Carta3Lbl.Text = "Carta 3";
                jugador6Carta4Lbl.Text = "Carta 4";
                jugador6JugadoLbl.Text = "0";
                jugador6PuntosLbl.Text = "0";
            }
            if (numJugadores > 6)
            {
                jugador7Carta1Lbl.Text = "Carta 1";
                jugador7Carta2Lbl.Text = "Carta 2";
                jugador7Carta3Lbl.Text = "Carta 3";
                jugador7Carta4Lbl.Text = "Carta 4";
                jugador7JugadoLbl.Text = "0";
                jugador7PuntosLbl.Text = "0";
            }
            if (numJugadores > 7)
            {
                jugador8Carta1Lbl.Text = "Carta 1";
                jugador8Carta2Lbl.Text = "Carta 2";
                jugador8Carta3Lbl.Text = "Carta 3";
                jugador8Carta4Lbl.Text = "Carta 4";
                jugador8JugadoLbl.Text = "0";
                jugador8PuntosLbl.Text = "0";
            }
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

                case 12:
                    //delegado = new DelegadoRespuesta(Accion12);
                    //this.Invoke(delegado, new object[] { mensaje });
                    Accion12(mensaje);
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
            //Función que es llamada cuando se recibe un mensaje con una advertencia de que un jugador se ha ido de la partida
            //Actualiza los label del formulario de acuerdo con la nueva distribucion de jugadores que hay actualmente
            int n = Convert.ToInt32(mensaje);
            if (n == 0)
            {
                jugador1NombreLbl.Text = jugador2NombreLbl.Text;
                jugador1JugadoLbl.Text = jugador2JugadoLbl.Text;
                jugador1Carta1Lbl.Text = jugador2Carta1Lbl.Text;
                jugador1Carta2Lbl.Text = jugador2Carta2Lbl.Text;
                jugador1Carta3Lbl.Text = jugador2Carta3Lbl.Text;
                jugador1Carta4Lbl.Text = jugador2Carta4Lbl.Text;
                jugador1FichasLbl.Text = jugador2FichasLbl.Text;
                jugador1PuntosLbl.Text = jugador2PuntosLbl.Text;
                jugador2NombreLbl.Text = jugador3NombreLbl.Text;
                jugador2JugadoLbl.Text = jugador3JugadoLbl.Text;
                jugador2Carta1Lbl.Text = jugador3Carta1Lbl.Text;
                jugador2Carta2Lbl.Text = jugador3Carta2Lbl.Text;
                jugador2Carta3Lbl.Text = jugador3Carta3Lbl.Text;
                jugador2Carta4Lbl.Text = jugador3Carta4Lbl.Text;
                jugador2FichasLbl.Text = jugador3FichasLbl.Text;
                jugador2PuntosLbl.Text = jugador3PuntosLbl.Text;
                jugador3NombreLbl.Text = jugador4NombreLbl.Text;
                jugador3JugadoLbl.Text = jugador4JugadoLbl.Text;
                jugador3Carta1Lbl.Text = jugador4Carta1Lbl.Text;
                jugador3Carta2Lbl.Text = jugador4Carta2Lbl.Text;
                jugador3Carta3Lbl.Text = jugador4Carta3Lbl.Text;
                jugador3Carta4Lbl.Text = jugador4Carta4Lbl.Text;
                jugador3FichasLbl.Text = jugador4FichasLbl.Text;
                jugador3PuntosLbl.Text = jugador4PuntosLbl.Text;
                jugador4NombreLbl.Text = jugador5NombreLbl.Text;
                jugador4JugadoLbl.Text = jugador5JugadoLbl.Text;
                jugador4Carta1Lbl.Text = jugador5Carta1Lbl.Text;
                jugador4Carta2Lbl.Text = jugador5Carta2Lbl.Text;
                jugador4Carta3Lbl.Text = jugador5Carta3Lbl.Text;
                jugador4Carta4Lbl.Text = jugador5Carta4Lbl.Text;
                jugador4FichasLbl.Text = jugador5FichasLbl.Text;
                jugador4PuntosLbl.Text = jugador5PuntosLbl.Text;
                jugador5NombreLbl.Text = jugador6NombreLbl.Text;
                jugador5JugadoLbl.Text = jugador6JugadoLbl.Text;
                jugador5Carta1Lbl.Text = jugador6Carta1Lbl.Text;
                jugador5Carta2Lbl.Text = jugador6Carta2Lbl.Text;
                jugador5Carta3Lbl.Text = jugador6Carta3Lbl.Text;
                jugador5Carta4Lbl.Text = jugador6Carta4Lbl.Text;
                jugador5FichasLbl.Text = jugador6FichasLbl.Text;
                jugador5PuntosLbl.Text = jugador6PuntosLbl.Text;
                jugador6NombreLbl.Text = jugador7NombreLbl.Text;
                jugador6JugadoLbl.Text = jugador7JugadoLbl.Text;
                jugador6Carta1Lbl.Text = jugador7Carta1Lbl.Text;
                jugador6Carta2Lbl.Text = jugador7Carta2Lbl.Text;
                jugador6Carta3Lbl.Text = jugador7Carta3Lbl.Text;
                jugador6Carta4Lbl.Text = jugador7Carta4Lbl.Text;
                jugador6FichasLbl.Text = jugador7FichasLbl.Text;
                jugador6PuntosLbl.Text = jugador7PuntosLbl.Text;
                jugador7NombreLbl.Text = jugador8NombreLbl.Text;
                jugador7JugadoLbl.Text = jugador8JugadoLbl.Text;
                jugador7Carta1Lbl.Text = jugador8Carta1Lbl.Text;
                jugador7Carta2Lbl.Text = jugador8Carta2Lbl.Text;
                jugador7Carta3Lbl.Text = jugador8Carta3Lbl.Text;
                jugador7Carta4Lbl.Text = jugador8Carta4Lbl.Text;
                jugador7FichasLbl.Text = jugador8FichasLbl.Text;
                jugador7PuntosLbl.Text = jugador8PuntosLbl.Text;
                jugador8NombreLbl.Text = "";
                jugador8JugadoLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8PuntosLbl.Text = "";
            }
            else if (n == 1)
            {
                jugador2NombreLbl.Text = jugador3NombreLbl.Text;
                jugador2JugadoLbl.Text = jugador3JugadoLbl.Text;
                jugador2Carta1Lbl.Text = jugador3Carta1Lbl.Text;
                jugador2Carta2Lbl.Text = jugador3Carta2Lbl.Text;
                jugador2Carta3Lbl.Text = jugador3Carta3Lbl.Text;
                jugador2Carta4Lbl.Text = jugador3Carta4Lbl.Text;
                jugador2FichasLbl.Text = jugador3FichasLbl.Text;
                jugador2PuntosLbl.Text = jugador3PuntosLbl.Text;
                jugador3NombreLbl.Text = jugador4NombreLbl.Text;
                jugador3JugadoLbl.Text = jugador4JugadoLbl.Text;
                jugador3Carta1Lbl.Text = jugador4Carta1Lbl.Text;
                jugador3Carta2Lbl.Text = jugador4Carta2Lbl.Text;
                jugador3Carta3Lbl.Text = jugador4Carta3Lbl.Text;
                jugador3Carta4Lbl.Text = jugador4Carta4Lbl.Text;
                jugador3FichasLbl.Text = jugador4FichasLbl.Text;
                jugador3PuntosLbl.Text = jugador4PuntosLbl.Text;
                jugador4NombreLbl.Text = jugador5NombreLbl.Text;
                jugador4JugadoLbl.Text = jugador5JugadoLbl.Text;
                jugador4Carta1Lbl.Text = jugador5Carta1Lbl.Text;
                jugador4Carta2Lbl.Text = jugador5Carta2Lbl.Text;
                jugador4Carta3Lbl.Text = jugador5Carta3Lbl.Text;
                jugador4Carta4Lbl.Text = jugador5Carta4Lbl.Text;
                jugador4FichasLbl.Text = jugador5FichasLbl.Text;
                jugador4PuntosLbl.Text = jugador5PuntosLbl.Text;
                jugador5NombreLbl.Text = jugador6NombreLbl.Text;
                jugador5JugadoLbl.Text = jugador6JugadoLbl.Text;
                jugador5Carta1Lbl.Text = jugador6Carta1Lbl.Text;
                jugador5Carta2Lbl.Text = jugador6Carta2Lbl.Text;
                jugador5Carta3Lbl.Text = jugador6Carta3Lbl.Text;
                jugador5Carta4Lbl.Text = jugador6Carta4Lbl.Text;
                jugador5FichasLbl.Text = jugador6FichasLbl.Text;
                jugador5PuntosLbl.Text = jugador6PuntosLbl.Text;
                jugador6NombreLbl.Text = jugador7NombreLbl.Text;
                jugador6JugadoLbl.Text = jugador7JugadoLbl.Text;
                jugador6Carta1Lbl.Text = jugador7Carta1Lbl.Text;
                jugador6Carta2Lbl.Text = jugador7Carta2Lbl.Text;
                jugador6Carta3Lbl.Text = jugador7Carta3Lbl.Text;
                jugador6Carta4Lbl.Text = jugador7Carta4Lbl.Text;
                jugador6FichasLbl.Text = jugador7FichasLbl.Text;
                jugador6PuntosLbl.Text = jugador7PuntosLbl.Text;
                jugador7NombreLbl.Text = jugador8NombreLbl.Text;
                jugador7JugadoLbl.Text = jugador8JugadoLbl.Text;
                jugador7Carta1Lbl.Text = jugador8Carta1Lbl.Text;
                jugador7Carta2Lbl.Text = jugador8Carta2Lbl.Text;
                jugador7Carta3Lbl.Text = jugador8Carta3Lbl.Text;
                jugador7Carta4Lbl.Text = jugador8Carta4Lbl.Text;
                jugador7FichasLbl.Text = jugador8FichasLbl.Text;
                jugador7PuntosLbl.Text = jugador8PuntosLbl.Text;
                jugador8NombreLbl.Text = "";
                jugador8JugadoLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8PuntosLbl.Text = "";
            }
            else if (n == 2)
            {
                jugador3NombreLbl.Text = jugador4NombreLbl.Text;
                jugador3JugadoLbl.Text = jugador4JugadoLbl.Text;
                jugador3Carta1Lbl.Text = jugador4Carta1Lbl.Text;
                jugador3Carta2Lbl.Text = jugador4Carta2Lbl.Text;
                jugador3Carta3Lbl.Text = jugador4Carta3Lbl.Text;
                jugador3Carta4Lbl.Text = jugador4Carta4Lbl.Text;
                jugador3FichasLbl.Text = jugador4FichasLbl.Text;
                jugador3PuntosLbl.Text = jugador4PuntosLbl.Text;
                jugador4NombreLbl.Text = jugador5NombreLbl.Text;
                jugador4JugadoLbl.Text = jugador5JugadoLbl.Text;
                jugador4Carta1Lbl.Text = jugador5Carta1Lbl.Text;
                jugador4Carta2Lbl.Text = jugador5Carta2Lbl.Text;
                jugador4Carta3Lbl.Text = jugador5Carta3Lbl.Text;
                jugador4Carta4Lbl.Text = jugador5Carta4Lbl.Text;
                jugador4FichasLbl.Text = jugador5FichasLbl.Text;
                jugador4PuntosLbl.Text = jugador5PuntosLbl.Text;
                jugador5NombreLbl.Text = jugador6NombreLbl.Text;
                jugador5JugadoLbl.Text = jugador6JugadoLbl.Text;
                jugador5Carta1Lbl.Text = jugador6Carta1Lbl.Text;
                jugador5Carta2Lbl.Text = jugador6Carta2Lbl.Text;
                jugador5Carta3Lbl.Text = jugador6Carta3Lbl.Text;
                jugador5Carta4Lbl.Text = jugador6Carta4Lbl.Text;
                jugador5FichasLbl.Text = jugador6FichasLbl.Text;
                jugador5PuntosLbl.Text = jugador6PuntosLbl.Text;
                jugador6NombreLbl.Text = jugador7NombreLbl.Text;
                jugador6JugadoLbl.Text = jugador7JugadoLbl.Text;
                jugador6Carta1Lbl.Text = jugador7Carta1Lbl.Text;
                jugador6Carta2Lbl.Text = jugador7Carta2Lbl.Text;
                jugador6Carta3Lbl.Text = jugador7Carta3Lbl.Text;
                jugador6Carta4Lbl.Text = jugador7Carta4Lbl.Text;
                jugador6FichasLbl.Text = jugador7FichasLbl.Text;
                jugador6PuntosLbl.Text = jugador7PuntosLbl.Text;
                jugador7NombreLbl.Text = jugador8NombreLbl.Text;
                jugador7JugadoLbl.Text = jugador8JugadoLbl.Text;
                jugador7Carta1Lbl.Text = jugador8Carta1Lbl.Text;
                jugador7Carta2Lbl.Text = jugador8Carta2Lbl.Text;
                jugador7Carta3Lbl.Text = jugador8Carta3Lbl.Text;
                jugador7Carta4Lbl.Text = jugador8Carta4Lbl.Text;
                jugador7FichasLbl.Text = jugador8FichasLbl.Text;
                jugador7PuntosLbl.Text = jugador8PuntosLbl.Text;
                jugador8NombreLbl.Text = "";
                jugador8JugadoLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8PuntosLbl.Text = "";
            }
            else if (n == 3)
            {
                jugador4NombreLbl.Text = jugador5NombreLbl.Text;
                jugador4JugadoLbl.Text = jugador5JugadoLbl.Text;
                jugador4Carta1Lbl.Text = jugador5Carta1Lbl.Text;
                jugador4Carta2Lbl.Text = jugador5Carta2Lbl.Text;
                jugador4Carta3Lbl.Text = jugador5Carta3Lbl.Text;
                jugador4Carta4Lbl.Text = jugador5Carta4Lbl.Text;
                jugador4FichasLbl.Text = jugador5FichasLbl.Text;
                jugador4PuntosLbl.Text = jugador5PuntosLbl.Text;
                jugador5NombreLbl.Text = jugador6NombreLbl.Text;
                jugador5JugadoLbl.Text = jugador6JugadoLbl.Text;
                jugador5Carta1Lbl.Text = jugador6Carta1Lbl.Text;
                jugador5Carta2Lbl.Text = jugador6Carta2Lbl.Text;
                jugador5Carta3Lbl.Text = jugador6Carta3Lbl.Text;
                jugador5Carta4Lbl.Text = jugador6Carta4Lbl.Text;
                jugador5FichasLbl.Text = jugador6FichasLbl.Text;
                jugador5PuntosLbl.Text = jugador6PuntosLbl.Text;
                jugador6NombreLbl.Text = jugador7NombreLbl.Text;
                jugador6JugadoLbl.Text = jugador7JugadoLbl.Text;
                jugador6Carta1Lbl.Text = jugador7Carta1Lbl.Text;
                jugador6Carta2Lbl.Text = jugador7Carta2Lbl.Text;
                jugador6Carta3Lbl.Text = jugador7Carta3Lbl.Text;
                jugador6Carta4Lbl.Text = jugador7Carta4Lbl.Text;
                jugador6FichasLbl.Text = jugador7FichasLbl.Text;
                jugador6PuntosLbl.Text = jugador7PuntosLbl.Text;
                jugador7NombreLbl.Text = jugador8NombreLbl.Text;
                jugador7JugadoLbl.Text = jugador8JugadoLbl.Text;
                jugador7Carta1Lbl.Text = jugador8Carta1Lbl.Text;
                jugador7Carta2Lbl.Text = jugador8Carta2Lbl.Text;
                jugador7Carta3Lbl.Text = jugador8Carta3Lbl.Text;
                jugador7Carta4Lbl.Text = jugador8Carta4Lbl.Text;
                jugador7FichasLbl.Text = jugador8FichasLbl.Text;
                jugador7PuntosLbl.Text = jugador8PuntosLbl.Text;
                jugador8NombreLbl.Text = "";
                jugador8JugadoLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8PuntosLbl.Text = "";
            }
            else if (n == 4)
            {
                jugador5NombreLbl.Text = jugador6NombreLbl.Text;
                jugador5JugadoLbl.Text = jugador6JugadoLbl.Text;
                jugador5Carta1Lbl.Text = jugador6Carta1Lbl.Text;
                jugador5Carta2Lbl.Text = jugador6Carta2Lbl.Text;
                jugador5Carta3Lbl.Text = jugador6Carta3Lbl.Text;
                jugador5Carta4Lbl.Text = jugador6Carta4Lbl.Text;
                jugador5FichasLbl.Text = jugador6FichasLbl.Text;
                jugador5PuntosLbl.Text = jugador6PuntosLbl.Text;
                jugador6NombreLbl.Text = jugador7NombreLbl.Text;
                jugador6JugadoLbl.Text = jugador7JugadoLbl.Text;
                jugador6Carta1Lbl.Text = jugador7Carta1Lbl.Text;
                jugador6Carta2Lbl.Text = jugador7Carta2Lbl.Text;
                jugador6Carta3Lbl.Text = jugador7Carta3Lbl.Text;
                jugador6Carta4Lbl.Text = jugador7Carta4Lbl.Text;
                jugador6FichasLbl.Text = jugador7FichasLbl.Text;
                jugador6PuntosLbl.Text = jugador7PuntosLbl.Text;
                jugador7NombreLbl.Text = jugador8NombreLbl.Text;
                jugador7JugadoLbl.Text = jugador8JugadoLbl.Text;
                jugador7Carta1Lbl.Text = jugador8Carta1Lbl.Text;
                jugador7Carta2Lbl.Text = jugador8Carta2Lbl.Text;
                jugador7Carta3Lbl.Text = jugador8Carta3Lbl.Text;
                jugador7Carta4Lbl.Text = jugador8Carta4Lbl.Text;
                jugador7FichasLbl.Text = jugador8FichasLbl.Text;
                jugador7PuntosLbl.Text = jugador8PuntosLbl.Text;
                jugador8NombreLbl.Text = "";
                jugador8JugadoLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8PuntosLbl.Text = "";
            }
            else if (n == 5)
            {
                jugador6NombreLbl.Text = jugador7NombreLbl.Text;
                jugador6JugadoLbl.Text = jugador7JugadoLbl.Text;
                jugador6Carta1Lbl.Text = jugador7Carta1Lbl.Text;
                jugador6Carta2Lbl.Text = jugador7Carta2Lbl.Text;
                jugador6Carta3Lbl.Text = jugador7Carta3Lbl.Text;
                jugador6Carta4Lbl.Text = jugador7Carta4Lbl.Text;
                jugador6FichasLbl.Text = jugador7FichasLbl.Text;
                jugador6PuntosLbl.Text = jugador7PuntosLbl.Text;
                jugador7NombreLbl.Text = jugador8NombreLbl.Text;
                jugador7JugadoLbl.Text = jugador8JugadoLbl.Text;
                jugador7Carta1Lbl.Text = jugador8Carta1Lbl.Text;
                jugador7Carta2Lbl.Text = jugador8Carta2Lbl.Text;
                jugador7Carta3Lbl.Text = jugador8Carta3Lbl.Text;
                jugador7Carta4Lbl.Text = jugador8Carta4Lbl.Text;
                jugador7FichasLbl.Text = jugador8FichasLbl.Text;
                jugador7PuntosLbl.Text = jugador8PuntosLbl.Text;
                jugador8NombreLbl.Text = "";
                jugador8JugadoLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8PuntosLbl.Text = "";
            }
            else if (n == 6)
            {
                jugador7NombreLbl.Text = jugador8NombreLbl.Text;
                jugador7JugadoLbl.Text = jugador8JugadoLbl.Text;
                jugador7Carta1Lbl.Text = jugador8Carta1Lbl.Text;
                jugador7Carta2Lbl.Text = jugador8Carta2Lbl.Text;
                jugador7Carta3Lbl.Text = jugador8Carta3Lbl.Text;
                jugador7Carta4Lbl.Text = jugador8Carta4Lbl.Text;
                jugador7FichasLbl.Text = jugador8FichasLbl.Text;
                jugador7PuntosLbl.Text = jugador8PuntosLbl.Text;
                jugador8NombreLbl.Text = "";
                jugador8JugadoLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8PuntosLbl.Text = "";
            }
            else if (n == 7)
            {
                jugador8NombreLbl.Text = "";
                jugador8JugadoLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8PuntosLbl.Text = "";
            }
        }

        public void Accion12(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje para configurar los parámetros de esta partida
            //Guarda los nuevos valores y los muestra al usuario
            string[] separado = mensaje.Split('/');
            int n = Convert.ToInt32(separado[0]);
            this.numJugadores = n;
            if (separado[1] == this.usuario)
                this.banca = true;
            else
                this.banca = false;
            jugador1NombreLbl.Text = separado[1];
            jugador1JugadoLbl.Text = "0";
            jugador1Carta1Lbl.Text = "Carta 1";
            jugador1Carta2Lbl.Text = "Carta 2";
            jugador1Carta3Lbl.Text = "Carta 3";
            jugador1Carta4Lbl.Text = "Carta 4";
            jugador1FichasLbl.Text = "100";
            jugador1PuntosLbl.Text = "0";
            jugador2NombreLbl.Text = separado[2];
            jugador2JugadoLbl.Text = "0";
            jugador2Carta1Lbl.Text = "Carta 1";
            jugador2Carta2Lbl.Text = "Carta 2";
            jugador2Carta3Lbl.Text = "Carta 3";
            jugador2Carta4Lbl.Text = "Carta 4";
            jugador2FichasLbl.Text = "100";
            jugador2PuntosLbl.Text = "0";
            if (n > 2)
            {
                jugador3NombreLbl.Text = separado[3];
                jugador3JugadoLbl.Text = "0";
                jugador3Carta1Lbl.Text = "Carta 1";
                jugador3Carta2Lbl.Text = "Carta 2";
                jugador3Carta3Lbl.Text = "Carta 3";
                jugador3Carta4Lbl.Text = "Carta 4";
                jugador3FichasLbl.Text = "100";
                jugador3PuntosLbl.Text = "0";
                if (n > 3)
                {
                    jugador4NombreLbl.Text = separado[4];
                    jugador4JugadoLbl.Text = "0";
                    jugador4Carta1Lbl.Text = "Carta 1";
                    jugador4Carta2Lbl.Text = "Carta 2";
                    jugador4Carta3Lbl.Text = "Carta 3";
                    jugador4Carta4Lbl.Text = "Carta 4";
                    jugador4FichasLbl.Text = "100";
                    jugador4PuntosLbl.Text = "0";
                    if (n > 4)
                    {
                        jugador5NombreLbl.Text = separado[5];
                        jugador5JugadoLbl.Text = "0";
                        jugador5Carta1Lbl.Text = "Carta 1";
                        jugador5Carta2Lbl.Text = "Carta 2";
                        jugador5Carta3Lbl.Text = "Carta 3";
                        jugador5Carta4Lbl.Text = "Carta 4";
                        jugador5FichasLbl.Text = "100";
                        jugador5PuntosLbl.Text = "0";
                        if (n > 5)
                        {
                            jugador6NombreLbl.Text = separado[6];
                            jugador6JugadoLbl.Text = "0";
                            jugador6Carta1Lbl.Text = "Carta 1";
                            jugador6Carta2Lbl.Text = "Carta 2";
                            jugador6Carta3Lbl.Text = "Carta 3";
                            jugador6Carta4Lbl.Text = "Carta 4";
                            jugador6FichasLbl.Text = "100";
                            jugador6PuntosLbl.Text = "0";
                            if (n > 6)
                            {
                                jugador7NombreLbl.Text = separado[7];
                                jugador7JugadoLbl.Text = "0";
                                jugador7Carta1Lbl.Text = "Carta 1";
                                jugador7Carta2Lbl.Text = "Carta 2";
                                jugador7Carta3Lbl.Text = "Carta 3";
                                jugador7Carta4Lbl.Text = "Carta 4";
                                jugador7FichasLbl.Text = "100";
                                jugador7PuntosLbl.Text = "0";
                                if (n > 7)
                                {
                                    jugador8NombreLbl.Text = separado[8];
                                    jugador8JugadoLbl.Text = "0";
                                    jugador8Carta1Lbl.Text = "Carta 1";
                                    jugador8Carta2Lbl.Text = "Carta 2";
                                    jugador8Carta3Lbl.Text = "Carta 3";
                                    jugador8Carta4Lbl.Text = "Carta 4";
                                    jugador8FichasLbl.Text = "100";
                                    jugador8PuntosLbl.Text = "0";
                                }
                                else
                                {
                                    jugador8NombreLbl.Text = "";
                                    jugador8Carta1Lbl.Text = "";
                                    jugador8Carta2Lbl.Text = "";
                                    jugador8Carta3Lbl.Text = "";
                                    jugador8Carta4Lbl.Text = "";
                                    jugador8PuntosLbl.Text = "";
                                    jugador8FichasLbl.Text = "";
                                    jugador8JugadoLbl.Text = "";
                                }
                            }
                            else
                            {
                                jugador7NombreLbl.Text = "";
                                jugador7Carta1Lbl.Text = "";
                                jugador7Carta2Lbl.Text = "";
                                jugador7Carta3Lbl.Text = "";
                                jugador7Carta4Lbl.Text = "";
                                jugador7PuntosLbl.Text = "";
                                jugador7FichasLbl.Text = "";
                                jugador7JugadoLbl.Text = "";
                                jugador8NombreLbl.Text = "";
                                jugador8Carta1Lbl.Text = "";
                                jugador8Carta2Lbl.Text = ""; 
                                jugador8Carta3Lbl.Text = "";
                                jugador8Carta4Lbl.Text = "";
                                jugador8PuntosLbl.Text = "";
                                jugador8FichasLbl.Text = "";
                                jugador8JugadoLbl.Text = "";
                            }
                        }
                        else
                        {
                            jugador6NombreLbl.Text = "";
                            jugador6Carta1Lbl.Text = "";
                            jugador6Carta2Lbl.Text = "";
                            jugador6Carta3Lbl.Text = "";
                            jugador6Carta4Lbl.Text = "";
                            jugador6PuntosLbl.Text = "";
                            jugador6FichasLbl.Text = "";
                            jugador6JugadoLbl.Text = "";
                            jugador7NombreLbl.Text = "";
                            jugador7Carta1Lbl.Text = "";
                            jugador7Carta2Lbl.Text = "";
                            jugador7Carta3Lbl.Text = "";
                            jugador7Carta4Lbl.Text = "";
                            jugador7PuntosLbl.Text = "";
                            jugador7FichasLbl.Text = "";
                            jugador7JugadoLbl.Text = "";
                            jugador8NombreLbl.Text = "";
                            jugador8Carta1Lbl.Text = "";
                            jugador8Carta2Lbl.Text = "";
                            jugador8Carta3Lbl.Text = "";
                            jugador8Carta4Lbl.Text = "";
                            jugador8PuntosLbl.Text = "";
                            jugador8FichasLbl.Text = "";
                            jugador8JugadoLbl.Text = "";
                        }
                    }
                    else
                    {
                        jugador5NombreLbl.Text = "";
                        jugador5Carta1Lbl.Text = "";
                        jugador5Carta2Lbl.Text = "";
                        jugador5Carta3Lbl.Text = "";
                        jugador5Carta4Lbl.Text = "";
                        jugador5PuntosLbl.Text = "";
                        jugador5FichasLbl.Text = "";
                        jugador5JugadoLbl.Text = "";
                        jugador6NombreLbl.Text = "";
                        jugador6Carta1Lbl.Text = "";
                        jugador6Carta2Lbl.Text = "";
                        jugador6Carta3Lbl.Text = "";
                        jugador6Carta4Lbl.Text = "";
                        jugador6PuntosLbl.Text = "";
                        jugador6FichasLbl.Text = "";
                        jugador6JugadoLbl.Text = "";
                        jugador7NombreLbl.Text = "";
                        jugador7Carta1Lbl.Text = "";
                        jugador7Carta2Lbl.Text = "";
                        jugador7Carta3Lbl.Text = "";
                        jugador7Carta4Lbl.Text = "";
                        jugador7PuntosLbl.Text = "";
                        jugador7FichasLbl.Text = "";
                        jugador7JugadoLbl.Text = "";
                        jugador8NombreLbl.Text = "";
                        jugador8Carta1Lbl.Text = "";
                        jugador8Carta2Lbl.Text = "";
                        jugador8Carta3Lbl.Text = "";
                        jugador8Carta4Lbl.Text = "";
                        jugador8PuntosLbl.Text = "";
                        jugador8FichasLbl.Text = "";
                        jugador8JugadoLbl.Text = "";
                    }
                }
                else
                {
                    jugador4NombreLbl.Text = "";
                    jugador4Carta1Lbl.Text = "";
                    jugador4Carta2Lbl.Text = "";
                    jugador4Carta3Lbl.Text = "";
                    jugador4Carta4Lbl.Text = "";
                    jugador4PuntosLbl.Text = "";
                    jugador4FichasLbl.Text = "";
                    jugador4JugadoLbl.Text = "";
                    jugador5NombreLbl.Text = "";
                    jugador5Carta1Lbl.Text = "";
                    jugador5Carta2Lbl.Text = "";
                    jugador5Carta3Lbl.Text = "";
                    jugador5Carta4Lbl.Text = "";
                    jugador5PuntosLbl.Text = "";
                    jugador5FichasLbl.Text = "";
                    jugador5JugadoLbl.Text = "";
                    jugador6NombreLbl.Text = "";
                    jugador6Carta1Lbl.Text = "";
                    jugador6Carta2Lbl.Text = "";
                    jugador6Carta3Lbl.Text = "";
                    jugador6Carta4Lbl.Text = "";
                    jugador6PuntosLbl.Text = "";
                    jugador6FichasLbl.Text = "";
                    jugador6JugadoLbl.Text = "";
                    jugador7NombreLbl.Text = "";
                    jugador7Carta1Lbl.Text = "";
                    jugador7Carta2Lbl.Text = "";
                    jugador7Carta3Lbl.Text = "";
                    jugador7Carta4Lbl.Text = "";
                    jugador7PuntosLbl.Text = "";
                    jugador7FichasLbl.Text = "";
                    jugador7JugadoLbl.Text = "";
                    jugador8NombreLbl.Text = "";
                    jugador8Carta1Lbl.Text = "";
                    jugador8Carta2Lbl.Text = "";
                    jugador8Carta3Lbl.Text = "";
                    jugador8Carta4Lbl.Text = "";
                    jugador8PuntosLbl.Text = "";
                    jugador8FichasLbl.Text = "";
                    jugador8JugadoLbl.Text = "";
                }
            }
            else
            {
                jugador3NombreLbl.Text = "";
                jugador3Carta1Lbl.Text = "";
                jugador3Carta2Lbl.Text = "";
                jugador3Carta3Lbl.Text = "";
                jugador3Carta4Lbl.Text = "";
                jugador3PuntosLbl.Text = "";
                jugador3FichasLbl.Text = "";
                jugador3JugadoLbl.Text = "";
                jugador4NombreLbl.Text = "";
                jugador4Carta1Lbl.Text = "";
                jugador4Carta2Lbl.Text = "";
                jugador4Carta3Lbl.Text = "";
                jugador4Carta4Lbl.Text = "";
                jugador4PuntosLbl.Text = "";
                jugador4FichasLbl.Text = "";
                jugador4JugadoLbl.Text = "";
                jugador5NombreLbl.Text = "";
                jugador5Carta1Lbl.Text = "";
                jugador5Carta2Lbl.Text = "";
                jugador5Carta3Lbl.Text = "";
                jugador5Carta4Lbl.Text = "";
                jugador5PuntosLbl.Text = "";
                jugador5FichasLbl.Text = "";
                jugador5JugadoLbl.Text = "";
                jugador6NombreLbl.Text = "";
                jugador6Carta1Lbl.Text = "";
                jugador6Carta2Lbl.Text = "";
                jugador6Carta3Lbl.Text = "";
                jugador6Carta4Lbl.Text = "";
                jugador6PuntosLbl.Text = "";
                jugador6FichasLbl.Text = "";
                jugador6JugadoLbl.Text = "";
                jugador7NombreLbl.Text = "";
                jugador7Carta1Lbl.Text = "";
                jugador7Carta2Lbl.Text = "";
                jugador7Carta3Lbl.Text = "";
                jugador7Carta4Lbl.Text = "";
                jugador7PuntosLbl.Text = "";
                jugador7FichasLbl.Text = "";
                jugador7JugadoLbl.Text = "";
                jugador8NombreLbl.Text = "";
                jugador8Carta1Lbl.Text = "";
                jugador8Carta2Lbl.Text = "";
                jugador8Carta3Lbl.Text = "";
                jugador8Carta4Lbl.Text = "";
                jugador8PuntosLbl.Text = "";
                jugador8FichasLbl.Text = "";
                jugador8JugadoLbl.Text = "";
            }
        }

        public void Accion13(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje conforme comienza una ronda
            //Quita los objetos de los parámetros iniciales y muestra las cartas que lleguen
            string[] separado = mensaje.Split('/');
            empezarBtn.Visible = false;

            resetLabels();

            //Comienza una nueva ronda y llegan las dos primeras cartas
            if (jugador1NombreLbl.Text == this.usuario)
            {
                jugador1PuntosLbl.Text = separado[0];
                if (Convert.ToInt32(jugador1PuntosLbl.Text) > 20)
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                }
                else
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    
                    if (!banca)
                    {
                        apostarBtn.Enabled = true;
                        apostarBtn.Visible = true;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = true;
                        apostarNum.Visible = true;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                    }
                    else
                    {
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                        pedirBtn.Enabled = true;
                        pedirBtn.Visible = true;
                        plantarseBtn.Enabled = true;
                        plantarseBtn.Visible = true;
                    }
                }
                jugador1Carta1Lbl.Text = separado[1];
                jugador1Carta2Lbl.Text = separado[2];
                apostarNum.Maximum = Convert.ToInt32(jugador1FichasLbl.Text);
            }
            else if (jugador2NombreLbl.Text == this.usuario)
            {
                jugador2PuntosLbl.Text = separado[0]; 
                if (Convert.ToInt32(jugador2PuntosLbl.Text) > 20)
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                }
                else
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;

                    if (!banca)
                    {
                        apostarBtn.Enabled = true;
                        apostarBtn.Visible = true;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = true;
                        apostarNum.Visible = true;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                    }
                    else
                    {
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                        pedirBtn.Enabled = true;
                        pedirBtn.Visible = true;
                        plantarseBtn.Enabled = true;
                        plantarseBtn.Visible = true;
                    }
                }
                jugador2Carta1Lbl.Text = separado[1];
                jugador2Carta2Lbl.Text = separado[2];
                apostarNum.Maximum = Convert.ToInt32(jugador2FichasLbl.Text);
            }
            else if (jugador3NombreLbl.Text == this.usuario)
            {
                jugador3PuntosLbl.Text = separado[0];
                if (Convert.ToInt32(jugador3PuntosLbl.Text) > 20)
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                }
                else
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;

                    if (!banca)
                    {
                        apostarBtn.Enabled = true;
                        apostarBtn.Visible = true;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = true;
                        apostarNum.Visible = true;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                    }
                    else
                    {
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                        pedirBtn.Enabled = true;
                        pedirBtn.Visible = true;
                        plantarseBtn.Enabled = true;
                        plantarseBtn.Visible = true;
                    }
                }
                jugador3Carta1Lbl.Text = separado[1];
                jugador3Carta2Lbl.Text = separado[2];
                apostarNum.Maximum = Convert.ToInt32(jugador3FichasLbl.Text);
            }
            else if (jugador4NombreLbl.Text == this.usuario)
            {
                jugador4PuntosLbl.Text = separado[0];
                if (Convert.ToInt32(jugador4PuntosLbl.Text) > 20)
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                }
                else
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;

                    if (!banca)
                    {
                        apostarBtn.Enabled = true;
                        apostarBtn.Visible = true;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = true;
                        apostarNum.Visible = true;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                    }
                    else
                    {
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                        pedirBtn.Enabled = true;
                        pedirBtn.Visible = true;
                        plantarseBtn.Enabled = true;
                        plantarseBtn.Visible = true;
                    }
                }
                jugador4Carta1Lbl.Text = separado[1];
                jugador4Carta2Lbl.Text = separado[2];
                apostarNum.Maximum = Convert.ToInt32(jugador4FichasLbl.Text);
            }
            else if (jugador5NombreLbl.Text == this.usuario)
            {
                jugador5PuntosLbl.Text = separado[0];
                if (Convert.ToInt32(jugador5PuntosLbl.Text) > 20)
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                }
                else
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;

                    if (!banca)
                    {
                        apostarBtn.Enabled = true;
                        apostarBtn.Visible = true;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = true;
                        apostarNum.Visible = true;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                    }
                    else
                    {
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                        pedirBtn.Enabled = true;
                        pedirBtn.Visible = true;
                        plantarseBtn.Enabled = true;
                        plantarseBtn.Visible = true;
                    }
                }
                jugador5Carta1Lbl.Text = separado[1];
                jugador5Carta2Lbl.Text = separado[2];
                apostarNum.Maximum = Convert.ToInt32(jugador5FichasLbl.Text);
            }
            else if (jugador6NombreLbl.Text == this.usuario)
            {
                jugador6PuntosLbl.Text = separado[0];
                if (Convert.ToInt32(jugador6PuntosLbl.Text) > 20)
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                }
                else
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;

                    if (!banca)
                    {
                        apostarBtn.Enabled = true;
                        apostarBtn.Visible = true;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = true;
                        apostarNum.Visible = true;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                    }
                    else
                    {
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                        pedirBtn.Enabled = true;
                        pedirBtn.Visible = true;
                        plantarseBtn.Enabled = true;
                        plantarseBtn.Visible = true;
                    }
                }
                jugador6Carta1Lbl.Text = separado[1];
                jugador6Carta2Lbl.Text = separado[2];
                apostarNum.Maximum = Convert.ToInt32(jugador6FichasLbl.Text);
            }
            else if (jugador7NombreLbl.Text == this.usuario)
            {
                jugador7PuntosLbl.Text = separado[0];
                if (Convert.ToInt32(jugador7PuntosLbl.Text) > 20)
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                }
                else
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;

                    if (!banca)
                    {
                        apostarBtn.Enabled = true;
                        apostarBtn.Visible = true;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = true;
                        apostarNum.Visible = true;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                    }
                    else
                    {
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                        pedirBtn.Enabled = true;
                        pedirBtn.Visible = true;
                        plantarseBtn.Enabled = true;
                        plantarseBtn.Visible = true;
                    }
                }
                jugador7Carta1Lbl.Text = separado[1];
                jugador7Carta2Lbl.Text = separado[2];
                apostarNum.Maximum = Convert.ToInt32(jugador7FichasLbl.Text);
            }
            else if (jugador8NombreLbl.Text == this.usuario)
            {
                jugador8PuntosLbl.Text = separado[0];
                if (Convert.ToInt32(jugador8PuntosLbl.Text) > 20)
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                }
                else
                {
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;

                    if (!banca)
                    {
                        apostarBtn.Enabled = true;
                        apostarBtn.Visible = true;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = true;
                        apostarNum.Visible = true;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                    }
                    else
                    {
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                        pedirBtn.Enabled = true;
                        pedirBtn.Visible = true;
                        plantarseBtn.Enabled = true;
                        plantarseBtn.Visible = true;
                    }
                }
                jugador8Carta1Lbl.Text = separado[1];
                jugador8Carta2Lbl.Text = separado[2];
                apostarNum.Maximum = Convert.ToInt32(jugador8FichasLbl.Text);
            }
        }

        public void Accion14(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje para mostrar la acción de algún jugador
            //Actualiza las etiquetas del jugador que ha hecho la acción
            string[] separado = mensaje.Split('/');
            int n = Convert.ToInt32(separado[0]);
            int accion = Convert.ToInt32(separado[1]);

            if (accion == 0)
            {
                if (n == 0)
                    jugador1JugadoLbl.Text = separado[2];
                else if (n == 1)
                    jugador2JugadoLbl.Text = separado[2];
                else if (n == 2)
                    jugador3JugadoLbl.Text = separado[2];
                else if (n == 3)
                    jugador4JugadoLbl.Text = separado[2];
                else if (n == 4)
                    jugador5JugadoLbl.Text = separado[2];
                else if (n == 5)
                    jugador6JugadoLbl.Text = separado[2];
                else if (n == 6)
                    jugador7JugadoLbl.Text = separado[2];
                else if (n == 7)
                    jugador8JugadoLbl.Text = separado[2];

            }
            else if (accion == -1)
            {
                if (n == 0)
                {
                    jugador1FichasLbl.Text = ((Convert.ToInt32(jugador1FichasLbl.Text)) - ((Convert.ToInt32(jugador1JugadoLbl.Text) / 2))).ToString();
                    jugador1JugadoLbl.Text = "0";
                }
                else if (n == 1)
                {
                    jugador2FichasLbl.Text = ((Convert.ToInt32(jugador2FichasLbl.Text)) - ((Convert.ToInt32(jugador2JugadoLbl.Text) / 2))).ToString();
                    jugador2JugadoLbl.Text = "0";
                }
                else if (n == 2)
                {
                    jugador3FichasLbl.Text = ((Convert.ToInt32(jugador3FichasLbl.Text)) - ((Convert.ToInt32(jugador3JugadoLbl.Text) / 2))).ToString();
                    jugador3JugadoLbl.Text = "0";
                }
                else if (n == 3)
                {
                    jugador4FichasLbl.Text = ((Convert.ToInt32(jugador4FichasLbl.Text)) - ((Convert.ToInt32(jugador4JugadoLbl.Text) / 2))).ToString();
                    jugador4JugadoLbl.Text = "0";
                }
                else if (n == 4)
                {
                    jugador5FichasLbl.Text = ((Convert.ToInt32(jugador5FichasLbl.Text)) - ((Convert.ToInt32(jugador5JugadoLbl.Text) / 2))).ToString();
                    jugador5JugadoLbl.Text = "0";
                }
                else if (n == 5)
                {
                    jugador6FichasLbl.Text = ((Convert.ToInt32(jugador6FichasLbl.Text)) - ((Convert.ToInt32(jugador6JugadoLbl.Text) / 2))).ToString();
                    jugador6JugadoLbl.Text = "0";
                }
                else if (n == 6)
                {
                    jugador7FichasLbl.Text = ((Convert.ToInt32(jugador7FichasLbl.Text)) - ((Convert.ToInt32(jugador7JugadoLbl.Text) / 2))).ToString();
                    jugador7JugadoLbl.Text = "0";
                }
                else if (n == 7)
                {
                    jugador8FichasLbl.Text = ((Convert.ToInt32(jugador8FichasLbl.Text)) - ((Convert.ToInt32(jugador8JugadoLbl.Text) / 2))).ToString();
                    jugador8JugadoLbl.Text = "0";
                }
            }
            else if (accion == 1)
            {
                if (jugador1NombreLbl.Text == this.usuario)
                {
                    jugador1PuntosLbl.Text = separado[2];
                    if(jugador1Carta3Lbl.Text == "Carta 3")
                        jugador1Carta3Lbl.Text = separado[3];
                    else if(jugador1Carta4Lbl.Text == "Carta 4")
                        jugador1Carta4Lbl.Text = separado[3];
                    if (Convert.ToInt32(jugador1PuntosLbl.Text) > 20)
                    {
                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                    }
                }
                else if (jugador2NombreLbl.Text == this.usuario)
                {
                    jugador2PuntosLbl.Text = separado[2];
                    if (jugador2Carta3Lbl.Text == "Carta 3")
                        jugador2Carta3Lbl.Text = separado[3];
                    else if (jugador2Carta4Lbl.Text == "Carta 4")
                        jugador2Carta4Lbl.Text = separado[3];
                    if (Convert.ToInt32(jugador2PuntosLbl.Text) > 20)
                    {
                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                    }
                }
                else if (jugador3NombreLbl.Text == this.usuario)
                {
                    jugador3PuntosLbl.Text = separado[2];
                    if (jugador3Carta3Lbl.Text == "Carta 3")
                        jugador3Carta3Lbl.Text = separado[3];
                    else if (jugador3Carta4Lbl.Text == "Carta 4")
                        jugador3Carta4Lbl.Text = separado[3];
                    if (Convert.ToInt32(jugador3PuntosLbl.Text) > 20)
                    {
                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                    }
                }
                else if (jugador4NombreLbl.Text == this.usuario)
                {
                    jugador4PuntosLbl.Text = separado[2];
                    if (jugador4Carta3Lbl.Text == "Carta 3")
                        jugador4Carta3Lbl.Text = separado[3];
                    else if (jugador4Carta4Lbl.Text == "Carta 4")
                        jugador4Carta4Lbl.Text = separado[3];
                    if (Convert.ToInt32(jugador4PuntosLbl.Text) > 20)
                    {
                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                    }
                }
                else if (jugador5NombreLbl.Text == this.usuario)
                {
                    jugador5PuntosLbl.Text = separado[2];
                    if (jugador5Carta3Lbl.Text == "Carta 3")
                        jugador5Carta3Lbl.Text = separado[3];
                    else if (jugador5Carta4Lbl.Text == "Carta 4")
                        jugador5Carta4Lbl.Text = separado[3];
                    if (Convert.ToInt32(jugador5PuntosLbl.Text) > 20)
                    {
                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                    }
                }
                else if (jugador6NombreLbl.Text == this.usuario)
                {
                    jugador6PuntosLbl.Text = separado[2];
                    if (jugador6Carta3Lbl.Text == "Carta 3")
                        jugador6Carta3Lbl.Text = separado[3];
                    else if (jugador6Carta4Lbl.Text == "Carta 4")
                        jugador6Carta4Lbl.Text = separado[3];
                    if (Convert.ToInt32(jugador6PuntosLbl.Text) > 20)
                    {
                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                    }
                }
                else if (jugador7NombreLbl.Text == this.usuario)
                {
                    jugador7PuntosLbl.Text = separado[2];
                    if (jugador7Carta3Lbl.Text == "Carta 3")
                        jugador7Carta3Lbl.Text = separado[3];
                    else if (jugador7Carta4Lbl.Text == "Carta 4")
                        jugador7Carta4Lbl.Text = separado[3];
                    if (Convert.ToInt32(jugador7PuntosLbl.Text) > 20)
                    {
                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                    }
                }
                else if (jugador8NombreLbl.Text == this.usuario)
                {
                    jugador7PuntosLbl.Text = separado[2];
                    if (jugador7Carta3Lbl.Text == "Carta 3")
                        jugador7Carta3Lbl.Text = separado[3];
                    else if (jugador7Carta4Lbl.Text == "Carta 4")
                        jugador7Carta4Lbl.Text = separado[3];
                    if (Convert.ToInt32(jugador8PuntosLbl.Text) > 20)
                    {
                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                    }
                }
            }
            else if (accion == 3)
            {
                if (jugador1NombreLbl.Text == this.usuario)
                {
                    jugador1JugadoLbl.Text = (2 * Convert.ToInt32(jugador1JugadoLbl.Text)).ToString();
                    jugador1PuntosLbl.Text = separado[2];
                    if (jugador1Carta3Lbl.Text == "Carta 3")
                        jugador1Carta3Lbl.Text = separado[3];
                    else if (jugador1Carta4Lbl.Text == "Carta 4")
                        jugador1Carta4Lbl.Text = separado[3]; 

                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;

                    
                }
                else if (jugador2NombreLbl.Text == this.usuario)
                {
                    jugador2JugadoLbl.Text = (2 * Convert.ToInt32(jugador2JugadoLbl.Text)).ToString();
                    jugador2PuntosLbl.Text = separado[2];
                    if (jugador2Carta3Lbl.Text == "Carta 3")
                        jugador2Carta3Lbl.Text = separado[3];
                    else if (jugador2Carta4Lbl.Text == "Carta 4")
                        jugador2Carta4Lbl.Text = separado[3];

                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                 
                }
                else if (jugador3NombreLbl.Text == this.usuario)
                {
                    jugador3JugadoLbl.Text = (2 * Convert.ToInt32(jugador3JugadoLbl.Text)).ToString();
                    jugador3PuntosLbl.Text = separado[2];
                    if (jugador3Carta3Lbl.Text == "Carta 3")
                        jugador3Carta3Lbl.Text = separado[3];
                    else if (jugador3Carta4Lbl.Text == "Carta 4")
                        jugador3Carta4Lbl.Text = separado[3];

                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;

                }
                else if (jugador4NombreLbl.Text == this.usuario)
                {
                    jugador4JugadoLbl.Text = (2 * Convert.ToInt32(jugador4JugadoLbl.Text)).ToString();
                    jugador4PuntosLbl.Text = separado[2];
                    if (jugador4Carta3Lbl.Text == "Carta 3")
                        jugador4Carta3Lbl.Text = separado[3];
                    else if (jugador4Carta4Lbl.Text == "Carta 4")
                        jugador4Carta4Lbl.Text = separado[3];

                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;

                }
                else if (jugador5NombreLbl.Text == this.usuario)
                {
                    jugador5JugadoLbl.Text = (2 * Convert.ToInt32(jugador5JugadoLbl.Text)).ToString();
                    jugador5PuntosLbl.Text = separado[2];
                    if (jugador5Carta3Lbl.Text == "Carta 3")
                        jugador5Carta3Lbl.Text = separado[3];
                    else if (jugador5Carta4Lbl.Text == "Carta 4")
                        jugador5Carta4Lbl.Text = separado[3];

                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                }
                else if (jugador6NombreLbl.Text == this.usuario)
                {
                    jugador6JugadoLbl.Text = (2 * Convert.ToInt32(jugador6JugadoLbl.Text)).ToString();
                    jugador6PuntosLbl.Text = separado[2];
                    if (jugador6Carta3Lbl.Text == "Carta 3")
                        jugador6Carta3Lbl.Text = separado[3];
                    else if (jugador6Carta4Lbl.Text == "Carta 4")
                        jugador6Carta4Lbl.Text = separado[3];

                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                }
                else if (jugador7NombreLbl.Text == this.usuario)
                {
                    jugador7JugadoLbl.Text = (2 * Convert.ToInt32(jugador7JugadoLbl.Text)).ToString();
                    jugador7PuntosLbl.Text = separado[2];
                    if (jugador7Carta3Lbl.Text == "Carta 3")
                        jugador7Carta3Lbl.Text = separado[3];
                    else if (jugador7Carta4Lbl.Text == "Carta 4")
                        jugador7Carta4Lbl.Text = separado[3];

                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;

                }
                else if (jugador8NombreLbl.Text == this.usuario)
                {
                    jugador8JugadoLbl.Text = (2 * Convert.ToInt32(jugador8JugadoLbl.Text)).ToString();
                    jugador8PuntosLbl.Text = separado[2];
                    if (jugador8Carta3Lbl.Text == "Carta 3")
                        jugador8Carta3Lbl.Text = separado[3];
                    else if (jugador8Carta4Lbl.Text == "Carta 4")
                        jugador8Carta4Lbl.Text = separado[3];

                        rendirseBtn.Enabled = false;
                        rendirseBtn.Visible = false;
                        pedirBtn.Enabled = false;
                        pedirBtn.Visible = false;
                        plantarseBtn.Enabled = false;
                        plantarseBtn.Visible = false;
                        apostarBtn.Enabled = false;
                        apostarBtn.Visible = false;
                        apostarBtn.Text = "Apostar";
                        apostarNum.Enabled = false;
                        apostarNum.Visible = false;
                }
            }
        }

        public void Accion15(string mensaje)
        {
            string[] separado = mensaje.Split('/');
            int n = Convert.ToInt32(separado[0]);
            int fichas = Convert.ToInt32(separado[1]);
            int puntos = Convert.ToInt32(separado[2]);
            int numCartas = Convert.ToInt32(separado[3]);
            if (n == 0)
            {
                jugador1FichasLbl.Text = fichas.ToString();
                jugador1PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
                if (numCartas == 1)
                {
                    jugador1Carta1Lbl.Text = separado[4];
                }
                else if (numCartas == 2)
                {
                    jugador1Carta1Lbl.Text = separado[4];
                    jugador1Carta2Lbl.Text = separado[5];
                }
                else if (numCartas == 3)
                {
                    jugador1Carta1Lbl.Text = separado[4];
                    jugador1Carta2Lbl.Text = separado[5];
                    jugador1Carta3Lbl.Text = separado[6];
                }
                else if (numCartas == 4)
                {
                    jugador1Carta1Lbl.Text = separado[4];
                    jugador1Carta2Lbl.Text = separado[5];
                    jugador1Carta3Lbl.Text = separado[6];
                    jugador1Carta4Lbl.Text = separado[7];
                }
                empezarBtn.Text = "Siguiente";
                empezarBtn.Visible = true;
                empezarBtn.Enabled = true;
            }
            else if (n == 1)
            {
                jugador2FichasLbl.Text = fichas.ToString();
                jugador2PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
                if (numCartas == 1)
                {
                    jugador2Carta1Lbl.Text = separado[4];
                }
                else if (numCartas == 2)
                {
                    jugador2Carta1Lbl.Text = separado[4];
                    jugador2Carta2Lbl.Text = separado[5];
                }
                else if (numCartas == 3)
                {
                    jugador2Carta1Lbl.Text = separado[4];
                    jugador2Carta2Lbl.Text = separado[5];
                    jugador2Carta3Lbl.Text = separado[6];
                }
                else if (numCartas == 4)
                {
                    jugador2Carta1Lbl.Text = separado[4];
                    jugador2Carta2Lbl.Text = separado[5];
                    jugador2Carta3Lbl.Text = separado[6];
                    jugador2Carta4Lbl.Text = separado[7];
                }
            }
            else if (n == 2)
            {
                jugador3FichasLbl.Text = fichas.ToString();
                jugador3PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
                if (numCartas == 1)
                {
                    jugador3Carta1Lbl.Text = separado[4];
                }
                else if (numCartas == 2)
                {
                    jugador3Carta1Lbl.Text = separado[4];
                    jugador3Carta2Lbl.Text = separado[5];
                }
                else if (numCartas == 3)
                {
                    jugador3Carta1Lbl.Text = separado[4];
                    jugador3Carta2Lbl.Text = separado[5];
                    jugador3Carta3Lbl.Text = separado[6];
                }
                else if (numCartas == 4)
                {
                    jugador3Carta1Lbl.Text = separado[4];
                    jugador3Carta2Lbl.Text = separado[5];
                    jugador3Carta3Lbl.Text = separado[6];
                    jugador3Carta4Lbl.Text = separado[7];
                }
            }
            else if (n == 3)
            {
                jugador4FichasLbl.Text = fichas.ToString();
                jugador4PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
                if (numCartas == 1)
                {
                    jugador4Carta1Lbl.Text = separado[4];
                }
                else if (numCartas == 2)
                {
                    jugador4Carta1Lbl.Text = separado[4];
                    jugador4Carta2Lbl.Text = separado[5];
                }
                else if (numCartas == 3)
                {
                    jugador4Carta1Lbl.Text = separado[4];
                    jugador4Carta2Lbl.Text = separado[5];
                    jugador4Carta3Lbl.Text = separado[6];
                }
                else if (numCartas == 4)
                {
                    jugador4Carta1Lbl.Text = separado[4];
                    jugador4Carta2Lbl.Text = separado[5];
                    jugador4Carta3Lbl.Text = separado[6];
                    jugador4Carta4Lbl.Text = separado[7];
                }
            }
            else if (n == 4)
            {
                jugador5FichasLbl.Text = fichas.ToString();
                jugador5PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
                if (numCartas == 1)
                {
                    jugador5Carta1Lbl.Text = separado[4];
                }
                else if (numCartas == 2)
                {
                    jugador5Carta1Lbl.Text = separado[4];
                    jugador5Carta2Lbl.Text = separado[5];
                }
                else if (numCartas == 3)
                {
                    jugador5Carta1Lbl.Text = separado[4];
                    jugador5Carta2Lbl.Text = separado[5];
                    jugador5Carta3Lbl.Text = separado[6];
                }
                else if (numCartas == 4)
                {
                    jugador5Carta1Lbl.Text = separado[4];
                    jugador5Carta2Lbl.Text = separado[5];
                    jugador5Carta3Lbl.Text = separado[6];
                    jugador5Carta4Lbl.Text = separado[7];
                }
            }
            else if (n == 5)
            {
                jugador6FichasLbl.Text = fichas.ToString();
                jugador6PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
                if (numCartas == 1)
                {
                    jugador6Carta1Lbl.Text = separado[4];
                }
                else if (numCartas == 2)
                {
                    jugador6Carta1Lbl.Text = separado[4];
                    jugador6Carta2Lbl.Text = separado[5];
                }
                else if (numCartas == 3)
                {
                    jugador6Carta1Lbl.Text = separado[4];
                    jugador6Carta2Lbl.Text = separado[5];
                    jugador6Carta3Lbl.Text = separado[6];
                }
                else if (numCartas == 4)
                {
                    jugador6Carta1Lbl.Text = separado[4];
                    jugador6Carta2Lbl.Text = separado[5];
                    jugador6Carta3Lbl.Text = separado[6];
                    jugador6Carta4Lbl.Text = separado[7];
                }
            }
            else if (n == 6)
            {
                jugador7FichasLbl.Text = fichas.ToString();
                jugador7PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
                if (numCartas == 1)
                {
                    jugador7Carta1Lbl.Text = separado[4];
                }
                else if (numCartas == 2)
                {
                    jugador7Carta1Lbl.Text = separado[4];
                    jugador7Carta2Lbl.Text = separado[5];
                }
                else if (numCartas == 3)
                {
                    jugador7Carta1Lbl.Text = separado[4];
                    jugador7Carta2Lbl.Text = separado[5];
                    jugador7Carta3Lbl.Text = separado[6];
                }
                else if (numCartas == 4)
                {
                    jugador7Carta1Lbl.Text = separado[4];
                    jugador7Carta2Lbl.Text = separado[5];
                    jugador7Carta3Lbl.Text = separado[6];
                    jugador7Carta4Lbl.Text = separado[7];
                }
            }
            else if (n == 7)
            {
                jugador8FichasLbl.Text = fichas.ToString();
                jugador8PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
                if (numCartas == 1)
                {
                    jugador8Carta1Lbl.Text = separado[4];
                }
                else if (numCartas == 2)
                {
                    jugador8Carta1Lbl.Text = separado[4];
                    jugador8Carta2Lbl.Text = separado[5];
                }
                else if (numCartas == 3)
                {
                    jugador8Carta1Lbl.Text = separado[4];
                    jugador8Carta2Lbl.Text = separado[5];
                    jugador8Carta3Lbl.Text = separado[6];
                }
                else if (numCartas == 4)
                {
                    jugador8Carta1Lbl.Text = separado[4];
                    jugador8Carta2Lbl.Text = separado[5];
                    jugador8Carta3Lbl.Text = separado[6];
                    jugador8Carta4Lbl.Text = separado[7];
                }
            }
        }

        public void Accion16(string mensaje)
        {
            
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
            empezarBtn.Enabled = false;
            empezarBtn.Visible = false;
            if (empezarBtn.Text == "Empezar")
            {
                string mensaje = "13/" + this.ID;
                server.Enviar(mensaje);
            }
            else
            {
                string mensaje = "15/" + this.ID;
                server.Enviar(mensaje);
            }
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

        private void rendirseBtn_Click(object sender, EventArgs e)
        {
            //En su turno, el jugador decide rendirse
            //Se envia la decisión al servidor y bloqueamos los objetos relacionados con acciones en la ronda
            string mensaje = "14/" + this.ID + "/-1";
            server.Enviar(mensaje);
            rendirseBtn.Enabled = false;
            rendirseBtn.Visible = false;
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarBtn.Text = "Apostar";
            apostarNum.Enabled = false;
            apostarNum.Visible = false;
        }

        private void pedirBtn_Click(object sender, EventArgs e)
        {
            //En su turno, el jugador decide pedir una carta
            //Se envia la decisión al servidor y bloqueamos los objetos relacionados con acciones en la ronda
            string mensaje = "14/" + this.ID + "/1";
            server.Enviar(mensaje);
        }

        private void plantarseBtn_Click(object sender, EventArgs e)
        {
            //En su turno, el jugador decide plantarse
            //Se envia la decisión al servidor y bloqueamos los objetos relacionados con acciones en la ronda
            string mensaje = "14/" + this.ID + "/2";
            server.Enviar(mensaje);
            rendirseBtn.Enabled = false;
            rendirseBtn.Visible = false;
            pedirBtn.Enabled = false;
            pedirBtn.Visible = false;
            plantarseBtn.Enabled = false;
            plantarseBtn.Visible = false;
            apostarBtn.Enabled = false;
            apostarBtn.Visible = false;
            apostarBtn.Text = "Apostar";
            apostarNum.Enabled = false;
            apostarNum.Visible = false;
        }

        private void apostarBtn_Click(object sender, EventArgs e)
        {
            //Se envia la decisión al servidor y bloqueamos los objetos relacionados con acciones en la ronda
            if (apostarBtn.Text == "Apostar")
            {
                //Apuesta inicial del jugador
                string mensaje = "14/" + this.ID + "/0/" + apostarNum.Value;
                server.Enviar(mensaje);

                rendirseBtn.Enabled = true;
                rendirseBtn.Visible = true;
                pedirBtn.Enabled = true;
                pedirBtn.Visible = true;
                plantarseBtn.Enabled = true;
                plantarseBtn.Visible = true;
                apostarBtn.Text = "Doblar";
                apostarNum.Enabled = false;
                apostarNum.Visible = false;
            }
            else
            {
                //El jugador decide doblar la apuesta y pedir una ultima carta
                string mensaje = "14/" + this.ID + "/3";
                server.Enviar(mensaje);

            }
        }
    }
}
