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
using System.Collections.Generic;

namespace Cliente
{
    public partial class NuevaPartida : Form
    {
        //Variables globales del formulario: datos de conexión con el servidor, identificador de partida, nombre del usuario y dealer de la ronda.
        Server server;
        int ID;
        string usuario;
        int numJugador;
        int lineasChat;
        bool banca;
        int numJugadores;

        PictureBox[] PictureBoxCartas = new PictureBox[5];

        List<string> cartas = new List<string>();

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

        private void NuevaPartida_Load(object sender, EventArgs e)
        {
            PictureBoxCartas[0] = pictureBox1;
            PictureBoxCartas[1] = pictureBox2;
            PictureBoxCartas[2] = pictureBox3;
            PictureBoxCartas[3] = pictureBox4;
            PictureBoxCartas[4] = pictureBox5;
        }

        public int getID()
        {
            //Método que devuelve el identificador de partida.
            return this.ID;
        }

        public void resetLabels()
        {
 
            jugador1Carta1Lbl.Text = "Carta 1";
            jugador1Carta2Lbl.Text = "Carta 2";
            jugador1Carta3Lbl.Text = "Carta 3";
            jugador1Carta4Lbl.Text = "Carta 4";
            jugador1JugadoLbl.Text = "0";
            jugador1PuntosLbl.Text = "0";
            

        }

        //A partir de la carta en formato numero-palo, retornamos el nombre del archivo imagen de esa carta
        private string AsignarNombreImagenCarta(string carta)
        {
            string numero = carta.Split('-')[0];
            string palo = carta.Split('-')[1];

            for (int i = 0; i < 2; i++)
            {
                switch (palo)
                {
                    case "1":
                        palo = "Pica";
                        break;

                    case "2":
                        palo = "Trebol";
                        break;

                    case "3":
                        palo = "Corazon";
                        break;

                    case "4":
                        palo = "Diamante";
                        break;
                }

                switch (numero)
                {
                    case "11":
                        numero = "J";
                        break;

                    case "12":
                        numero = "Q";
                        break;

                    case "13":
                        numero = "K";
                        break;

                    case "1":
                        numero = "A";
                        break;
                }
            }

            return numero + "_" + palo + ".png";
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

            jugador1JugadoLbl.Text = "0";
            jugador1Carta1Lbl.Text = "Carta 1";
            jugador1Carta2Lbl.Text = "Carta 2";
            jugador1Carta3Lbl.Text = "Carta 3";
            jugador1Carta4Lbl.Text = "Carta 4";
            jugador1FichasLbl.Text = "100";
            jugador1PuntosLbl.Text = "0";
        }

        public void Accion13(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje conforme comienza una ronda
            //Quita los objetos de los parámetros iniciales y muestra las cartas que lleguen

            //Recibimos mensaje :  puntos/numero1-palo1/numero2-palo2
            string[] separado = mensaje.Split('/');
            empezarBtn.Visible = false;

            resetLabels();

            //Comienza una nueva ronda y llegan las dos primeras cartas
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

            apostarNum.Maximum = Convert.ToInt32(jugador1FichasLbl.Text);

            this.numJugador = Convert.ToInt32(separado[3]);

            cartas.Add(separado[1]);
            cartas.Add(separado[2]);

            //Asignamos a carta1 y carta2 el nombre de sus imagenes correspondientes
            string carta1, carta2;
            carta1 = AsignarNombreImagenCarta(separado[1]);
            carta2 = AsignarNombreImagenCarta(separado[2]);

            //Insertamos la imagen de cada carta
            PictureBoxCartas[0].ImageLocation = "..\\..\\Img\\cards\\" + carta1;
            PictureBoxCartas[1].ImageLocation = "..\\..\\Img\\cards\\" + carta2;

        }

        public void Accion14(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje para mostrar la acción de algún jugador
            //Actualiza las etiquetas del jugador que ha hecho la acción

            //mensaje = numJugador/accion
            
            string[] separado = mensaje.Split('/');
            int n = Convert.ToInt32(separado[0]);
            int accion = Convert.ToInt32(separado[1]);

            //Accion = 0/N cuando al principio apuesta N fichas
            if (accion == 0)
            {
                if (n == this.numJugador)
                    jugador1JugadoLbl.Text = separado[2];
            }
            //-1 cuando se retira
            else if (accion == -1)
            {
                if (n == this.numJugador)
                {
                    jugador1FichasLbl.Text = ((Convert.ToInt32(jugador1FichasLbl.Text)) - ((Convert.ToInt32(jugador1JugadoLbl.Text) / 2))).ToString();
                    jugador1JugadoLbl.Text = "0";
                }
            }
            //1 cuando pide carta
            else if (accion == 1)
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

                cartas.Add(separado[3]);

                //Asignamos a carta el nombre de sus imagenes correspondientes
                string carta;
                carta = AsignarNombreImagenCarta(separado[3]);

                //Insertamos la imagen de cada carta
                PictureBoxCartas[cartas.Count-1].ImageLocation = "..\\..\\Img\\cards\\" + carta;

            }
            //3 cuando dobla
            else if (accion == 3)
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

                cartas.Add(separado[3]);

                //Asignamos a carta el nombre de sus imagenes correspondientes
                string carta;
                carta = AsignarNombreImagenCarta(separado[3]);

                //Insertamos la imagen de cada carta
                PictureBoxCartas[cartas.Count - 1].ImageLocation = "..\\..\\Img\\cards\\" + carta;

            }
        }

        public void Accion15(string mensaje)
        {
            string[] separado = mensaje.Split('/');
            int n = Convert.ToInt32(separado[0]);
            int fichas = Convert.ToInt32(separado[1]);
            int puntos = Convert.ToInt32(separado[2]);
            int numCartas = Convert.ToInt32(separado[3]);
            /*if (n == 0)
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
            }*/
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
