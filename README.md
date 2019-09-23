# netLoader
C# implementation/port of the [Android loaders](https://developer.android.com/reference/android/support/v4/app/LoaderManager.html), for better task/async handling

Example
------------

    Func<string, InetLoader, string> Load = delegate(string input, InetLoader l)
    {
    	(DateTime.Now + TimeSpan.FromSeconds(5)).With(time => { while (DateTime.Now < time) ; }); // Simulate long runing operation
    	// l.Cancel(0, true);
    	// throw new Exception("");
    	return "Loaded Data";
    };

    netLoaderFuncLoader loader = new netLoaderFuncLoader("TEST LOADER", null, Load, null);
    loader.FuncParams = new object[] { "input value", loader };
    loader.OnLoadStart += (l) => Debug.WriteLine($"{l.Name} start");
    loader.OnLoadFinished += (l) => Debug.WriteLine($"{l.Name} finished with data:<{l.Data}>");
    loader.OnLoadCanceled += (l) => Debug.WriteLine($"{l.Name} canceled");
    loader.OnLoadFailed += (l) => Debug.WriteLine($"{l.Name} failed");
    loader.AsyncLoad();
