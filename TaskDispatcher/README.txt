===================���߳̿��=======================

��Ҫ�ࣺ
	MyTaskDispatcher  ������������������̣߳�������ָ������߳�ȥ��
	MyThread  �̷߳�װ���ṩ��MyTaskDispatcher�ã����ߵ���ʹ�á�
	
ʾ����

	//delegate log����Ӧ��ע���̰߳�ȫ��
	MyThread.LogErrorHandler = Console.WriteLine;
	DelayActionProcessor.LogErrorHandler = Console.WriteLine;
	DelayActionProcessor.LogInfoHandler = Console.WriteLine;

	//MyTaskDispatcher
	
	int nThreadNum = 3;
	bool isPassive = true;//�������ԣ������̹߳���һ��ActionQueue������ÿ���߳�һ��ActionQueue
	MyTaskDispatcher mTaskDispatcher = new MyTaskDispatcher(3, true);
    mTaskDispatcher.DispatchAction(TestThread, "this is a param!");
	
	//MyThread
	
	MyThread thread = new MyThread();
	thread.Start();
	thread.QueueAction(TestThread, "this is a param!");//˳���Ŷ�ִ��
	
	//thread.Stop();
	
	private static void TestThread(string param)
	{
		Console.WriteLine(param);
	}
	
	