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

        const int MetersPerKilometer = 1000;
        const double HoursPerSecond = 1 / (60 * 60);
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
                TimeSpan oneSecond = new TimeSpan(0, 0, 1);
                using(TextWriter textWriter = new StreamWriter(dialog.FileName))
                using (TcxWriter writer = new TcxWriter(textWriter, !chkUseLocalTime.Checked))
                {
                    writer.StartTcx();
                    bool firstFile = true;
                    LapStats stats = new LapStats() { Calories = 0, DistanceMeters = 0, TotalTimeSeconds = 0 };
                    foreach (var item in lstFiles.Items)
                    {
                        var provider = new LeMondGForceCsvDataProvider(new StreamReader((string)item), Path.GetFileName((string)item));
                        var reader = new LeMondDataReader(provider);
                        if (firstFile)
                        {
                            writer.StartActivity(reader.StartTime, TcxSport.Biking);
                            firstFile = false;
                        }
                        
                        writer.StartLap(reader.StartTime);

                        bool firstPoint = true;
                        foreach (var point in reader.DataPoints)
                        {
                            if (firstPoint)
                            {
                                // adding a fake first point becuase strava seems
                                // to like seeing seconds 0:00-1:00 for a minute instead of 0:01-1:00
                                // this new point will actually give us 61 points, but will be considerd
                                // a full minute
                                WriteTrackPoint(writer, stats, reader.StartTime - oneSecond, point);
                                firstPoint = false;
                            }
                            WriteTrackPoint(writer, stats, reader.StartTime, point);
                        }

                        stats = writer.EndLap();
                    }
                    writer.EndActivity();
                    writer.EndTcx();
                }

                MessageBox.Show(string.Format("File '{0}' was created successfully", dialog.FileName));
            }
        }

        private static void WriteTrackPoint(TcxWriter writer, LapStats stats, DateTime baseTime, LeMondDataPoint point)
        {
            writer.StartTrackPoint();
            writer.WriteTrackPointTime(baseTime + point.ElapsedTime);
            writer.WriteTrackPointCadence(point.CadenceRotationsPerMinute);
            writer.WriteTrackPointElapsedCalories(point.ElapsedCalories + stats.Calories);
            writer.WriteTrackPointElapsedDistanceMeters(point.DistanceKilometers * MetersPerKilometer + stats.DistanceMeters);
            writer.WriteTrackPointHeartRateBpm(point.HeartRateBeatsPerMinute);
            writer.WriteTrackPointPowerWatts(point.PowerWatts);
            writer.WriteTrackPointSpeedMetersPerSecond(point.SpeedKilometersPerHour * MetersPerKilometer * HoursPerSecond);
            writer.EndTrackPoint();
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
