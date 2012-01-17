using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LeMondCsvToTcxConverter
{
    public partial class MainForms : Form
    {
        public MainForms()
        {
            InitializeComponent();
        }

        private void btnAddFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;
            dialog.Title = "Find LeMond .csv files";
            dialog.Filter = "LeMond Files (*.csv)|*.csv";
            dialog.FilterIndex = 1;

            var result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                lstFiles.Items.Add(dialog.FileName);
            }
        }

        private void btnCreateTcx_Click(object sender, EventArgs e)
        {
            if (lstFiles.Items.Count == 0)
            {
                MessageBox.Show("You must add a file before you can create a .tcx file");
                return;
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "Save Tcx As";
            dialog.Filter = "tcx files (*.tcx)|*.tcx|All files (*.*)|*.*";
            dialog.FilterIndex = 1;
            dialog.RestoreDirectory = true;
            dialog.FileName = Path.GetFileNameWithoutExtension((string)lstFiles.Items[0]);
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    using (TextWriter textWriter = new StreamWriter(dialog.FileName))
                    {
                        List<string> paths = new List<string>();
                        foreach (var item in lstFiles.Items)
                        {
                            paths.Add((string)item);
                        }
                        new Converter().WriteTcxFile(paths.Select(p => new SourcedReader() { Source = Path.GetFileName(p), TextReader = new StreamReader(p) }), textWriter);
                    }
                    MessageBox.Show(string.Format("File '{0}' was created successfully", dialog.FileName));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format("Error creating the TCX file: \r\n{0}\r\n\r\nDetails:\r\n{1}", ex.Message, ex.ToString()), "Error creating TCX file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRemoveAll_Click(object sender, EventArgs e)
        {
            lstFiles.Items.Clear();
        }
    }
}
