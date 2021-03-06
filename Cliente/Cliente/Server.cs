﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace Cliente
{
    public class Server
    {
        Socket server;
        bool conectado = false;

        public bool IsConnected()
        {
            return this.conectado;
        }

        //Retorna 1 si se ha conectado, y 0 si no
        public int Conectar()
        {
            //Creamos un IPEndPoint con el ip del servidor y puerto del servidor al que deseamos conectarnos

            //Parametros de shiva
            IPAddress direc = IPAddress.Parse("147.83.117.22");
            IPEndPoint ipep = new IPEndPoint(direc, 50082);

            //Parametros de pruebas
            //IPAddress direc = IPAddress.Parse("192.168.56.101"); //101 Sergi 102 Arnau
            //IPEndPoint ipep = new IPEndPoint(direc, 50082);

            //Creamos el socket 
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                server.Connect(ipep); //Intentamos conectar el socket
            }
            catch (SocketException)
            {
                //Si hay excepcion imprimimos error y salimos del programa con return 
                return 0;
            }
            this.conectado = true;
            return 1;
        }

        public void Desconectar()
        {
            Enviar("0/");

            // Nos desconectamos
            server.Shutdown(SocketShutdown.Both);
            server.Close();

            conectado = false;
        }

        public string Recibir()
        {
            byte[] msg2 = new byte[80];
            try
            {
                server.Receive(msg2);
            }
            catch (SocketException)
            {
                conectado = false;
            }
            return Encoding.ASCII.GetString(msg2);
        }
        public void Enviar(string sentencia)
        {
            string[] separado = sentencia.Split('/');
            try
            {
                //Extraemos el código de mensaje a enviar. Si el formato es incorrecto, se detecta un FormatException
                int codigo = Convert.ToInt32(sentencia[0]);

                //Si el formato es correcto, se sigue con el proceso.
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
            catch (FormatException)
            {
                //Si hay un error en el formato del mensaje a enviar, siemplemente se descarta este mensaje
            }
        }

        public Socket GetSocket()
        {
            return this.server;
        }

    }
}
