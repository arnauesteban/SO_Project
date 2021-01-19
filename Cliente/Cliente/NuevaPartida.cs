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

        //Matriz de los PictureBox [Jugador, PictureBox]
        PictureBox[,] PictureBoxCartas = new PictureBox[8,6];

        int[] numCartasPorJugador = new int[8];

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
            //Organizamos todos los PictureBox en la matrix PictureBoxCartas
            PictureBoxCartas[0, 0] = pictureBox1_MiJugador;
            PictureBoxCartas[0, 1] = pictureBox2_MiJugador;
            PictureBoxCartas[0, 2] = pictureBox3_MiJugador;
            PictureBoxCartas[0, 3] = pictureBox4_MiJugador;
            PictureBoxCartas[0, 4] = pictureBox5_MiJugador;
            PictureBoxCartas[0, 5] = pictureBox6_MiJugador;

            PictureBoxCartas[1, 0] = pictureBox1_Jugador2;
            PictureBoxCartas[1, 1] = pictureBox2_Jugador2;
            PictureBoxCartas[1, 2] = pictureBox3_Jugador2;
            PictureBoxCartas[1, 3] = pictureBox4_Jugador2;
            PictureBoxCartas[1, 4] = pictureBox5_Jugador2;
            PictureBoxCartas[1, 5] = pictureBox6_Jugador2;

            PictureBoxCartas[2, 0] = pictureBox1_Jugador3;
            PictureBoxCartas[2, 1] = pictureBox2_Jugador3;
            PictureBoxCartas[2, 2] = pictureBox3_Jugador3;
            PictureBoxCartas[2, 3] = pictureBox4_Jugador3;
            PictureBoxCartas[2, 4] = pictureBox5_Jugador3;
            PictureBoxCartas[2, 5] = pictureBox6_Jugador3;

            PictureBoxCartas[3, 0] = pictureBox1_Jugador4;
            PictureBoxCartas[3, 1] = pictureBox2_Jugador4;
            PictureBoxCartas[3, 2] = pictureBox3_Jugador4;
            PictureBoxCartas[3, 3] = pictureBox4_Jugador4;
            PictureBoxCartas[3, 4] = pictureBox5_Jugador4;
            PictureBoxCartas[3, 5] = pictureBox6_Jugador4;

            PictureBoxCartas[4, 0] = pictureBox1_Jugador5;
            PictureBoxCartas[4, 1] = pictureBox2_Jugador5;
            PictureBoxCartas[4, 2] = pictureBox3_Jugador5;
            PictureBoxCartas[4, 3] = pictureBox4_Jugador5;
            PictureBoxCartas[4, 4] = pictureBox5_Jugador5;
            PictureBoxCartas[4, 5] = pictureBox6_Jugador5;

            PictureBoxCartas[5, 0] = pictureBox1_Jugador6;
            PictureBoxCartas[5, 1] = pictureBox2_Jugador6;
            PictureBoxCartas[5, 2] = pictureBox3_Jugador6;
            PictureBoxCartas[5, 3] = pictureBox4_Jugador6;
            PictureBoxCartas[5, 4] = pictureBox5_Jugador6;
            PictureBoxCartas[5, 5] = pictureBox6_Jugador6;

            PictureBoxCartas[6, 0] = pictureBox1_Jugador7;
            PictureBoxCartas[6, 1] = pictureBox2_Jugador7;
            PictureBoxCartas[6, 2] = pictureBox3_Jugador7;
            PictureBoxCartas[6, 3] = pictureBox4_Jugador7;
            PictureBoxCartas[6, 4] = pictureBox5_Jugador7;
            PictureBoxCartas[6, 5] = pictureBox6_Jugador7;

            PictureBoxCartas[7, 0] = pictureBox1_Jugador8;
            PictureBoxCartas[7, 1] = pictureBox2_Jugador8;
            PictureBoxCartas[7, 2] = pictureBox3_Jugador8;
            PictureBoxCartas[7, 3] = pictureBox4_Jugador8;
            PictureBoxCartas[7, 4] = pictureBox5_Jugador8;
            PictureBoxCartas[7, 5] = pictureBox6_Jugador8;

            for (int i = 1; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    PictureBoxCartas[i, j].Visible = false;
                    PictureBoxCartas[i, j].ImageLocation = "..\\..\\Img\\cards\\dorso.png";
                }
            }

        }

        public int getID()
        {
            //Método que devuelve el identificador de partida.
            return this.ID;
        }

        public void resetLabels()
        {
 
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
            jugador1FichasLbl.Text = "100";
            jugador1PuntosLbl.Text = "0";
        }

        public void Accion13(string mensaje)
        {
            //Función que es llamada cada vez que se recibe un mensaje conforme comienza una ronda
            //Quita los objetos de los parámetros iniciales y muestra las cartas que lleguen

            //Reseteamos las cartas
            for (int j = 0; j < numCartasPorJugador[0]; j++)
            {
                PictureBoxCartas[0, j].Image = null;
            }
            for (int i = 1; i < 8; i++)
            {
                for (int j = 0; j < numCartasPorJugador[i]; j++)
                {
                    PictureBoxCartas[i, j].Visible = false;
                }
                
            }

            //Recibimos mensaje :  puntos/numero1-palo1/numero2-palo2/numJugador/numero_de_jugadores
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

            numCartasPorJugador[0] = 2;

            //Asignamos a carta1 y carta2 el nombre de sus imagenes correspondientes
            string carta1, carta2;
            carta1 = AsignarNombreImagenCarta(separado[1]);
            carta2 = AsignarNombreImagenCarta(separado[2]);

            //Insertamos la imagen de cada carta de mi jugador
            PictureBoxCartas[0, 0].ImageLocation = "..\\..\\Img\\cards\\" + carta1;
            PictureBoxCartas[0, 1].ImageLocation = "..\\..\\Img\\cards\\" + carta2;

            //Insertamos la imagen del dorso de las cartas iniciales del resto de jugadores
            for (int i = 1; i < numJugadores; i++)
            {
                PictureBoxCartas[i, 0].Visible = true;
                PictureBoxCartas[i, 1].Visible = true;
                numCartasPorJugador[i] = 2;
            }


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
                int numJugador_carta_recibida = Convert.ToInt32(separado[0]);
                //Si la carta que se ha pedido es nuestra
                if (numJugador_carta_recibida == numJugador)
                {
                    jugador1PuntosLbl.Text = separado[2];

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

                    //Asignamos a carta el nombre de sus imagenes correspondientes
                    string carta;
                    carta = AsignarNombreImagenCarta(separado[3]);

                    //Insertamos la imagen de cada carta
                    PictureBoxCartas[0, numCartasPorJugador[0]].ImageLocation = "..\\..\\Img\\cards\\" + carta;
                    numCartasPorJugador[0]++;
                }
                //Si la carta no es mia
                else
                {
                    //Si es el croupier
                    if (numJugador_carta_recibida == 0)
                    {
                        PictureBoxCartas[1, numCartasPorJugador[1]].Visible = true;
                        numCartasPorJugador[1]++;
                    }
                    else
                    {
                        if (numJugador_carta_recibida == 1 && numJugador != 0)
                        {
                            PictureBoxCartas[numJugador, numCartasPorJugador[numJugador]].Visible = true;
                            numCartasPorJugador[numJugador]++;
                        }
                        else
                        {
                            PictureBoxCartas[numJugador_carta_recibida, numCartasPorJugador[numJugador_carta_recibida]].Visible = true;
                            numCartasPorJugador[numJugador_carta_recibida]++;
                        }
                    }
                }

            }

            //3 cuando dobla
            else if (accion == 3)
            {
                int numJugador_carta_recibida = Convert.ToInt32(separado[0]);
                //Si la carta que se ha pedido es nuestra
                if (numJugador_carta_recibida == numJugador)
                {
                    jugador1JugadoLbl.Text = (2 * Convert.ToInt32(jugador1JugadoLbl.Text)).ToString();
                    jugador1PuntosLbl.Text = separado[2];

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

                    //Asignamos a carta el nombre de sus imagenes correspondientes
                    string carta;
                    carta = AsignarNombreImagenCarta(separado[3]);

                    //Insertamos la imagen de cada carta
                    PictureBoxCartas[0, numCartasPorJugador[0]].ImageLocation = "..\\..\\Img\\cards\\" + carta;
                    numCartasPorJugador[0]++;
                }
                //Si la carta no es mia
                else
                {
                    //Si es el croupier
                    if (numJugador_carta_recibida == 0)
                    {
                        PictureBoxCartas[1, numCartasPorJugador[1]].Visible = true;
                        numCartasPorJugador[1]++;
                    }
                    else
                    {
                        if (numJugador_carta_recibida == 1 && numJugador != 0)
                        {
                            PictureBoxCartas[numJugador, numCartasPorJugador[numJugador]].Visible = true;
                            numCartasPorJugador[numJugador]++;
                        }
                        else
                        {
                            PictureBoxCartas[numJugador_carta_recibida, numCartasPorJugador[numJugador_carta_recibida]].Visible = true;
                            numCartasPorJugador[numJugador_carta_recibida]++;
                        }
                    }
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

            if (n == numJugador)
            {
                jugador1FichasLbl.Text = fichas.ToString();
                jugador1PuntosLbl.Text = puntos.ToString();
                jugador1JugadoLbl.Text = "0";
            }
                
            empezarBtn.Text = "Siguiente";
            empezarBtn.Visible = true;
            empezarBtn.Enabled = true;
            
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
