using System;

namespace netLoader
{
    [Flags]
    public enum netLoaderState
    {
        None = 1,
        Loading = 1 << 0,
        Starting = 1 << 1,
        Running = 1 << 2,
        Finished = 1 << 3,
        CancelRequested = 1 << 4,
        Canceled = 1 << 5,
        Failed = 1 << 6,

        Any = ~0
    }
}
