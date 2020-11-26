namespace Cliente
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.opcionBtn = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.nombreIn = new System.Windows.Forms.TextBox();
            this.claveIn = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.clave2Lb = new System.Windows.Forms.Label();
            this.clave2In = new System.Windows.Forms.TextBox();
            this.titleLb = new System.Windows.Forms.Label();
            this.enviarBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // opcionBtn
            // 
            this.opcionBtn.Location = new System.Drawing.Point(543, 52);
            this.opcionBtn.Name = "opcionBtn";
            this.opcionBtn.Size = new System.Drawing.Size(170, 40);
            this.opcionBtn.TabIndex = 1;
            this.opcionBtn.Text = "Iniciar Sesión";
            this.opcionBtn.UseVisualStyleBackColor = true;
            this.opcionBtn.Click += new System.EventHandler(this.opcionBtn_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.WindowText;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Window;
            this.label1.Location = new System.Drawing.Point(557, 161);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(143, 20);
            this.label1.TabIndex = 2;
            this.label1.Text = "Nombre de usuario";
            // 
            // nombreIn
            // 
            this.nombreIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.nombreIn.Location = new System.Drawing.Point(543, 184);
            this.nombreIn.Name = "nombreIn";
            this.nombreIn.Size = new System.Drawing.Size(170, 23);
            this.nombreIn.TabIndex = 3;
            // 
            // claveIn
            // 
            this.claveIn.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.claveIn.Location = new System.Drawing.Point(543, 249);
            this.claveIn.Name = "claveIn";
            this.claveIn.Size = new System.Drawing.Size(170, 23);
            this.claveIn.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.SystemColors.WindowText;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Window;
            this.label2.Location = new System.Drawing.Point(578, 226);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Contraseña";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // clave2Lb
            // 
            this.clave2Lb.AutoSize = true;
            this.clave2Lb.BackColor = System.Drawing.SystemColors.WindowText;
            this.clave2Lb.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clave2Lb.ForeColor = System.Drawing.SystemColors.Window;
            this.clave2Lb.Location = new System.Drawing.Point(551, 293);
            this.clave2Lb.Name = "clave2Lb";
            this.clave2Lb.Size = new System.Drawing.Size(156, 20);
            this.clave2Lb.TabIndex = 7;
            this.clave2Lb.Text = "Repite la contraseña";
            this.clave2Lb.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // clave2In
            // 
            this.clave2In.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.clave2In.Location = new System.Drawing.Point(543, 316);
            this.clave2In.Name = "clave2In";
            this.clave2In.Size = new System.Drawing.Size(170, 23);
            this.clave2In.TabIndex = 6;
            // 
            // titleLb
            // 
            this.titleLb.AutoSize = true;
            this.titleLb.BackColor = System.Drawing.SystemColors.WindowText;
            this.titleLb.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.titleLb.ForeColor = System.Drawing.SystemColors.Window;
            this.titleLb.Location = new System.Drawing.Point(581, 119);
            this.titleLb.Name = "titleLb";
            this.titleLb.Size = new System.Drawing.Size(87, 24);
            this.titleLb.TabIndex = 8;
            this.titleLb.Text = "Registro";
            // 
            // enviarBtn
            // 
            this.enviarBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.enviarBtn.Location = new System.Drawing.Point(569, 359);
            this.enviarBtn.Name = "enviarBtn";
            this.enviarBtn.Size = new System.Drawing.Size(125, 31);
            this.enviarBtn.TabIndex = 9;
            this.enviarBtn.Text = "Registrarse";
            this.enviarBtn.UseVisualStyleBackColor = true;
            this.enviarBtn.Click += new System.EventHandler(this.enviarBtn_Click);
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(838, 435);
            this.Controls.Add(this.enviarBtn);
            this.Controls.Add(this.titleLb);
            this.Controls.Add(this.clave2Lb);
            this.Controls.Add(this.clave2In);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.claveIn);
            this.Controls.Add(this.nombreIn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.opcionBtn);
            this.Name = "Login";
            this.Text = "Login";
            this.Load += new System.EventHandler(this.Login_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button opcionBtn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox nombreIn;
        private System.Windows.Forms.TextBox claveIn;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label clave2Lb;
        private System.Windows.Forms.TextBox clave2In;
        private System.Windows.Forms.Label titleLb;
        private System.Windows.Forms.Button enviarBtn;
    }
}