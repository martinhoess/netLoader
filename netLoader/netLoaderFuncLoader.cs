using System;
using System.Threading;

namespace netLoader
{
    /// <summary>
    /// A loader which uses a delegate or action to load it's content
    /// </summary>
    public class netLoaderFuncLoader : netLoader
    {
        private Delegate _FuncToLoad = null;

        public object[] FuncParams = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="CallBackObserver"></param>
        /// <param name="FuncToLoad"></param>
        /// <param name="FuncParams"></param>
        public netLoaderFuncLoader(Int64 Id,
                                   String Name,
                                   InetLoaderCallbacks CallBackObserver,
                                   Delegate FuncToLoad,
                                   object[] FuncParams) : base(Id, CallBackObserver)
        {
            this.Name = Name;
            this._FuncToLoad = FuncToLoad;
            this.FuncParams = FuncParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Name"></param>
        /// <param name="CallBackObserver"></param>
        /// <param name="FuncToLoad"></param>
        /// <param name="FuncParams"></param>
        public netLoaderFuncLoader(Int64 Id,
                                   String Name,
                                   InetLoaderCallbacks CallBackObserver,
                                   Action FuncToLoad,
                                   object[] FuncParams) : base(Id, CallBackObserver)
        {
            this.Name = Name;
            this._FuncToLoad = FuncToLoad;
            this.FuncParams = FuncParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CallBackObserver"></param>
        /// <param name="FuncToLoad"></param>
        /// <param name="FuncParams"></param>
        public netLoaderFuncLoader(Int64 Id,
                                   InetLoaderCallbacks CallBackObserver,
                                   Delegate FuncToLoad,
                                   object[] FuncParams) : this(Id, "", CallBackObserver, FuncToLoad, FuncParams)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="CallBackObserver"></param>
        /// <param name="FuncToLoad"></param>
        /// <param name="FuncParams"></param>
        public netLoaderFuncLoader(string Name,
                                   InetLoaderCallbacks CallBackObserver,
                                   Delegate FuncToLoad,
                                   object[] FuncParams) : this(-1, Name, CallBackObserver, FuncToLoad, FuncParams)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="CallBackObserver"></param>
        /// <param name="FuncToLoad"></param>
        /// <param name="FuncParams"></param>
        public netLoaderFuncLoader(Int64 Id,
                                   InetLoaderCallbacks CallBackObserver,
                                   Action FuncToLoad,
                                   object[] FuncParams) : this(Id, "", CallBackObserver, FuncToLoad, FuncParams)
        {
            this._FuncToLoad = FuncToLoad;
            this.FuncParams = FuncParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="CallBackObserver"></param>
        /// <param name="FuncToLoad"></param>
        /// <param name="FuncParams"></param>
        public netLoaderFuncLoader(string Name,
                                   InetLoaderCallbacks CallBackObserver,
                                   Action FuncToLoad,
                                   object[] FuncParams) : this(-1, Name, CallBackObserver, FuncToLoad, FuncParams)
        {
            this._FuncToLoad = FuncToLoad;
            this.FuncParams = FuncParams;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CancelToken"></param>
        protected override void DoLoad(CancellationToken CancelToken)
        {
            base.DoLoad(CancelToken);

            if (_FuncToLoad != null)
                this.Data = _FuncToLoad.DynamicInvoke(FuncParams);
        }
    }
}
