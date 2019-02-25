using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;

namespace netLoader
{
    /// <summary>
    /// 
    /// </summary>
    public class kLoaderManager : IDisposable
    {
        private ConcurrentBag<InetLoader> _registeredLoaders = new ConcurrentBag<InetLoader>();

        public InetLoader this[Int64 Id] { get => _registeredLoaders.SingleOrDefault(l => l.Id == Id); }
        public InetLoader this[string Name] { get => _registeredLoaders.SingleOrDefault(l => l.Name == Name); }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this?.ClearLoaders();
        }

        /// <summary>
        /// Registeres a loader
        /// </summary>
        /// <param name="Loader">Loader</param>
        /// <param name="LoaderMode">Loader mode "start" or "wait"</param>
        public void RegisterLoader(InetLoader Loader, netLoaderMode LoaderMode = netLoaderMode.Wait)
        {
            try
            {
                if (!_registeredLoaders.Contains(Loader))
                {
                    _registeredLoaders.Add(Loader);
                    if (LoaderMode == netLoaderMode.Start)
                        Loader.AsyncLoad();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // ToDo: logging interface
            }
        }
        
        /// <summary>
        /// Unregisteres a specific loader
        /// </summary>
        /// <param name="Loader">Loader to unregister</param>
        /// <param name="DestroyLoader">Dispose the loader</param>
        public void UnregisterLoader(InetLoader Loader, bool DestroyLoader)
        {
            try
            {
                if (_registeredLoaders.Contains(Loader))
                {
                    Loader.Cancel(0, true);

                    _registeredLoaders = new ConcurrentBag<InetLoader>(_registeredLoaders.Except(new[] { Loader }));
                    if (DestroyLoader)
                    {
                        Loader?.Dispose();
                        Loader = null;
                    }
                        
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // ToDo: logging interface
            }
        }

        /// <summary>
        /// Unregister an force cancel all registered loaderes
        /// </summary>
        public void ClearLoaders()
        {
            foreach (InetLoader loader in Loaders())
                UnregisterLoader(loader, true);
        }

        /// <summary>
        /// (re)loads all registered loadaders which are not running
        /// </summary>
        /// <param name="ReloadFinished">also reload loaders which are finished</param>
        /// <param name="ReloadCanceled">also reload loaders which are canceled</param>
        /// <param name="ReloadFailed">also reload loaderes which are failed</param>
        public void LoadLoaders(bool ReloadFinished = false, bool ReloadCanceled = false, bool ReloadFailed = false)
        {
            foreach (InetLoader loader in _registeredLoaders)
            {
                bool load = (loader.State == netLoaderState.None) ||
                            ((loader.State == netLoaderState.Finished) && ReloadFinished) ||
                            ((loader.State == netLoaderState.Canceled) && ReloadCanceled) ||
                            ((loader.State == netLoaderState.Failed) && ReloadFailed);

                if (load)
                    loader.AsyncLoad();
            }
        }

        /// <summary>
        /// Returns the number of registered loaders
        /// </summary>
        /// <param name="States">Only count loadeder which state is in</param>
        /// <returns>The number of registered loaderes</returns>
        public Int32 Count(netLoaderState States = netLoaderState.Any)
        {
            return _registeredLoaders.Where(l => States.HasFlag(l.State)).Count();
        }
        
        /// <summary>
        /// Returns the registered loadeders
        /// </summary>
        /// <param name="States">Only return loaders which state is in</param>
        /// <returns>Registered loaders</returns>
        public IEnumerable<InetLoader> Loaders(netLoaderState States = netLoaderState.Any)
        {
            return _registeredLoaders.Where(l => States.HasFlag(l.State)).ToList();
        }
    }
}