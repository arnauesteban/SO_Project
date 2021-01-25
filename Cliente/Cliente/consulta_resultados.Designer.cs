namespace Cliente
{
    partial class consulta_resultados
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
            this.buscarBtn = new System.Windows.Forms.Button();
            this.cancelarBtn = new System.Windows.Forms.Button();
            this.nombreIn = new System.Windows.Forms.TextBox();
            this.consulta1Btn = new System.Windows.Forms.RadioButton();
            this.jugadorLbl = new System.Windows.Forms.Label();
            this.consulta2Btn = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // buscarBtn
            // 
            this.buscarBtn.Location = new System.Drawing.Point(33, 186);
            this.buscarBtn.Name = "buscarBtn";
            this.buscarBtn.Size = new System.Drawing.Size(75, 23);
            this.buscarBtn.TabIndex = 0;
            this.buscarBtn.Text = "Buscar";
            this.buscarBtn.UseVisualStyleBackColor = true;
            this.buscarBtn.Click += new System.EventHandler(this.buscarBtn_Click);
            // 
            // cancelarBtn
            // 
            this.cancelarBtn.Location = new System.Drawing.Point(163, 186);
            this.cancelarBtn.Name = "cancelarBtn";
            this.cancelarBtn.Size = new System.Drawing.Size(75, 23);
            this.cancelarBtn.TabIndex = 1;
            this.cancelarBtn.Text = "Cancelar";
            this.cancelarBtn.UseVisualStyleBackColor = true;
            this.cancelarBtn.Click += new System.EventHandler(this.cancelarBtn_Click);
            // 
            // nombreIn
            // 
            this.nombreIn.Location = new System.Drawing.Point(117, 117);
            this.nombreIn.Name = "nombreIn";
            this.nombreIn.Size = new System.Drawing.Size(100, 20);
            this.nombreIn.TabIndex = 2;
            // 
            // consulta1Btn
            // 
            this.consulta1Btn.AutoSize = true;
            this.consulta1Btn.Location = new System.Drawing.Point(13, 13);
            this.consulta1Btn.Name = "consulta1Btn";
            this.consulta1Btn.Size = new System.Drawing.Size(249, 17);
            this.consulta1Btn.TabIndex = 3;
            this.consulta1Btn.TabStop = true;
            this.consulta1Btn.Text = "Registro de partidas en común con otro jugador";
            this.consulta1Btn.UseVisualStyleBackColor = true;
            this.consulta1Btn.CheckedChanged += new System.EventHandler(this.consulta1Btn_CheckedChanged);
            // 
            // jugadorLbl
            // 
            this.jugadorLbl.AutoSize = true;
            this.jugadorLbl.Location = new System.Drawing.Point(64, 120);
            this.jugadorLbl.Name = "jugadorLbl";
            this.jugadorLbl.Size = new System.Drawing.Size(47, 13);
            this.jugadorLbl.TabIndex = 4;
            this.jugadorLbl.Text = "Nombre:";
            // 
            // consulta2Btn
            // 
            this.consulta2Btn.AutoSize = true;
            this.consulta2Btn.Location = new System.Drawing.Point(13, 36);
            this.consulta2Btn.Name = "consulta2Btn";
            this.consulta2Btn.Size = new System.Drawing.Size(183, 17);
            this.consulta2Btn.TabIndex = 5;
            this.consulta2Btn.TabStop = true;
            this.consulta2Btn.Text = "Jugadores de la partida más larga";
            this.consulta2Btn.UseVisualStyleBackColor = true;
            this.consulta2Btn.CheckedChanged += new System.EventHandler(this.consulta2Btn_CheckedChanged);
            // 
            // consulta_resultados
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.consulta2Btn);
            this.Controls.Add(this.jugadorLbl);
            this.Controls.Add(this.consulta1Btn);
            this.Controls.Add(this.nombreIn);
            this.Controls.Add(this.cancelarBtn);
            this.Controls.Add(this.buscarBtn);
            this.Name = "consulta_resultados";
            this.Text = "Consultar estadísticas";
            this.Load += new System.EventHandler(this.consulta_resultados_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buscarBtn;
        private System.Windows.Forms.Button cancelarBtn;
        private System.Windows.Forms.TextBox nombreIn;
        private System.Windows.Forms.RadioButton consulta1Btn;
        private System.Windows.Forms.Label jugadorLbl;
        private System.Windows.Forms.RadioButton consulta2Btn;
    }
}