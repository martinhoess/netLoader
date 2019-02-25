using System;

namespace netLoader
{
    public interface InetLoaderCallbacks
    {
        void OnLoadFinished(InetLoader Loader);
        void OnLoadCanceled(InetLoader Loader);
        void OnLoadFailed(InetLoader Loader, Exception Exception);
        void OnLoadStart(InetLoader Loader);
    }
}
