using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ConvertToTcx
{
    public class CompuTrainer3DPFileProvider 
    {
        private BinaryReader input;
        private string userName;
        private int age;
        private float weightKilos;
        private int upperHeartRate;
        private int lowerHeartRate;
        private DateTime startTime;

        private int numberOfDataPoints;


        // we'll keep track of the altitude over time.  since computrainer
        // gives us slope, we can calculate change in altitude if we know
        // change in distance traveled, so we also need to keep track of
        // the previous sample's distance.
        private float altitude = 100.0f;  // arbitrary starting altitude of 100m
        private float lastKM = 0;

        // computrainer 3d software lets you start your ride partway into
        // a course.  if you do this, then the first distance reported in
        // the corresponding log file will be that offset, rather than
        // zero.  so, we'll stash away the first reported distance, and
        // use that to offset distances that we report to GC so that they
        // are zero-based (i.e., so that the first data point is at
        // distance zero).
        private float firstKM = 0;
        private bool gotFirstKM = false;

        // computrainer doesn't have a fixed inter-sample-interval; GC
        // expects one, and estimating one by averaging causes problems
        // for some calculations that GC does.  also, computrainer samples
        // so frequently (once every 30-50ms) that the O(n^2) critical
        // power plot calculation takes waaaaay too long.  to solve both
        // problems at once, we smooth the file, emitting an averaged data
        // point every 250 milliseconds.
        //
        // for HR, cadence, watts, and speed, we'll do time averaging to
        // figure out the correct average since the last emitted point.
        // for distance and altitude, we just need to interpolate from the
        // last data point in the computrainer file itself.
        private float lastAltitude = 100.0f;
        private uint lastEmittedMS = 0;
        private uint lastSampleMS = 0;
        private double hr_sum = 0.0;
        private double cad_sum = 0.0;
        private double speed_sum = 0.0;
        private double watts_sum = 0.0;

        public CompuTrainer3DPFileProvider(SourcedStream sourced)
        {
            this.input = new BinaryReader(sourced.Stream, Encoding.ASCII);

            ReadHeaderInfomation(sourced.Source);
        }

        private void ReadHeaderInfomation(string source)
        {
            // looks like the first part is a header... ignore it.
            this.input.Skip(4);

            // the next 4 bytes are the ASCII characters 'perf'
            var perf = new String(this.input.ReadChars(4));
            if (perf != "perf")
            {
                throw new Exception(string.Format("The computrainer file given '{0}' does not appear to be a valid computrainer performance file", source));
            }

            // not sure what the next 8 bytes are; skip them
            this.input.Skip(8);

            // the next 65 bytes are a null-terminated and padded
            // ASCII user name string
            StringBuilder userNameBuilder = new StringBuilder();
            var rawUserName = this.input.ReadChars(65);
            foreach (char c in rawUserName)
            {
                if (c == 0)
                {
                    break;
                }

                userNameBuilder.Append(c);
            }
            userName = userNameBuilder.ToString();

            // next is a single byte of user age, in years.  I guess
            // Computrainer doesn't allow people to get older than 255
            // years. ;)
            age = this.input.ReadByte();

            // not sure what the next 6 bytes are; skip them.
            input.Skip(6);

            // next is a (4 byte) C-style floating point with weight in kg
            weightKilos = input.ReadSingle();

            // next is the upper heart rate limit (4 byte int)
            upperHeartRate = input.ReadInt32();

            // and then the resting heart rate (4 byte int)
            lowerHeartRate = input.ReadInt32();

            // then year, month, day, hour, minute the exercise started
            // (4, 1, 1, 1, 1 bytes)
            int year = input.ReadInt32();
            int month = input.ReadByte();
            int day = input.ReadByte();
            int hour = input.ReadByte();
            int minute = input.ReadByte();

            startTime = new DateTime(year, month, day, hour, minute, 0, DateTimeKind.Local);

            // the number of exercise data points in the file (4 byte int)
            numberOfDataPoints = input.ReadInt32();

            // go back to the start, and skip header to go to
            // the start of the data samples.
            input.BaseStream.Seek(0, SeekOrigin.Begin);
            input.Skip(0xf8);
        }

        public DateTime StartTime { get { return startTime; } }
        public int Age { get { return age; } }
        public string UserName { get { return userName; } }
        public float WeightKilos { get { return weightKilos; } }
        public int UpperHeartRate { get { return upperHeartRate; } }
        public int LowerHeartRate { get { return lowerHeartRate; } }

        public int SampleCount { get { return numberOfDataPoints; } }

        public IEnumerable<ComputrainerDataSample> Samples
        {
            get
            {
                for (int i = 0; i < numberOfDataPoints; i++)
                {
                    yield return ReadSample();
                }
            }
        }

        private ComputrainerDataSample ReadSample()
        {
            var sample = new ComputrainerDataSample();
            // 1 byte heart rate, in BPM
            sample.HeartRateBpm = input.ReadByte();

            // 1 byte cadence, in RPM
            sample.CadenceRpm = input.ReadByte();

            // 2 unsigned bytes of watts
            sample.PowerWatts = input.ReadUInt16();

            // 4 bytes of floating point speed (in mph/160 !!)
            sample.SpeedMph = input.ReadSingle() * 160;

            // 4 bytes of total elapsed time, in milliseconds
            sample.TimeMilisecondElapsed = input.ReadUInt32();

            // 2 signed bytes of 100 * [percent grade]
            // (i.e., grade == 100 * 100 * rise/run !!)
            sample.GradePercent = input.ReadInt16() / (100.0f * 100.0f);

            // not sure what the next 2 bytes are
            input.Skip(2);

            // 4 bytes of floating point total distance traveled, in KM
            sample.DistanceKilometerElapsed = input.ReadSingle();

            // not sure what the next 28 bytes are.
            input.Skip(0x1c);

            return sample;
        }
    
    }

    public class ComputrainerDataSample
    {
        public int HeartRateBpm { get; set; }
        public int CadenceRpm { get; set; }
        public int PowerWatts { get; set; }
        public float SpeedMph { get; set; }
        public uint TimeMilisecondElapsed { get; set; }
        public float GradePercent { get; set; }
        public float DistanceKilometerElapsed { get; set; }
    }

    public static class BinaryReaderExtensions
    {
        public static void Skip(this BinaryReader reader, long numBytesToSkip)
        {
            reader.BaseStream.Seek(numBytesToSkip, SeekOrigin.Current);
        }
    }
}
