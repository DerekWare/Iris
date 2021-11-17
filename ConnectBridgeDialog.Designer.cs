
namespace DerekWare.Iris
{
    partial class ConnectBridgeDialog
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
            this.YupButton = new System.Windows.Forms.Button();
            this.NopeButton = new System.Windows.Forms.Button();
            this.IpAddressComboBox = new System.Windows.Forms.ComboBox();
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
            this.label1.Text = "Select or enter the IP address of your Hue bridge below, press the button on the " +
    "bridge, then click OK.";
            // 
            // YupButton
            // 
            this.YupButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.YupButton.Location = new System.Drawing.Point(207, 163);
            this.YupButton.Name = "YupButton";
            this.YupButton.Size = new System.Drawing.Size(120, 40);
            this.YupButton.TabIndex = 2;
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
            this.NopeButton.TabIndex = 3;
            this.NopeButton.Text = "Cancel";
            this.NopeButton.UseVisualStyleBackColor = true;
            this.NopeButton.Click += new System.EventHandler(this.NopeButton_Click);
            // 
            // IpAddressComboBox
            // 
            this.IpAddressComboBox.FormattingEnabled = true;
            this.IpAddressComboBox.Location = new System.Drawing.Point(12, 100);
            this.IpAddressComboBox.Name = "IpAddressComboBox";
            this.IpAddressComboBox.Size = new System.Drawing.Size(441, 28);
            this.IpAddressComboBox.TabIndex = 1;
            this.IpAddressComboBox.SelectedIndexChanged += new System.EventHandler(this.IpAddressComboBox_SelectedIndexChanged);
            this.IpAddressComboBox.TextUpdate += new System.EventHandler(this.IpAddressComboBox_TextUpdate);
            // 
            // ConnectBridgeDialog
            // 
            this.AcceptButton = this.YupButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.NopeButton;
            this.ClientSize = new System.Drawing.Size(465, 215);
            this.Controls.Add(this.IpAddressComboBox);
            this.Controls.Add(this.YupButton);
            this.Controls.Add(this.NopeButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectBridgeDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Connect to Hue Bridge";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ConnectBridgeDialog_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button YupButton;
        private System.Windows.Forms.Button NopeButton;
        private System.Windows.Forms.ComboBox IpAddressComboBox;
    }
}