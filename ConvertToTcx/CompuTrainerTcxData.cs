using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvertToTcx
{
    public class CompuTrainerTcxData : ITcxData
    {
        private CompuTrainer3DPFileProvider provider;   

        public CompuTrainerTcxData(CompuTrainer3DPFileProvider provider)
        {
            this.provider = provider;
        }

        public DateTime StartTime
        {
            get { return provider.StartTime; }
        }

        public TcxSport Sport
        {
            get { return TcxSport.Biking; }
        }

        public IEnumerable<TcxTrackPoint> TrackPoints
        {
            get 
            { 
                // the computrainer samples every few miliseconds, we don't want that many samples
                // so we will take the first and the last one, and roughly 1 second in between.
                int countDown = provider.SampleCount;
                uint lastSecondLogged = 0;
                foreach (var sample in provider.Samples)
                {
                    uint currentSecond = sample.TimeMilisecondElapsed / 1000;
                    if(countDown == provider.SampleCount || // last one
                       countDown == 1 ||                    // fist one
                       lastSecondLogged != currentSecond)   // first one for this second
                    {
                        // log it
                        lastSecondLogged = currentSecond;
                        yield return new TcxTrackPoint()
                        {
                            CadenceRpm = sample.CadenceRpm,
                            CaloriesElapsed = 0,
                            DistanceMetersElapsed = ConvertDistance.KilometersToMeters(sample.DistanceKilometerElapsed),
                            HeartRateBpm = sample.HeartRateBpm,
                            PowerWatts = sample.PowerWatts,
                            SpeedMetersPerSecond = ConvertTime.SecondsToHours(ConvertDistance.KilometersToMeters(ConvertDistance.MilesToKilometers(sample.SpeedMph))),
                            Time = provider.StartTime + TimeSpan.FromMilliseconds(sample.TimeMilisecondElapsed),
                        };
                    
                    }

                    countDown--;
                }
            }
        }
    }
}
