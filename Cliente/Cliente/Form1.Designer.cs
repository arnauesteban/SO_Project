namespace Cliente
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.enviar_Btn = new System.Windows.Forms.Button();
            this.usuario = new System.Windows.Forms.TextBox();
            this.clave = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.registrar = new System.Windows.Forms.RadioButton();
            this.iniciar_sesion = new System.Windows.Forms.RadioButton();
            this.puntosPerdedores = new System.Windows.Forms.RadioButton();
            this.nombresPartidaLarga = new System.Windows.Forms.RadioButton();
            this.dameRecord = new System.Windows.Forms.RadioButton();
            this.conectar_Btn = new System.Windows.Forms.Button();
            this.desconectar_Btn = new System.Windows.Forms.Button();
            this.nombresConectados = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // enviar_Btn
            // 
            this.enviar_Btn.Location = new System.Drawing.Point(140, 186);
            this.enviar_Btn.Name = "enviar_Btn";
            this.enviar_Btn.Size = new System.Drawing.Size(75, 23);
            this.enviar_Btn.TabIndex = 0;
            this.enviar_Btn.Text = "Enviar";
            this.enviar_Btn.UseVisualStyleBackColor = true;
            this.enviar_Btn.Click += new System.EventHandler(this.enviar_Btn_Click);
            // 
            // usuario
            // 
            this.usuario.Location = new System.Drawing.Point(129, 51);
            this.usuario.Name = "usuario";
            this.usuario.Size = new System.Drawing.Size(100, 20);
            this.usuario.TabIndex = 1;
            // 
            // clave
            // 
            this.clave.Location = new System.Drawing.Point(129, 78);
            this.clave.Name = "clave";
            this.clave.Size = new System.Drawing.Size(100, 20);
            this.clave.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(63, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Usuario";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Contraseña";
            // 
            // registrar
            // 
            this.registrar.AutoSize = true;
            this.registrar.Location = new System.Drawing.Point(129, 118);
            this.registrar.Name = "registrar";
            this.registrar.Size = new System.Drawing.Size(67, 17);
            this.registrar.TabIndex = 5;
            this.registrar.TabStop = true;
            this.registrar.Text = "Registrar";
            this.registrar.UseVisualStyleBackColor = true;
            // 
            // iniciar_sesion
            // 
            this.iniciar_sesion.AutoSize = true;
            this.iniciar_sesion.Location = new System.Drawing.Point(129, 142);
            this.iniciar_sesion.Name = "iniciar_sesion";
            this.iniciar_sesion.Size = new System.Drawing.Size(86, 17);
            this.iniciar_sesion.TabIndex = 6;
            this.iniciar_sesion.TabStop = true;
            this.iniciar_sesion.Text = "Iniciar sesión";
            this.iniciar_sesion.UseVisualStyleBackColor = true;
            // 
            // puntosPerdedores
            // 
            this.puntosPerdedores.AutoSize = true;
            this.puntosPerdedores.Location = new System.Drawing.Point(325, 100);
            this.puntosPerdedores.Name = "puntosPerdedores";
            this.puntosPerdedores.Size = new System.Drawing.Size(337, 17);
            this.puntosPerdedores.TabIndex = 7;
            this.puntosPerdedores.TabStop = true;
            this.puntosPerdedores.Text = "Dame los récord de los jugadores que hayan perdido contra Arnau";
            this.puntosPerdedores.UseVisualStyleBackColor = true;
            // 
            // nombresPartidaLarga
            // 
            this.nombresPartidaLarga.AutoSize = true;
            this.nombresPartidaLarga.Location = new System.Drawing.Point(325, 124);
            this.nombresPartidaLarga.Name = "nombresPartidaLarga";
            this.nombresPartidaLarga.Size = new System.Drawing.Size(296, 17);
            this.nombresPartidaLarga.TabIndex = 8;
            this.nombresPartidaLarga.TabStop = true;
            this.nombresPartidaLarga.Text = "Dame los nombres de los jugadores con partida mas larga";
            this.nombresPartidaLarga.UseVisualStyleBackColor = true;
            // 
            // dameRecord
            // 
            this.dameRecord.AutoSize = true;
            this.dameRecord.Location = new System.Drawing.Point(325, 148);
            this.dameRecord.Name = "dameRecord";
            this.dameRecord.Size = new System.Drawing.Size(148, 17);
            this.dameRecord.TabIndex = 9;
            this.dameRecord.TabStop = true;
            this.dameRecord.Text = "Dame el número de fichas";
            this.dameRecord.UseVisualStyleBackColor = true;
            this.dameRecord.CheckedChanged += new System.EventHandler(this.dameRecord_CheckedChanged);
            // 
            // conectar_Btn
            // 
            this.conectar_Btn.Location = new System.Drawing.Point(48, 186);
            this.conectar_Btn.Name = "conectar_Btn";
            this.conectar_Btn.Size = new System.Drawing.Size(75, 23);
            this.conectar_Btn.TabIndex = 10;
            this.conectar_Btn.Text = "Conectar";
            this.conectar_Btn.UseVisualStyleBackColor = true;
            this.conectar_Btn.Click += new System.EventHandler(this.conectar_Btn_Click);
            // 
            // desconectar_Btn
            // 
            this.desconectar_Btn.Location = new System.Drawing.Point(232, 186);
            this.desconectar_Btn.Name = "desconectar_Btn";
            this.desconectar_Btn.Size = new System.Drawing.Size(84, 23);
            this.desconectar_Btn.TabIndex = 11;
            this.desconectar_Btn.Text = "Desconectar";
            this.desconectar_Btn.UseVisualStyleBackColor = true;
            this.desconectar_Btn.Click += new System.EventHandler(this.desconectar_Btn_Click);
            // 
            // nombresConectados
            // 
            this.nombresConectados.AutoSize = true;
            this.nombresConectados.Location = new System.Drawing.Point(325, 78);
            this.nombresConectados.Name = "nombresConectados";
            this.nombresConectados.Size = new System.Drawing.Size(217, 17);
            this.nombresConectados.TabIndex = 12;
            this.nombresConectados.TabStop = true;
            this.nombresConectados.Text = "Dame la lista de los usuarios conectados";
            this.nombresConectados.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(812, 301);
            this.Controls.Add(this.nombresConectados);
            this.Controls.Add(this.desconectar_Btn);
            this.Controls.Add(this.conectar_Btn);
            this.Controls.Add(this.dameRecord);
            this.Controls.Add(this.nombresPartidaLarga);
            this.Controls.Add(this.puntosPerdedores);
            this.Controls.Add(this.iniciar_sesion);
            this.Controls.Add(this.registrar);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.clave);
            this.Controls.Add(this.usuario);
            this.Controls.Add(this.enviar_Btn);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button enviar_Btn;
        private System.Windows.Forms.TextBox usuario;
        private System.Windows.Forms.TextBox clave;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton registrar;
        private System.Windows.Forms.RadioButton iniciar_sesion;
        private System.Windows.Forms.RadioButton puntosPerdedores;
        private System.Windows.Forms.RadioButton nombresPartidaLarga;
        private System.Windows.Forms.RadioButton dameRecord;
        private System.Windows.Forms.Button conectar_Btn;
        private System.Windows.Forms.Button desconectar_Btn;
        private System.Windows.Forms.RadioButton nombresConectados;
    }
}

