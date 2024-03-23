namespace Ro32
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Status = new Label();
            Logo = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)Logo).BeginInit();
            SuspendLayout();
            // 
            // Status
            // 
            Status.Anchor = AnchorStyles.None;
            Status.BackColor = Color.Transparent;
            Status.Font = new Font("Comfortaa", 20F, FontStyle.Regular, GraphicsUnit.World);
            Status.ForeColor = Color.White;
            Status.Location = new Point(225, 122);
            Status.Margin = new Padding(0);
            Status.Name = "Status";
            Status.Size = new Size(500, 42);
            Status.TabIndex = 0;
            Status.Text = "Waiting for Roblox to launch...";
            Status.TextAlign = ContentAlignment.MiddleCenter;
            Status.Click += label1_Click;
            // 
            // Logo
            // 
            Logo.Anchor = AnchorStyles.None;
            Logo.ImageLocation = "C:\\Users\\bcher\\Documents\\Ro32\\logo.png";
            Logo.Location = new Point(425, 200);
            Logo.Margin = new Padding(0);
            Logo.Name = "Logo";
            Logo.Size = new Size(72, 72);
            Logo.TabIndex = 1;
            Logo.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(23, 23, 23);
            ClientSize = new Size(984, 461);
            Controls.Add(Logo);
            Controls.Add(Status);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)Logo).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Label lbl1;
        private PictureBox pictureBox1;
        private Label Status;
        private PictureBox Logo;
    }
}
