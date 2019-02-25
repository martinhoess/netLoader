using System;

namespace netLoader
{
    public interface InetLoader : IDisposable
    {
        Int64 Id { get; }
        string Name { get; }
        object Data { get; }
        object Tag { get; set; }
        InetLoaderCallbacks CallbackObserver { get; }
        netLoaderState State { get; }

        InetLoader AsyncLoad();
        InetLoader Cancel(Int32 Delay = 0, bool Force = false);
    }
}
