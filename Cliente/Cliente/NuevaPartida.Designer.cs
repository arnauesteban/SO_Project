
namespace Cliente
{
    partial class NuevaPartida
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NuevaPartida));
            this.chatTextBox = new System.Windows.Forms.TextBox();
            this.enviar_Btn = new System.Windows.Forms.Button();
            this.chatLbl = new System.Windows.Forms.Label();
            this.empezarBtn = new System.Windows.Forms.Button();
            this.usuarioLbl = new System.Windows.Forms.Label();
            this.pedirBtn = new System.Windows.Forms.Button();
            this.plantarseBtn = new System.Windows.Forms.Button();
            this.apostarBtn = new System.Windows.Forms.Button();
            this.apostarNum = new System.Windows.Forms.NumericUpDown();
            this.rendirseBtn = new System.Windows.Forms.Button();
            this.jugador1Carta3Lbl = new System.Windows.Forms.Label();
            this.jugador1Carta4Lbl = new System.Windows.Forms.Label();
            this.jugador1Carta2Lbl = new System.Windows.Forms.Label();
            this.jugador1Carta1Lbl = new System.Windows.Forms.Label();
            this.jugador1PuntosLbl = new System.Windows.Forms.Label();
            this.jugador1JugadoLbl = new System.Windows.Forms.Label();
            this.jugador1FichasLbl = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.apostarNum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // chatTextBox
            // 
            this.chatTextBox.Location = new System.Drawing.Point(587, 382);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.Size = new System.Drawing.Size(116, 20);
            this.chatTextBox.TabIndex = 6;
            this.chatTextBox.Text = "Escribe algo";
            this.chatTextBox.Click += new System.EventHandler(this.chatTextBox_Click);
            // 
            // enviar_Btn
            // 
            this.enviar_Btn.Location = new System.Drawing.Point(709, 382);
            this.enviar_Btn.Name = "enviar_Btn";
            this.enviar_Btn.Size = new System.Drawing.Size(75, 22);
            this.enviar_Btn.TabIndex = 5;
            this.enviar_Btn.Text = "Enviar";
            this.enviar_Btn.UseVisualStyleBackColor = true;
            this.enviar_Btn.Click += new System.EventHandler(this.enviar_Btn_Click);
            // 
            // chatLbl
            // 
            this.chatLbl.BackColor = System.Drawing.Color.Black;
            this.chatLbl.ForeColor = System.Drawing.Color.White;
            this.chatLbl.Location = new System.Drawing.Point(584, -1);
            this.chatLbl.Name = "chatLbl";
            this.chatLbl.Size = new System.Drawing.Size(200, 380);
            this.chatLbl.TabIndex = 4;
            this.chatLbl.Text = "Chat";
            // 
            // empezarBtn
            // 
            this.empezarBtn.Location = new System.Drawing.Point(490, 8);
            this.empezarBtn.Name = "empezarBtn";
            this.empezarBtn.Size = new System.Drawing.Size(88, 35);
            this.empezarBtn.TabIndex = 7;
            this.empezarBtn.Text = "Empezar";
            this.empezarBtn.UseVisualStyleBackColor = true;
            this.empezarBtn.Click += new System.EventHandler(this.empezarBtn_Click);
            // 
            // usuarioLbl
            // 
            this.usuarioLbl.AutoSize = true;
            this.usuarioLbl.Location = new System.Drawing.Point(13, 13);
            this.usuarioLbl.Name = "usuarioLbl";
            this.usuarioLbl.Size = new System.Drawing.Size(45, 13);
            this.usuarioLbl.TabIndex = 8;
            this.usuarioLbl.Text = "Jugador";
            // 
            // pedirBtn
            // 
            this.pedirBtn.Location = new System.Drawing.Point(490, 62);
            this.pedirBtn.Name = "pedirBtn";
            this.pedirBtn.Size = new System.Drawing.Size(75, 23);
            this.pedirBtn.TabIndex = 58;
            this.pedirBtn.Text = "Pedir carta";
            this.pedirBtn.UseVisualStyleBackColor = true;
            this.pedirBtn.Click += new System.EventHandler(this.pedirBtn_Click);
            // 
            // plantarseBtn
            // 
            this.plantarseBtn.Location = new System.Drawing.Point(490, 108);
            this.plantarseBtn.Name = "plantarseBtn";
            this.plantarseBtn.Size = new System.Drawing.Size(75, 23);
            this.plantarseBtn.TabIndex = 59;
            this.plantarseBtn.Text = "Plantarse";
            this.plantarseBtn.UseVisualStyleBackColor = true;
            this.plantarseBtn.Click += new System.EventHandler(this.plantarseBtn_Click);
            // 
            // apostarBtn
            // 
            this.apostarBtn.Location = new System.Drawing.Point(490, 146);
            this.apostarBtn.Name = "apostarBtn";
            this.apostarBtn.Size = new System.Drawing.Size(75, 23);
            this.apostarBtn.TabIndex = 60;
            this.apostarBtn.Text = "Apostar";
            this.apostarBtn.UseVisualStyleBackColor = true;
            this.apostarBtn.Click += new System.EventHandler(this.apostarBtn_Click);
            // 
            // apostarNum
            // 
            this.apostarNum.Location = new System.Drawing.Point(440, 149);
            this.apostarNum.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.apostarNum.Name = "apostarNum";
            this.apostarNum.Size = new System.Drawing.Size(44, 20);
            this.apostarNum.TabIndex = 61;
            this.apostarNum.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // rendirseBtn
            // 
            this.rendirseBtn.Location = new System.Drawing.Point(490, 200);
            this.rendirseBtn.Name = "rendirseBtn";
            this.rendirseBtn.Size = new System.Drawing.Size(75, 23);
            this.rendirseBtn.TabIndex = 62;
            this.rendirseBtn.Text = "Rendirse";
            this.rendirseBtn.UseVisualStyleBackColor = true;
            this.rendirseBtn.Click += new System.EventHandler(this.rendirseBtn_Click);
            // 
            // jugador1Carta3Lbl
            // 
            this.jugador1Carta3Lbl.AutoSize = true;
            this.jugador1Carta3Lbl.Location = new System.Drawing.Point(30, 169);
            this.jugador1Carta3Lbl.Name = "jugador1Carta3Lbl";
            this.jugador1Carta3Lbl.Size = new System.Drawing.Size(41, 13);
            this.jugador1Carta3Lbl.TabIndex = 71;
            this.jugador1Carta3Lbl.Text = "Carta 3";
            // 
            // jugador1Carta4Lbl
            // 
            this.jugador1Carta4Lbl.AutoSize = true;
            this.jugador1Carta4Lbl.Location = new System.Drawing.Point(77, 169);
            this.jugador1Carta4Lbl.Name = "jugador1Carta4Lbl";
            this.jugador1Carta4Lbl.Size = new System.Drawing.Size(41, 13);
            this.jugador1Carta4Lbl.TabIndex = 79;
            this.jugador1Carta4Lbl.Text = "Carta 4";
            // 
            // jugador1Carta2Lbl
            // 
            this.jugador1Carta2Lbl.AutoSize = true;
            this.jugador1Carta2Lbl.Location = new System.Drawing.Point(76, 146);
            this.jugador1Carta2Lbl.Name = "jugador1Carta2Lbl";
            this.jugador1Carta2Lbl.Size = new System.Drawing.Size(41, 13);
            this.jugador1Carta2Lbl.TabIndex = 22;
            this.jugador1Carta2Lbl.Text = "Carta 2";
            // 
            // jugador1Carta1Lbl
            // 
            this.jugador1Carta1Lbl.AutoSize = true;
            this.jugador1Carta1Lbl.Location = new System.Drawing.Point(29, 146);
            this.jugador1Carta1Lbl.Name = "jugador1Carta1Lbl";
            this.jugador1Carta1Lbl.Size = new System.Drawing.Size(41, 13);
            this.jugador1Carta1Lbl.TabIndex = 21;
            this.jugador1Carta1Lbl.Text = "Carta 1";
            // 
            // jugador1PuntosLbl
            // 
            this.jugador1PuntosLbl.AutoSize = true;
            this.jugador1PuntosLbl.Location = new System.Drawing.Point(30, 118);
            this.jugador1PuntosLbl.Name = "jugador1PuntosLbl";
            this.jugador1PuntosLbl.Size = new System.Drawing.Size(13, 13);
            this.jugador1PuntosLbl.TabIndex = 63;
            this.jugador1PuntosLbl.Text = "0";
            // 
            // jugador1JugadoLbl
            // 
            this.jugador1JugadoLbl.AutoSize = true;
            this.jugador1JugadoLbl.Location = new System.Drawing.Point(29, 95);
            this.jugador1JugadoLbl.Name = "jugador1JugadoLbl";
            this.jugador1JugadoLbl.Size = new System.Drawing.Size(13, 13);
            this.jugador1JugadoLbl.TabIndex = 20;
            this.jugador1JugadoLbl.Text = "0";
            // 
            // jugador1FichasLbl
            // 
            this.jugador1FichasLbl.AutoSize = true;
            this.jugador1FichasLbl.Location = new System.Drawing.Point(30, 72);
            this.jugador1FichasLbl.Name = "jugador1FichasLbl";
            this.jugador1FichasLbl.Size = new System.Drawing.Size(13, 13);
            this.jugador1FichasLbl.TabIndex = 19;
            this.jugador1FichasLbl.Text = "0";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(0, 285);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(100, 140);
            this.pictureBox1.TabIndex = 80;
            this.pictureBox1.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox2.Location = new System.Drawing.Point(105, 285);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(100, 140);
            this.pictureBox2.TabIndex = 81;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox3.Location = new System.Drawing.Point(210, 285);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(100, 140);
            this.pictureBox3.TabIndex = 82;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox4.Location = new System.Drawing.Point(315, 285);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(100, 140);
            this.pictureBox4.TabIndex = 83;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox5.Location = new System.Drawing.Point(420, 285);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(100, 140);
            this.pictureBox5.TabIndex = 84;
            this.pictureBox5.TabStop = false;
            // 
            // NuevaPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(784, 424);
            this.Controls.Add(this.pictureBox5);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pictureBox3);
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.jugador1Carta4Lbl);
            this.Controls.Add(this.jugador1Carta3Lbl);
            this.Controls.Add(this.jugador1PuntosLbl);
            this.Controls.Add(this.rendirseBtn);
            this.Controls.Add(this.apostarNum);
            this.Controls.Add(this.apostarBtn);
            this.Controls.Add(this.plantarseBtn);
            this.Controls.Add(this.pedirBtn);
            this.Controls.Add(this.jugador1Carta2Lbl);
            this.Controls.Add(this.jugador1Carta1Lbl);
            this.Controls.Add(this.jugador1JugadoLbl);
            this.Controls.Add(this.jugador1FichasLbl);
            this.Controls.Add(this.usuarioLbl);
            this.Controls.Add(this.empezarBtn);
            this.Controls.Add(this.chatTextBox);
            this.Controls.Add(this.enviar_Btn);
            this.Controls.Add(this.chatLbl);
            this.Name = "NuevaPartida";
            this.Text = "Partida";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NuevaPartida_FormClosing);
            this.Load += new System.EventHandler(this.NuevaPartida_Load);
            ((System.ComponentModel.ISupportInitialize)(this.apostarNum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatTextBox;
        private System.Windows.Forms.Button enviar_Btn;
        private System.Windows.Forms.Label chatLbl;
        private System.Windows.Forms.Button empezarBtn;
        private System.Windows.Forms.Label usuarioLbl;
        private System.Windows.Forms.Button pedirBtn;
        private System.Windows.Forms.Button plantarseBtn;
        private System.Windows.Forms.Button apostarBtn;
        private System.Windows.Forms.NumericUpDown apostarNum;
        private System.Windows.Forms.Button rendirseBtn;
        private System.Windows.Forms.Label jugador1Carta3Lbl;
        private System.Windows.Forms.Label jugador1Carta4Lbl;
        private System.Windows.Forms.Label jugador1Carta2Lbl;
        private System.Windows.Forms.Label jugador1Carta1Lbl;
        private System.Windows.Forms.Label jugador1PuntosLbl;
        private System.Windows.Forms.Label jugador1JugadoLbl;
        private System.Windows.Forms.Label jugador1FichasLbl;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.PictureBox pictureBox5;
    }
}