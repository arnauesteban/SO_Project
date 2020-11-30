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
        Socket socket;
        Thread atender;
        public NuevaPartida(Socket socket)
        {
            InitializeComponent();
            this.socket = socket;
            CheckForIllegalCrossThreadCalls = false;
        }

        private void EnviarServidor(string sentencia)
        {
            byte[] msg = System.Text.Encoding.ASCII.GetBytes(sentencia);
            socket.Send(msg);
        }

        private void NuevaPartida_Load(object sender, EventArgs e)
        {
            //Iniciamos el thread de atencion al servidor
            ThreadStart ts = delegate { atenderServidor(); };
            atender = new Thread(ts);
            atender.Start();
        }
        private void atenderServidor()
        {
            while (true)
            {
                //Recibimos la respuesta del servidor
                byte[] msg2 = new byte[80];
                socket.Receive(msg2);
                string[] trozos = Encoding.ASCII.GetString(msg2).Split('$');
                int codigo = Convert.ToInt32(trozos[0]);
                string mensaje = trozos[1].Split('\0')[0];
                string[] separado = mensaje.Split('/');

                switch (codigo)
                {
                    case 9:
                        if (Convert.ToInt32(separado[0]) == 1)
                        {
                            //Invitacion aceptada
                            JugadoresUnidosGrid.ColumnCount = 1;
                            JugadoresUnidosGrid.RowCount = 6;
                            JugadoresUnidosGrid.ColumnHeadersVisible = false;
                            JugadoresUnidosGrid.RowHeadersVisible = false;
                            JugadoresUnidosGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                            JugadoresUnidosGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

                            //Introducimos el nuevo usuario que se ha unido a la partida en la data grid view
                            int i = 0;
                            bool encontrado = false;
                            while (i < JugadoresUnidosGrid.RowCount && !encontrado)
                            {
                                if (JugadoresUnidosGrid[0, i].Value == null)
                                    encontrado = true;
                                else
                                    i++;
                            }
                            if(encontrado)
                                JugadoresUnidosGrid[0, i].Value = separado[1];
                            

                            //Sets the alignment of all columns to middle center
                            this.JugadoresUnidosGrid.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                        }
                        break;
                }


            }
        }
        
        private void InvitarBtn_Click(object sender, EventArgs e)
            {
                string[] invitados = invitadosIn.Text.Split(' ');

                string mensaje_invitados = "";
                for (int i = 0; i < invitados.Length; i++)
                {
                    mensaje_invitados += invitados[i];
                }

                //Enviamos la invitacion al servidor
                EnviarServidor("8/" + invitados.Length + "/" + mensaje_invitados);
            }

        
    }
}
