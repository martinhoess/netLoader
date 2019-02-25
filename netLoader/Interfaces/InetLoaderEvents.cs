namespace netLoader
{
    public interface InetLoaderEvents
    {
        /// <summary>
        /// When the loader starts
        /// </summary>
        event netLoaderEvent OnLoadStart;

        /// <summary>
        /// When the loader is finished
        /// </summary>
        event netLoaderEvent OnLoadFinished;

        /// <summary>
        /// When the loader is canceled
        /// </summary>
        event netLoaderEvent OnLoadCanceled;

        /// <summary>
        /// When the loader has failed
        /// </summary>
        event netLoaderEvent OnLoadFailed;

        
    }
}
