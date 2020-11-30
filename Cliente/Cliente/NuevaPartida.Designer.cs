
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
            this.JugadoresUnidosGrid = new System.Windows.Forms.DataGridView();
            this.label1 = new System.Windows.Forms.Label();
            this.invitadosIn = new System.Windows.Forms.TextBox();
            this.InvitarBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.JugadoresUnidosGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // JugadoresUnidosGrid
            // 
            this.JugadoresUnidosGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.JugadoresUnidosGrid.Location = new System.Drawing.Point(373, 12);
            this.JugadoresUnidosGrid.Name = "JugadoresUnidosGrid";
            this.JugadoresUnidosGrid.Size = new System.Drawing.Size(240, 150);
            this.JugadoresUnidosGrid.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Invitar a:";
            // 
            // invitadosIn
            // 
            this.invitadosIn.Location = new System.Drawing.Point(16, 30);
            this.invitadosIn.Name = "invitadosIn";
            this.invitadosIn.Size = new System.Drawing.Size(137, 20);
            this.invitadosIn.TabIndex = 2;
            // 
            // InvitarBtn
            // 
            this.InvitarBtn.Location = new System.Drawing.Point(16, 56);
            this.InvitarBtn.Name = "InvitarBtn";
            this.InvitarBtn.Size = new System.Drawing.Size(75, 23);
            this.InvitarBtn.TabIndex = 3;
            this.InvitarBtn.Text = "Invitar";
            this.InvitarBtn.UseVisualStyleBackColor = true;
            this.InvitarBtn.Click += new System.EventHandler(this.InvitarBtn_Click);
            // 
            // NuevaPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 295);
            this.Controls.Add(this.InvitarBtn);
            this.Controls.Add(this.invitadosIn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.JugadoresUnidosGrid);
            this.Name = "NuevaPartida";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.NuevaPartida_Load);
            ((System.ComponentModel.ISupportInitialize)(this.JugadoresUnidosGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView JugadoresUnidosGrid;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox invitadosIn;
        private System.Windows.Forms.Button InvitarBtn;
    }
}