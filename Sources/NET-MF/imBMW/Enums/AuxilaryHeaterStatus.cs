using System;
using Microsoft.SPOT;

namespace imBMW.Enums
{
    public enum AuxilaryHeaterStatus
    {
        Unknown,
        Present,
        Stopping,
        Stopped,
        Starting,
        Started,
        WorkPending,
        Working
    }
}
