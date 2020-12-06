
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
            this.chatTextBox = new System.Windows.Forms.TextBox();
            this.enviar_Btn = new System.Windows.Forms.Button();
            this.chatLbl = new System.Windows.Forms.Label();
            this.empezarBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // chatTextBox
            // 
            this.chatTextBox.Location = new System.Drawing.Point(416, 312);
            this.chatTextBox.Name = "chatTextBox";
            this.chatTextBox.Size = new System.Drawing.Size(116, 20);
            this.chatTextBox.TabIndex = 6;
            this.chatTextBox.Text = "Escribe algo";
            // 
            // enviar_Btn
            // 
            this.enviar_Btn.Location = new System.Drawing.Point(538, 312);
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
            this.chatLbl.Location = new System.Drawing.Point(413, 9);
            this.chatLbl.Name = "chatLbl";
            this.chatLbl.Size = new System.Drawing.Size(200, 300);
            this.chatLbl.TabIndex = 4;
            this.chatLbl.Text = "label2";
            // 
            // empezarBtn
            // 
            this.empezarBtn.Location = new System.Drawing.Point(152, 150);
            this.empezarBtn.Name = "empezarBtn";
            this.empezarBtn.Size = new System.Drawing.Size(75, 23);
            this.empezarBtn.TabIndex = 7;
            this.empezarBtn.Text = "Empezar";
            this.empezarBtn.UseVisualStyleBackColor = true;
            this.empezarBtn.Click += new System.EventHandler(this.empezarBtn_Click);
            // 
            // NuevaPartida
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(625, 345);
            this.Controls.Add(this.empezarBtn);
            this.Controls.Add(this.chatTextBox);
            this.Controls.Add(this.enviar_Btn);
            this.Controls.Add(this.chatLbl);
            this.Name = "NuevaPartida";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NuevaPartida_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox chatTextBox;
        private System.Windows.Forms.Button enviar_Btn;
        private System.Windows.Forms.Label chatLbl;
        private System.Windows.Forms.Button empezarBtn;

    }
}