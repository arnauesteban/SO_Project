using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Cliente
{
    public partial class consulta_resultados : Form
    {
        Server server;
        int consulta;
        public consulta_resultados(Server server)
        {
            InitializeComponent();
            this.server = server;
            this.consulta = -1;
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

        private void buscarBtn_Click(object sender, EventArgs e)
        {
            if (consulta == 1)
            {
                bool error = ComprobarCaracteres(nombreIn.Text, "nombre");
                if (!error)
                {
                    string mensaje = "9/" + nombreIn.Text;
                    server.Enviar(mensaje);
                    this.Close();
                }
            }
            else if (consulta == 2)
            {
                string mensaje = "10/";
                server.Enviar(mensaje);
                this.Close();
            }
            else if (consulta == 3)
            {
                bool error = ComprobarCaracteres(nombreIn.Text, "nombre");
                if (!error)
                {
                    string mensaje = "11/" + nombreIn.Text;
                    server.Enviar(mensaje);
                    this.Close();
                }
            }
        }

        private void cancelarBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void consulta1Btn_CheckedChanged(object sender, EventArgs e)
        {
            if (consulta1Btn.Checked)
            {
                consulta = 1;
                nombreIn.Enabled = true;
                buscarBtn.Enabled = true;
            }
        }

        private void consulta_resultados_Load(object sender, EventArgs e)
        {
            nombreIn.Enabled = false;
            buscarBtn.Enabled = false;
        }

        private void consulta2Btn_CheckedChanged(object sender, EventArgs e)
        {
            if (consulta2Btn.Checked)
            {
                consulta = 2;
                nombreIn.Enabled = false;
                buscarBtn.Enabled = true;
            }
        }

        private void consulta3Btn_CheckedChanged(object sender, EventArgs e)
        {
            if (consulta3Btn.Checked)
            {
                consulta = 3;
                nombreIn.Enabled = true;
                buscarBtn.Enabled = true;
            }
        }
    }
}
