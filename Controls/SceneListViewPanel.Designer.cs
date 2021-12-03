
namespace DerekWare.Iris
{
    partial class SceneListViewPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.SceneListView = new DerekWare.Iris.SceneListView();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.DescriptionLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.SceneListView, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1768, 1186);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // SceneListView
            // 
            this.SceneListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SceneListView.FullRowSelect = true;
            this.SceneListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.SceneListView.HideSelection = false;
            this.SceneListView.LabelWrap = false;
            this.SceneListView.Location = new System.Drawing.Point(3, 23);
            this.SceneListView.Name = "SceneListView";
            this.SceneListView.Scene = null;
            this.SceneListView.Size = new System.Drawing.Size(1762, 1160);
            this.SceneListView.TabIndex = 0;
            this.SceneListView.UseCompatibleStateImageBehavior = false;
            this.SceneListView.View = System.Windows.Forms.View.Details;
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Location = new System.Drawing.Point(3, 0);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(0, 20);
            this.DescriptionLabel.TabIndex = 2;
            this.DescriptionLabel.Visible = false;
            // 
            // SceneListViewPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "SceneListViewPanel";
            this.Size = new System.Drawing.Size(1768, 1186);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private SceneListView SceneListView;
        private System.Windows.Forms.Label DescriptionLabel;
    }
}
