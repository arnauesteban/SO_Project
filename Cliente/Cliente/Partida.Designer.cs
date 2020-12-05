namespace Cliente
{
    partial class Partida
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
            this.label1 = new System.Windows.Forms.Label();
            this.enviar_Btn = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(695, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(200, 300);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // enviar_Btn
            // 
            this.enviar_Btn.Location = new System.Drawing.Point(820, 310);
            this.enviar_Btn.Name = "enviar_Btn";
            this.enviar_Btn.Size = new System.Drawing.Size(75, 22);
            this.enviar_Btn.TabIndex = 1;
            this.enviar_Btn.Text = "Enviar";
            this.enviar_Btn.UseVisualStyleBackColor = true;
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(698, 312);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(116, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "Escribe algo";
            // 
            // Partida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(907, 353);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.enviar_Btn);
            this.Controls.Add(this.label1);
            this.Name = "Partida";
            this.Text = "Partida";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button enviar_Btn;
        private System.Windows.Forms.TextBox textBox1;
    }
}