using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConvertToTcx
{
    public interface ITcxData
    {
        DateTime StartTime { get; }
        TcxSport Sport { get; }
        IEnumerable<TcxTrackPoint> TrackPoints { get; }
    }
}
