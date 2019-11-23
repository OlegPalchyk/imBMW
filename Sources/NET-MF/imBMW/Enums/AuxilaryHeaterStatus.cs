using System;
using Microsoft.SPOT;

namespace imBMW.Enums
{
    public enum AuxilaryHeaterStatus
    {
        Unknown,
        Present,
        StopPending,
        Stopping,
        Stopped,
        StartRequested,
        StartPending,
        Starting,
        Started,
        Working
    }
}
