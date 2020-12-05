﻿namespace Cliente
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
            this.desconectar_Btn = new System.Windows.Forms.Button();
            this.ConectadosGrid = new System.Windows.Forms.DataGridView();
            this.nombreJugadorLb = new System.Windows.Forms.Label();
            this.NuevaPartidaBtn = new System.Windows.Forms.Button();
            this.invitar_Btn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ConectadosGrid)).BeginInit();
            this.SuspendLayout();
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
            this.ConectadosGrid.Size = new System.Drawing.Size(176, 322);
            this.ConectadosGrid.TabIndex = 13;
            this.ConectadosGrid.SelectionChanged += new System.EventHandler(this.ConectadosGrid_SelectionChanged);
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
            // invitar_Btn
            // 
            this.invitar_Btn.Location = new System.Drawing.Point(772, 168);
            this.invitar_Btn.Name = "invitar_Btn";
            this.invitar_Btn.Size = new System.Drawing.Size(93, 23);
            this.invitar_Btn.TabIndex = 16;
            this.invitar_Btn.Text = "Invitar selección";
            this.invitar_Btn.UseVisualStyleBackColor = true;
            this.invitar_Btn.Click += new System.EventHandler(this.invitar_Btn_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(919, 346);
            this.Controls.Add(this.invitar_Btn);
            this.Controls.Add(this.NuevaPartidaBtn);
            this.Controls.Add(this.nombreJugadorLb);
            this.Controls.Add(this.ConectadosGrid);
            this.Controls.Add(this.desconectar_Btn);
            this.Name = "Main";
            this.Text = "Main (conectado)";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ConectadosGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button desconectar_Btn;
        private System.Windows.Forms.DataGridView ConectadosGrid;
        private System.Windows.Forms.Label nombreJugadorLb;
        private System.Windows.Forms.Button NuevaPartidaBtn;
        private System.Windows.Forms.Button invitar_Btn;
    }
}

