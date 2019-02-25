using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace netLoader
{
    /// <summary>
    /// Base class for netLoader
    /// </summary>
    public class netLoader: InetLoader, InetLoaderEvents
    {
        public Int64 Id { get; private set; } = -1;
        public string Name { get; internal set; } = null;
        public object Data { get; internal set; } = null; // ToDo: somehow as generic?
        public InetLoaderCallbacks CallbackObserver { get; private set; } = null;
        public netLoaderState State { get; internal set; } = netLoaderState.None;
        public object Tag { get; set; } = null;

        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

        public event netLoaderEvent OnLoadStart;
        public event netLoaderEvent OnLoadFinished;
        public event netLoaderEvent OnLoadCanceled;
        public event netLoaderEvent OnLoadFailed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CallbackObserver"></param>
        public netLoader(Int64 Id, InetLoaderCallbacks CallbackObserver = null)
        {
            this.Id = Id;
            this.CallbackObserver = CallbackObserver;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="CallbackObserver"></param>
        public netLoader(string Name, InetLoaderCallbacks CallbackObserver = null)
        {
            this.Name = Name;
            this.CallbackObserver = CallbackObserver;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            // Try to cancel the loader so that the task is canceled prematurely, otherwhise the task would run until 
            // it's finished
            this?.Cancel(0, true);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="CancelToken"></param>
        protected virtual void DoLoad(CancellationToken CancelToken)
        {
            CheckCanceled();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CancelToken"></param>
        private void InternalLoad(CancellationToken CancelToken)
        {
            try
            {
                CheckCanceled();

                LoaderStart();

                CheckCanceled();

                DoLoad(CancelToken);

                CheckCanceled();

                LoaderFinished();
            }
            catch (OperationCanceledException ex)
            {
                LoaderCanceled(ex.CancellationToken);
            }
            catch (Exception ex)
            {
                LoaderFailed(ex);
            }
        }

        /// <summary>
        /// Starts the loading process
        /// </summary>
        /// <returns></returns>
        public InetLoader AsyncLoad()
        {
            // It shouldn't be possible to start a loader again if it's already starting/running
            if ((State == netLoaderState.Loading) ||
                (State == netLoaderState.Starting) ||
                (State == netLoaderState.Running))
                return this;

            this.State = netLoaderState.Loading;

            // ToDo: better way?
            if (_cancelTokenSource.IsCancellationRequested)
                _cancelTokenSource = new CancellationTokenSource();

            CancellationToken cancelToken = _cancelTokenSource.Token;

            Task.Factory.StartNew(() =>
            {
                InternalLoad(cancelToken);
            }, cancelToken);

            return this;
        }

        /// <summary>
        /// Cancels a running loader
        /// </summary>
        /// <param name="Delay">The time span to wait before canceling</param>
        /// <param name="Force"></param>
        /// <returns></returns>
        public InetLoader Cancel(Int32 Delay = 0, bool Force = false)
        {
            if (Force || this.State.HasFlag(netLoaderState.Loading) || 
                         this.State.HasFlag(netLoaderState.Starting) || 
                         this.State.HasFlag(netLoaderState.Starting))
            {
                if (Delay > 0)
                    _cancelTokenSource.CancelAfter(Delay);
                else
                    _cancelTokenSource.Cancel(true);

                this.Data = null;
                this.State = netLoaderState.CancelRequested;
            }

            return this;
        }

        /// <summary>
        /// Check if a cancellation is requested
        /// </summary>
        protected void CheckCanceled()
        {
            if (_cancelTokenSource.IsCancellationRequested)
                _cancelTokenSource.Token.ThrowIfCancellationRequested();
        }

        /// <summary>
        /// Loader starts
        /// </summary>
        protected virtual void LoaderStart()
        {
            this.Data = null;
            this.State = netLoaderState.Starting;

            try
            {
                // never ever change order of event invokes!
                OnLoadStart?.Invoke(this);
                CallbackObserver?.OnLoadStart(this);
            }
            catch (Exception ex)
            {
                // this is only the exception handling for the event invoke not for the loader itself!
                Debug.WriteLine(ex);
                // ToDo: logging interface
            }
            finally
            {
                this.State = netLoaderState.Running;
            }
        }

        /// <summary>
        /// Loader is finished
        /// </summary>
        protected virtual void LoaderFinished()
        {
            #region INLINES
            void Finished ()
            {
                this.State = netLoaderState.Finished;

                try
                {
                    // never ever change order of event invokes!
                    OnLoadFinished?.Invoke(this);
                    CallbackObserver?.OnLoadFinished(this);
                }
                catch (Exception ex)
                {
                    // this is only the exception handling for the event invoke not for the loader itself!
                    Debug.WriteLine(ex);
                    // ToDo: logging interface
                }
            };
            #endregion

            if (this.State.HasFlag(netLoaderState.Canceled) || this.State.HasFlag(netLoaderState.CancelRequested))
                LoaderCanceled(_cancelTokenSource.Token);
            else
                Finished();
        }

        /// <summary>
        /// Loader is canceled
        /// </summary>
        /// <param name="Token"></param>
        protected virtual void LoaderCanceled(CancellationToken Token)
        {
            this.State = netLoaderState.Canceled;
            this.Data = null;

            try
            {
                // never ever change order of event invokes!
                OnLoadCanceled?.Invoke(this);
                CallbackObserver?.OnLoadCanceled(this);
            }
            catch (Exception ex)
            {
                // this is only the exception handling for the event invoke not for the loader itself!
                Debug.WriteLine(ex);
                // ToDo: logging interface
            }
        }

        /// <summary>
        /// Loader has failed
        /// </summary>
        /// <param name="Exception"></param>
        protected virtual void LoaderFailed(Exception Exception)
        {
            this.State = netLoaderState.Failed;
            this.Data = null;

            try
            {
                // never ever change order of event invokes!
                OnLoadFailed?.Invoke(this);
                CallbackObserver?.OnLoadFailed(this, Exception);
            }
            catch (Exception ex)
            {
                // this is only the exception handling for the event invoke not for the loader itself!
                Debug.WriteLine(ex);
                // ToDo: logging interface
            }
        }
    }
}
