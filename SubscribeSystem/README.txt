===============ȫ���¼�ϵͳ===========================

ּ�ڷ��������ʵ��֮���ͨ��

ͨ�������¼�����ʽ�����Ե��õ�������Ĺ��ܣ�������Ҫ��������

��Ҫע���̰߳�ȫ�ԣ�����

ʾ����

	ע�᣺
	
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

	������
	
	private static void FireEvent()
	{
		mEventSystem.Publish("print_to_console", "ui", "Hello ! ");
	}
	
	//Log  ע���̰߳�ȫ��
	PublishSubscribeSystem.LogErrorHandler = Console.WriteLine;
    PublishSubscribeSystem.LogInfoHandler = Console.WriteLine;