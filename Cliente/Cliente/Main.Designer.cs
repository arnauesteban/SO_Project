namespace Cliente
{
    partial class Main
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.enviar_Btn = new System.Windows.Forms.Button();
            this.puntosPerdedores = new System.Windows.Forms.RadioButton();
            this.nombresPartidaLarga = new System.Windows.Forms.RadioButton();
            this.dameRecord = new System.Windows.Forms.RadioButton();
            this.desconectar_Btn = new System.Windows.Forms.Button();
            this.ConectadosGrid = new System.Windows.Forms.DataGridView();
            this.nombreJugadorLb = new System.Windows.Forms.Label();
            this.NuevaPartidaBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ConectadosGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // enviar_Btn
            // 
            this.enviar_Btn.Location = new System.Drawing.Point(721, 284);
            this.enviar_Btn.Name = "enviar_Btn";
            this.enviar_Btn.Size = new System.Drawing.Size(75, 23);
            this.enviar_Btn.TabIndex = 0;
            this.enviar_Btn.Text = "Enviar";
            this.enviar_Btn.UseVisualStyleBackColor = true;
            this.enviar_Btn.Click += new System.EventHandler(this.enviar_Btn_Click);
            // 
            // puntosPerdedores
            // 
            this.puntosPerdedores.AutoSize = true;
            this.puntosPerdedores.BackColor = System.Drawing.SystemColors.WindowText;
            this.puntosPerdedores.ForeColor = System.Drawing.SystemColors.Window;
            this.puntosPerdedores.Location = new System.Drawing.Point(459, 200);
            this.puntosPerdedores.Name = "puntosPerdedores";
            this.puntosPerdedores.Size = new System.Drawing.Size(337, 17);
            this.puntosPerdedores.TabIndex = 7;
            this.puntosPerdedores.TabStop = true;
            this.puntosPerdedores.Text = "Dame los récord de los jugadores que hayan perdido contra Arnau";
            this.puntosPerdedores.UseVisualStyleBackColor = false;
            // 
            // nombresPartidaLarga
            // 
            this.nombresPartidaLarga.AutoSize = true;
            this.nombresPartidaLarga.BackColor = System.Drawing.SystemColors.WindowText;
            this.nombresPartidaLarga.ForeColor = System.Drawing.SystemColors.Window;
            this.nombresPartidaLarga.Location = new System.Drawing.Point(459, 224);
            this.nombresPartidaLarga.Name = "nombresPartidaLarga";
            this.nombresPartidaLarga.Size = new System.Drawing.Size(296, 17);
            this.nombresPartidaLarga.TabIndex = 8;
            this.nombresPartidaLarga.TabStop = true;
            this.nombresPartidaLarga.Text = "Dame los nombres de los jugadores con partida mas larga";
            this.nombresPartidaLarga.UseVisualStyleBackColor = false;
            // 
            // dameRecord
            // 
            this.dameRecord.AutoSize = true;
            this.dameRecord.BackColor = System.Drawing.SystemColors.WindowText;
            this.dameRecord.ForeColor = System.Drawing.SystemColors.Window;
            this.dameRecord.Location = new System.Drawing.Point(459, 248);
            this.dameRecord.Name = "dameRecord";
            this.dameRecord.Size = new System.Drawing.Size(148, 17);
            this.dameRecord.TabIndex = 9;
            this.dameRecord.TabStop = true;
            this.dameRecord.Text = "Dame el número de fichas";
            this.dameRecord.UseVisualStyleBackColor = false;
            // 
            // desconectar_Btn
            // 
            this.desconectar_Btn.Location = new System.Drawing.Point(12, 311);
            this.desconectar_Btn.Name = "desconectar_Btn";
            this.desconectar_Btn.Size = new System.Drawing.Size(96, 23);
            this.desconectar_Btn.TabIndex = 11;
            this.desconectar_Btn.Text = "Cerrar sesión";
            this.desconectar_Btn.UseVisualStyleBackColor = true;
            this.desconectar_Btn.Click += new System.EventHandler(this.desconectar_Btn_Click);
            // 
            // ConectadosGrid
            // 
            this.ConectadosGrid.BackgroundColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.ConectadosGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ConectadosGrid.Location = new System.Drawing.Point(731, 12);
            this.ConectadosGrid.Name = "ConectadosGrid";
            this.ConectadosGrid.Size = new System.Drawing.Size(176, 150);
            this.ConectadosGrid.TabIndex = 13;
            // 
            // nombreJugadorLb
            // 
            this.nombreJugadorLb.AutoSize = true;
            this.nombreJugadorLb.BackColor = System.Drawing.SystemColors.WindowText;
            this.nombreJugadorLb.ForeColor = System.Drawing.SystemColors.Window;
            this.nombreJugadorLb.Location = new System.Drawing.Point(12, 12);
            this.nombreJugadorLb.Name = "nombreJugadorLb";
            this.nombreJugadorLb.Size = new System.Drawing.Size(45, 13);
            this.nombreJugadorLb.TabIndex = 14;
            this.nombreJugadorLb.Text = "Jugador";
            // 
            // NuevaPartidaBtn
            // 
            this.NuevaPartidaBtn.Location = new System.Drawing.Point(622, 12);
            this.NuevaPartidaBtn.Name = "NuevaPartidaBtn";
            this.NuevaPartidaBtn.Size = new System.Drawing.Size(75, 23);
            this.NuevaPartidaBtn.TabIndex = 15;
            this.NuevaPartidaBtn.Text = "Nueva Partida";
            this.NuevaPartidaBtn.UseVisualStyleBackColor = true;
            this.NuevaPartidaBtn.Click += new System.EventHandler(this.NuevaPartidaBtn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(919, 346);
            this.Controls.Add(this.NuevaPartidaBtn);
            this.Controls.Add(this.nombreJugadorLb);
            this.Controls.Add(this.ConectadosGrid);
            this.Controls.Add(this.desconectar_Btn);
            this.Controls.Add(this.dameRecord);
            this.Controls.Add(this.nombresPartidaLarga);
            this.Controls.Add(this.puntosPerdedores);
            this.Controls.Add(this.enviar_Btn);
            this.Name = "Main";
            this.Text = "Main (conectado)";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ConectadosGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button enviar_Btn;
        private System.Windows.Forms.RadioButton puntosPerdedores;
        private System.Windows.Forms.RadioButton nombresPartidaLarga;
        private System.Windows.Forms.RadioButton dameRecord;
        private System.Windows.Forms.Button desconectar_Btn;
        private System.Windows.Forms.DataGridView ConectadosGrid;
        private System.Windows.Forms.Label nombreJugadorLb;
        private System.Windows.Forms.Button NuevaPartidaBtn;
    }
}

