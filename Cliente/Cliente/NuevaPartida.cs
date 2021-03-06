﻿using System;
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
        int numJugador;
        int lineasChat;
        bool banca;
        int numJugadores;

        //Matriz de los PictureBox [Jugador, PictureBox]
        PictureBox[,] PictureBoxCartas = new PictureBox[8, 6];
        Label[,] LabelsJugadores = new Label[8, 7];

        int[] numCartasPorJugador = new int[8];

        public delegate void DelegadoRespuesta(string mensaje);

        public NuevaPartida(Server server, string usuario)
        {
            //Constructor del formulario usado cuando el usuario es el host de la partida.
            InitializeComponent();
            this.server = server;
            this.usuario = usuario;
            this.numJugadores = 1;
            usuarioLbl.Text = this.usuario;
            chatLbl.Text = "¡Bienvenido a una nueva partida de poker! Espera a que tus contrincantes acepten tu invitación o pulsa el botón del centro de la mesa.";
            lineasChat = 4;
            this.ID = 0;
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
            //Organizamos todos los PictureBox en la matriz PictureBoxCartas
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

            //Organizamos los label de los jugadores en la matriz LabelsJugadores
            LabelsJugadores[1, 0] = jugador2NombreLbl;
            LabelsJugadores[1, 1] = jugador2FichasLbl;
            LabelsJugadores[1, 2] = jugador2JugadoLbl;
            LabelsJugadores[1, 3] = jugador2PuntosLbl;
            LabelsJugadores[1, 4] = jugador2FichasTitLbl;
            LabelsJugadores[1, 5] = jugador2JugadoTitLbl;
            LabelsJugadores[1, 6] = jugador2PuntosTitLbl;

            LabelsJugadores[2, 0] = jugador3NombreLbl;
            LabelsJugadores[2, 1] = jugador3FichasLbl;
            LabelsJugadores[2, 2] = jugador3JugadoLbl;
            LabelsJugadores[2, 3] = jugador3PuntosLbl;
            LabelsJugadores[2, 4] = jugador3FichasTitLbl;
            LabelsJugadores[2, 5] = jugador3JugadoTitLbl;
            LabelsJugadores[2, 6] = jugador3PuntosTitLbl;

            LabelsJugadores[3, 0] = jugador4NombreLbl;
            LabelsJugadores[3, 1] = jugador4FichasLbl;
            LabelsJugadores[3, 2] = jugador4JugadoLbl;
            LabelsJugadores[3, 3] = jugador4PuntosLbl;
            LabelsJugadores[3, 4] = jugador4FichasTitLbl;
            LabelsJugadores[3, 5] = jugador4JugadoTitLbl;
            LabelsJugadores[3, 6] = jugador4PuntosTitLbl;

            LabelsJugadores[4, 0] = jugador5NombreLbl;
            LabelsJugadores[4, 1] = jugador5FichasLbl;
            LabelsJugadores[4, 2] = jugador5JugadoLbl;
            LabelsJugadores[4, 3] = jugador5PuntosLbl;
            LabelsJugadores[4, 4] = jugador5FichasTitLbl;
            LabelsJugadores[4, 5] = jugador5JugadoTitLbl;
            LabelsJugadores[4, 6] = jugador5PuntosTitLbl;

            LabelsJugadores[5, 0] = jugador6NombreLbl;
            LabelsJugadores[5, 1] = jugador6FichasLbl;
            LabelsJugadores[5, 2] = jugador6JugadoLbl;
            LabelsJugadores[5, 3] = jugador6PuntosLbl;
            LabelsJugadores[5, 4] = jugador6FichasTitLbl;
            LabelsJugadores[5, 5] = jugador6JugadoTitLbl;
            LabelsJugadores[5, 6] = jugador6PuntosTitLbl;

            LabelsJugadores[6, 0] = jugador7NombreLbl;
            LabelsJugadores[6, 1] = jugador7FichasLbl;
            LabelsJugadores[6, 2] = jugador7JugadoLbl;
            LabelsJugadores[6, 3] = jugador7PuntosLbl;
            LabelsJugadores[6, 4] = jugador7FichasTitLbl;
            LabelsJugadores[6, 5] = jugador7JugadoTitLbl;
            LabelsJugadores[6, 6] = jugador7PuntosTitLbl;

            LabelsJugadores[7, 0] = jugador8NombreLbl;
            LabelsJugadores[7, 1] = jugador8FichasLbl;
            LabelsJugadores[7, 2] = jugador8JugadoLbl;
            LabelsJugadores[7, 3] = jugador8PuntosLbl;
            LabelsJugadores[7, 4] = jugador8FichasTitLbl;
            LabelsJugadores[7, 5] = jugador8JugadoTitLbl;
            LabelsJugadores[7, 6] = jugador8PuntosTitLbl;

            if (ID == 0)
            {
                int k = 1;
                while (k < 8)
                {
                    LabelsJugadores[k, 0].Text = "";
                    LabelsJugadores[k, 1].Text = "";
                    LabelsJugadores[k, 2].Text = "";
                    LabelsJugadores[k, 3].Text = "";
                    LabelsJugadores[k, 4].Text = "";
                    LabelsJugadores[k, 5].Text = "";
                    LabelsJugadores[k, 6].Text = "";
                    k++;
                }
            }

            string mensaje = "12/" + this.ID;
            server.Enviar(mensaje);
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

        public int getID()
        {
            //Método que devuelve el identificador de partida.
            return this.ID;
        }

        public void resetLabels()
        {
 
            jugador1JugadoLbl.Text = "0";
            jugador1PuntosLbl.Text = "0";
            int i = 1;
            while (i < numJugadores)
            {
                LabelsJugadores[i, 2].Text = "0";
                LabelsJugadores[i, 3].Text = "0";
                i++;
            }

        }

        private string AsignarNombreImagenCarta(string carta)
        {
            //A partir de la carta en formato numero-palo, retornamos el nombre del archivo imagen de esa carta
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

            if (!banca)
            {
                int i = 1;
                int index = 1;
                while (index < n)
                {
                    if (separado[i] != this.usuario)
                    {
                        LabelsJugadores[index, 0].Text = separado[i];
                        LabelsJugadores[index, 1].Text = "100";
                        LabelsJugadores[index, 2].Text = "0";
                        LabelsJugadores[index, 3].Text = "0";
                        LabelsJugadores[index, 4].Text = "Fichas:";
                        LabelsJugadores[index, 5].Text = "Apuesta:";
                        LabelsJugadores[index, 6].Text = "Puntos:";
                        index++;
                    }
                    i++;
                }
                while (i < 8)
                {
                    LabelsJugadores[i, 0].Text = "";
                    LabelsJugadores[i, 1].Text = "";
                    LabelsJugadores[i, 2].Text = "";
                    LabelsJugadores[i, 3].Text = "";
                    LabelsJugadores[i, 4].Text = "";
                    LabelsJugadores[i, 5].Text = "";
                    LabelsJugadores[i, 6].Text = "";
                    i++;
                }
            }
            else
            {
                int i = 1;
                while (i < n)
                {
                    LabelsJugadores[i, 0].Text = separado[i + 1];
                    LabelsJugadores[i, 1].Text = "100";
                    LabelsJugadores[i, 2].Text = "0";
                    LabelsJugadores[i, 3].Text = "0";
                    LabelsJugadores[i, 4].Text = "Fichas:";
                    LabelsJugadores[i, 5].Text = "Apuesta:";
                    LabelsJugadores[i, 6].Text = "Puntos:";
                    i++;
                }
                while (i < 8)
                {
                    LabelsJugadores[i, 0].Text = "";
                    LabelsJugadores[i, 1].Text = "";
                    LabelsJugadores[i, 2].Text = "";
                    LabelsJugadores[i, 3].Text = "";
                    LabelsJugadores[i, 4].Text = "";
                    LabelsJugadores[i, 5].Text = "";
                    LabelsJugadores[i, 6].Text = "";
                    i++;
                }
            }
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
                for (int j = 0; j < 6; j++)
                {
                     PictureBoxCartas[i, j].Visible = false;
                }
                
            }
            for (int i = 1; i < 8; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    PictureBoxCartas[i, j].Visible = false;
                    PictureBoxCartas[i, j].ImageLocation = "..\\..\\Img\\cards\\dorso.png";
                }
            }

            //Recibimos mensaje :  puntos/numero1-palo1/numero2-palo2/numJugador
            string[] separado = mensaje.Split('/');
            empezarBtn.Visible = false;

            resetLabels();

            //Comienza una nueva ronda y llegan las dos primeras cartas
            jugador1PuntosLbl.Text = separado[0];
            if (Convert.ToInt32(jugador1PuntosLbl.Text) > 20)
            {
                if (banca)
                {
                    apostarBtn.Enabled = false;
                    apostarBtn.Visible = false;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = false;
                    apostarNum.Visible = false;
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                }
                else
                {
                    apostarBtn.Enabled = true;
                    apostarBtn.Visible = true;
                    apostarBtn.Text = "Apostar";
                    apostarNum.Enabled = true;
                    apostarNum.Visible = true;
                    rendirseBtn.Enabled = false;
                    rendirseBtn.Visible = false;
                    pedirBtn.Enabled = false;
                    pedirBtn.Visible = false;
                    plantarseBtn.Enabled = false;
                    plantarseBtn.Visible = false;
                }
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
                else if (n < this.numJugador)
                {
                    LabelsJugadores[n + 1, 2].Text = separado[2];
                }
                else
                {
                    LabelsJugadores[n, 2].Text = separado[2];
                }
            }
            //-1 cuando se retira
            else if (accion == -1)
            {
                if (n == this.numJugador)
                {
                    jugador1FichasLbl.Text = ((Convert.ToInt32(jugador1FichasLbl.Text)) - ((Convert.ToInt32(jugador1JugadoLbl.Text) / 2))).ToString();
                    jugador1JugadoLbl.Text = "0";
                }
                else if (n < this.numJugador)
                {
                    LabelsJugadores[n + 1, 1].Text = ((Convert.ToInt32(LabelsJugadores[n + 1, 1].Text)) - ((Convert.ToInt32(LabelsJugadores[n + 1, 2].Text) / 2))).ToString();
                    LabelsJugadores[n + 1, 2].Text = "0";
                }
                else
                {
                    LabelsJugadores[n, 1].Text = ((Convert.ToInt32(LabelsJugadores[n, 1].Text)) - ((Convert.ToInt32(LabelsJugadores[n, 2].Text) / 2))).ToString();
                    LabelsJugadores[n, 2].Text = "0";
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
                            LabelsJugadores[numJugador, 2].Text = (2 * Convert.ToInt32(LabelsJugadores[numJugador, 2].Text)).ToString();
                            numCartasPorJugador[numJugador]++;
                        }
                        else
                        {
                            PictureBoxCartas[numJugador_carta_recibida, numCartasPorJugador[numJugador_carta_recibida]].Visible = true;
                            LabelsJugadores[numJugador_carta_recibida, 2].Text = (2 * Convert.ToInt32(LabelsJugadores[numJugador_carta_recibida, 2].Text)).ToString();
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
                jugador1JugadoLbl.Text = "0";
                jugador1PuntosLbl.Text = puntos.ToString();
            }
            else if (n < numJugador)
            {
                LabelsJugadores[n + 1, 1].Text = fichas.ToString();
                LabelsJugadores[n + 1, 2].Text = "0";
                LabelsJugadores[n + 1, 3].Text = puntos.ToString();
                int i = 0;
                while (i < numCartas)
                {
                    //Asignamos a carta el nombre de sus imagenes correspondientes
                    string carta;
                    carta = AsignarNombreImagenCarta(separado[i + 4]);

                    //Insertamos la imagen de cada carta
                    PictureBoxCartas[n + 1, i].ImageLocation = "..\\..\\Img\\cards\\" + carta;
                    numCartasPorJugador[n + 1]++;
                    i++;
                }
            }
            else
            {
                LabelsJugadores[n, 1].Text = fichas.ToString();
                LabelsJugadores[n, 2].Text = "0";
                LabelsJugadores[n, 3].Text = puntos.ToString();
                int i = 0;
                while (i < numCartas)
                {
                    //Asignamos a carta el nombre de sus imagenes correspondientes
                    string carta;
                    carta = AsignarNombreImagenCarta(separado[i + 4]);

                    //Insertamos la imagen de cada carta
                    PictureBoxCartas[n, i].ImageLocation = "..\\..\\Img\\cards\\" + carta;
                    numCartasPorJugador[n]++;
                    i++;
                }
            }

            if (n == 0)
            {
                empezarBtn.Text = "Siguiente";
                empezarBtn.Visible = true;
                empezarBtn.Enabled = true;
            }
            
        }

        public void Accion16(string mensaje)
        {
            MessageBox.Show("La partida ha terminado. ¡Bien jugado!");
            this.Close();
        }

        private void enviar_Btn_Click(object sender, EventArgs e)
        {
            //Evento llamado cuando el usuario pulsa el botón "Enviar". Envía el mensaje que ha escrito en el textBox al servidor.
            bool error = ComprobarCaracteres(chatTextBox.Text, "chat");
            if (!error)
            {
                string mensaje = "6/" + this.ID + "/" + this.usuario + ": " + chatTextBox.Text;
                server.Enviar(mensaje);
                chatTextBox.Text = "Escribe algo";
            }
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
                if (2 * apostarNum.Value > Convert.ToInt32(jugador1FichasLbl.Text))
                {
                    apostarBtn.Visible = false;
                    apostarBtn.Enabled = false;
                }
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
