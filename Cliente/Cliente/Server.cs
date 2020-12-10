using System;
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
            //IPAddress direc = IPAddress.Parse("147.83.117.22");
            //IPEndPoint ipep = new IPEndPoint(direc, 50084);

            //Parametros de pruebas
            IPAddress direc = IPAddress.Parse("192.168.56.101");
            IPEndPoint ipep = new IPEndPoint(direc, 9050);

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

        public Socket GetSocket()
        {
            return this.server;
        }

    }
}
