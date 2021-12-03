
namespace DerekWare.Iris
{
    partial class ConnectDeviceDialog
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
            if(disposing && (components != null))
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
            this.IpAddressTextBox = new System.Windows.Forms.TextBox();
            this.YupButton = new System.Windows.Forms.Button();
            this.NopeButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(440, 88);
            this.label1.TabIndex = 0;
            this.label1.Text = "Not all devices respond to a device discovery request. If one of your devices isn" +
    "\'t found automatically, we can try to connect to it manually by IP address.";
            // 
            // IpAddressTextBox
            // 
            this.IpAddressTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IpAddressTextBox.Location = new System.Drawing.Point(13, 106);
            this.IpAddressTextBox.Name = "IpAddressTextBox";
            this.IpAddressTextBox.Size = new System.Drawing.Size(438, 26);
            this.IpAddressTextBox.TabIndex = 1;
            this.IpAddressTextBox.TextChanged += new System.EventHandler(this.IpAddressTextBox_TextChanged);
            // 
            // YupButton
            // 
            this.YupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YupButton.Location = new System.Drawing.Point(207, 163);
            this.YupButton.Name = "YupButton";
            this.YupButton.Size = new System.Drawing.Size(120, 40);
            this.YupButton.TabIndex = 4;
            this.YupButton.Text = "OK";
            this.YupButton.UseVisualStyleBackColor = true;
            this.YupButton.Click += new System.EventHandler(this.YupButton_Click);
            // 
            // NopeButton
            // 
            this.NopeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.NopeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.NopeButton.Location = new System.Drawing.Point(333, 163);
            this.NopeButton.Name = "NopeButton";
            this.NopeButton.Size = new System.Drawing.Size(120, 40);
            this.NopeButton.TabIndex = 5;
            this.NopeButton.Text = "Cancel";
            this.NopeButton.UseVisualStyleBackColor = true;
            this.NopeButton.Click += new System.EventHandler(this.NopeButton_Click);
            // 
            // ConnectDeviceDialog
            // 
            this.AcceptButton = this.YupButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.NopeButton;
            this.ClientSize = new System.Drawing.Size(465, 215);
            this.Controls.Add(this.YupButton);
            this.Controls.Add(this.NopeButton);
            this.Controls.Add(this.IpAddressTextBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectDeviceDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect to LIFX Device";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox IpAddressTextBox;
        private System.Windows.Forms.Button YupButton;
        private System.Windows.Forms.Button NopeButton;
    }
}