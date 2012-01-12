namespace LeMondCsvToTcxConverter
{
    partial class MainForms
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
            this.btnCreateTcx = new System.Windows.Forms.Button();
            this.lstFiles = new System.Windows.Forms.ListBox();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnRemoveAll = new System.Windows.Forms.Button();
            this.chkUseLocalTime = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnCreateTcx
            // 
            this.btnCreateTcx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateTcx.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnCreateTcx.Location = new System.Drawing.Point(414, 218);
            this.btnCreateTcx.Name = "btnCreateTcx";
            this.btnCreateTcx.Size = new System.Drawing.Size(85, 23);
            this.btnCreateTcx.TabIndex = 3;
            this.btnCreateTcx.Text = "Create Tcx...";
            this.btnCreateTcx.UseVisualStyleBackColor = true;
            this.btnCreateTcx.Click += new System.EventHandler(this.btnCreateTcx_Click);
            // 
            // lstFiles
            // 
            this.lstFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lstFiles.FormattingEnabled = true;
            this.lstFiles.Location = new System.Drawing.Point(9, 46);
            this.lstFiles.Name = "lstFiles";
            this.lstFiles.Size = new System.Drawing.Size(571, 147);
            this.lstFiles.TabIndex = 2;
            // 
            // btnAddFile
            // 
            this.btnAddFile.Location = new System.Drawing.Point(12, 12);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(75, 23);
            this.btnAddFile.TabIndex = 0;
            this.btnAddFile.Text = "Add File...";
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(505, 218);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnRemoveAll
            // 
            this.btnRemoveAll.Location = new System.Drawing.Point(93, 12);
            this.btnRemoveAll.Name = "btnRemoveAll";
            this.btnRemoveAll.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveAll.TabIndex = 1;
            this.btnRemoveAll.Text = "Remove All";
            this.btnRemoveAll.UseVisualStyleBackColor = true;
            this.btnRemoveAll.Click += new System.EventHandler(this.btnRemoveAll_Click);
            // 
            // chkUseLocalTime
            // 
            this.chkUseLocalTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkUseLocalTime.AutoSize = true;
            this.chkUseLocalTime.Checked = true;
            this.chkUseLocalTime.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkUseLocalTime.Location = new System.Drawing.Point(12, 218);
            this.chkUseLocalTime.Name = "chkUseLocalTime";
            this.chkUseLocalTime.Size = new System.Drawing.Size(100, 17);
            this.chkUseLocalTime.TabIndex = 5;
            this.chkUseLocalTime.Text = "Use Local Time";
            this.chkUseLocalTime.UseVisualStyleBackColor = true;
            // 
            // MainForms
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(592, 253);
            this.Controls.Add(this.chkUseLocalTime);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnRemoveAll);
            this.Controls.Add(this.btnAddFile);
            this.Controls.Add(this.lstFiles);
            this.Controls.Add(this.btnCreateTcx);
            this.Name = "MainForms";
            this.Text = "LeMondCsvToTcx";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCreateTcx;
        private System.Windows.Forms.ListBox lstFiles;
        private System.Windows.Forms.Button btnAddFile;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnRemoveAll;
        private System.Windows.Forms.CheckBox chkUseLocalTime;
    }
}

