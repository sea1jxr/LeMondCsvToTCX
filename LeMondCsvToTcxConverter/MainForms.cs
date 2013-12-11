using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ConvertToTcx
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
            dialog.Filter = "Supported Files (*.csv;*.3dp;*.cdf.txt)|*.csv;*.3dp;*.cdf.txt|LeMond Files (*.csv)|*.csv|CompuTrainer (*.3dp)|*.3dp|Computrainer Coach (*.cdf.txt)|*.cdf.txt";
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
                List<SourcedStream> streams = new List<SourcedStream>();
                try
                {
                    using (TextWriter textWriter = new StreamWriter(dialog.FileName))
                    {
                        List<string> paths = new List<string>();
                        foreach (var item in lstFiles.Items)
                        {
                            string path = (string)item;
                            streams.Add(new SourcedStream() { Stream = new FileStream(path, FileMode.Open), Source = path });
                        }
                        new Converter().WriteTcxFile(streams, textWriter);
                    }
                    MessageBox.Show(string.Format("File '{0}' was created successfully", dialog.FileName), "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format("Error creating the TCX file: \r\n{0}\r\n\r\nDetails:\r\n{1}", ex.Message, ex.ToString()), "Error creating TCX file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    foreach (var sourcedStream in streams)
                    {
                        IDisposable disposable = sourcedStream.Stream as IDisposable;
                        if (disposable != null)
                        {
                            disposable.Dispose();
                        }
                    }
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
