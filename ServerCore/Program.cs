var ThreadName = new ThreadLocal<string>(() => $"My Name Is {Thread.CurrentThread.ManagedThreadId}");

void WhoAmI()
{
    bool repeat = ThreadName.IsValueCreated;
    if (repeat)
        Console.WriteLine(ThreadName.Value + " (repeat)");
    else
        Console.WriteLine(ThreadName.Value);
}

ThreadPool.SetMinThreads(1, 1);
ThreadPool.SetMaxThreads(3, 3);
Parallel.Invoke(WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI, WhoAmI);

ThreadName.Dispose();