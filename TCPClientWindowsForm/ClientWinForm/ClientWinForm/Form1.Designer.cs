namespace ClientWinForm
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
            this.LblClient = new System.Windows.Forms.Label();
            this.LblVeri = new System.Windows.Forms.Label();
            this.LblRef = new System.Windows.Forms.Label();
            this.txtClient = new System.Windows.Forms.TextBox();
            this.txtVeri = new System.Windows.Forms.TextBox();
            this.cmbislem = new System.Windows.Forms.ComboBox();
            this.btnGonder = new System.Windows.Forms.Button();
            this.LblMesaj = new System.Windows.Forms.Label();
            this.LblIp = new System.Windows.Forms.Label();
            this.txtip = new System.Windows.Forms.TextBox();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.LblMesajFile = new System.Windows.Forms.Label();
            this.btnFileGonder = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // LblClient
            // 
            this.LblClient.AutoSize = true;
            this.LblClient.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LblClient.Location = new System.Drawing.Point(73, 67);
            this.LblClient.Name = "LblClient";
            this.LblClient.Size = new System.Drawing.Size(85, 18);
            this.LblClient.TabIndex = 0;
            this.LblClient.Text = "Client Adı   :";
            // 
            // LblVeri
            // 
            this.LblVeri.AutoSize = true;
            this.LblVeri.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LblVeri.Location = new System.Drawing.Point(73, 94);
            this.LblVeri.Name = "LblVeri";
            this.LblVeri.Size = new System.Drawing.Size(85, 18);
            this.LblVeri.TabIndex = 1;
            this.LblVeri.Text = "Veri            :";
            // 
            // LblRef
            // 
            this.LblRef.AutoSize = true;
            this.LblRef.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LblRef.Location = new System.Drawing.Point(73, 121);
            this.LblRef.Name = "LblRef";
            this.LblRef.Size = new System.Drawing.Size(86, 18);
            this.LblRef.TabIndex = 1;
            this.LblRef.Text = "İşlem Tipi   :";
            // 
            // txtClient
            // 
            this.txtClient.Location = new System.Drawing.Point(199, 65);
            this.txtClient.Name = "txtClient";
            this.txtClient.Size = new System.Drawing.Size(175, 20);
            this.txtClient.TabIndex = 3;
            // 
            // txtVeri
            // 
            this.txtVeri.Location = new System.Drawing.Point(199, 91);
            this.txtVeri.Name = "txtVeri";
            this.txtVeri.Size = new System.Drawing.Size(175, 20);
            this.txtVeri.TabIndex = 4;
            // 
            // cmbislem
            // 
            this.cmbislem.FormattingEnabled = true;
            this.cmbislem.Items.AddRange(new object[] {
            "Ekle",
            "Guncelle",
            "Sil"});
            this.cmbislem.Location = new System.Drawing.Point(199, 117);
            this.cmbislem.Name = "cmbislem";
            this.cmbislem.Size = new System.Drawing.Size(175, 21);
            this.cmbislem.TabIndex = 5;
            // 
            // btnGonder
            // 
            this.btnGonder.Location = new System.Drawing.Point(282, 144);
            this.btnGonder.Name = "btnGonder";
            this.btnGonder.Size = new System.Drawing.Size(92, 35);
            this.btnGonder.TabIndex = 6;
            this.btnGonder.Text = "GÖNDER";
            this.btnGonder.UseVisualStyleBackColor = true;
            this.btnGonder.Click += new System.EventHandler(this.btnGonder_Click);
            // 
            // LblMesaj
            // 
            this.LblMesaj.AutoSize = true;
            this.LblMesaj.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LblMesaj.Location = new System.Drawing.Point(245, 267);
            this.LblMesaj.Name = "LblMesaj";
            this.LblMesaj.Size = new System.Drawing.Size(0, 20);
            this.LblMesaj.TabIndex = 6;
            // 
            // LblIp
            // 
            this.LblIp.AutoSize = true;
            this.LblIp.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LblIp.Location = new System.Drawing.Point(76, 13);
            this.LblIp.Name = "LblIp";
            this.LblIp.Size = new System.Drawing.Size(81, 18);
            this.LblIp.TabIndex = 7;
            this.LblIp.Text = "IP              :";
            // 
            // txtip
            // 
            this.txtip.Location = new System.Drawing.Point(199, 13);
            this.txtip.Name = "txtip";
            this.txtip.Size = new System.Drawing.Size(175, 20);
            this.txtip.TabIndex = 0;
            this.txtip.Text = "192.168.0.149";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.lblPort.Location = new System.Drawing.Point(73, 38);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(84, 18);
            this.lblPort.TabIndex = 9;
            this.lblPort.Text = "Port           :";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(199, 39);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(175, 20);
            this.txtPort.TabIndex = 1;
            this.txtPort.Text = "5555";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(454, 13);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(88, 34);
            this.button1.TabIndex = 10;
            this.button1.Text = "DOSYA SEÇ";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LblMesajFile
            // 
            this.LblMesajFile.AutoSize = true;
            this.LblMesajFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(162)));
            this.LblMesajFile.Location = new System.Drawing.Point(451, 55);
            this.LblMesajFile.Name = "LblMesajFile";
            this.LblMesajFile.Size = new System.Drawing.Size(0, 18);
            this.LblMesajFile.TabIndex = 11;
            // 
            // btnFileGonder
            // 
            this.btnFileGonder.Location = new System.Drawing.Point(548, 13);
            this.btnFileGonder.Name = "btnFileGonder";
            this.btnFileGonder.Size = new System.Drawing.Size(88, 34);
            this.btnFileGonder.TabIndex = 12;
            this.btnFileGonder.Text = "DOSYA GÖNDER";
            this.btnFileGonder.UseVisualStyleBackColor = true;
            this.btnFileGonder.Click += new System.EventHandler(this.btnFileGonder_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(658, 13);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(120, 355);
            this.listBox1.TabIndex = 13;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(803, 400);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.btnFileGonder);
            this.Controls.Add(this.LblMesajFile);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtip);
            this.Controls.Add(this.LblIp);
            this.Controls.Add(this.LblMesaj);
            this.Controls.Add(this.btnGonder);
            this.Controls.Add(this.cmbislem);
            this.Controls.Add(this.txtVeri);
            this.Controls.Add(this.txtClient);
            this.Controls.Add(this.LblRef);
            this.Controls.Add(this.LblVeri);
            this.Controls.Add(this.LblClient);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label LblClient;
        private System.Windows.Forms.Label LblVeri;
        private System.Windows.Forms.Label LblRef;
        private System.Windows.Forms.TextBox txtClient;
        private System.Windows.Forms.TextBox txtVeri;
        private System.Windows.Forms.ComboBox cmbislem;
        private System.Windows.Forms.Button btnGonder;
        private System.Windows.Forms.Label LblMesaj;
        private System.Windows.Forms.Label LblIp;
        private System.Windows.Forms.TextBox txtip;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label LblMesajFile;
        private System.Windows.Forms.Button btnFileGonder;
        private System.Windows.Forms.ListBox listBox1;
    }
}

