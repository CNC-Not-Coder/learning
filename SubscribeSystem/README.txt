===============全局事件系统===========================

旨在方便各个类实例之间的通信

通过发送事件的形式，可以调用到其他类的功能，而不需要引用它们

需要注意线程安全性！！！

示例：

	注册：
	
	private static void RegisterEvent()
	{
		//Init 
		mEventSystem = new PublishSubscribeSystem();
		mEventSystem.Subscribe<string>("print_to_console", "ui", PrintToConsole);
	}
	private static void PrintToConsole(string text)
	{
		Console.WriteLine(text);
	}

	触发：
	
	private static void FireEvent()
	{
		mEventSystem.Publish("print_to_console", "ui", "Hello ! ");
	}
	
	//Log  注意线程安全性
	PublishSubscribeSystem.LogErrorHandler = Console.WriteLine;
    PublishSubscribeSystem.LogInfoHandler = Console.WriteLine;