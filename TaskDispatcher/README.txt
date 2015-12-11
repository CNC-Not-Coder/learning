===================多线程框架=======================

主要类：
	MyTaskDispatcher  任务分配器，管理多个线程，将任务分给各个线程去做
	MyThread  线程封装，提供给MyTaskDispatcher用，或者单独使用。
	
示例：

	//delegate log函数应该注意线程安全性
	MyThread.LogErrorHandler = Console.WriteLine;
	DelayActionProcessor.LogErrorHandler = Console.WriteLine;
	DelayActionProcessor.LogInfoHandler = Console.WriteLine;

	//MyTaskDispatcher
	
	int nThreadNum = 3;
	bool isPassive = true;//积极策略，所有线程共用一个ActionQueue，否则每个线程一个ActionQueue
	MyTaskDispatcher mTaskDispatcher = new MyTaskDispatcher(3, true);
    mTaskDispatcher.DispatchAction(TestThread, "this is a param!");
	
	//MyThread
	
	MyThread thread = new MyThread();
	thread.Start();
	thread.QueueAction(TestThread, "this is a param!");//顺序排队执行
	
	//thread.Stop();
	
	private static void TestThread(string param)
	{
		Console.WriteLine(param);
	}
	
	