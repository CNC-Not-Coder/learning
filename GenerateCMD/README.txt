=======����Զ����ɹ���=========

���ܣ���Excel�������txt�Ͷ�Ӧ�Ķ������

���ߣ�limotao@qq.com   2015��11��27��, PM 03:48:24

�÷���


1.�����ļ�Config.txtλ��

���ļ�����������ļ�GenerateCMD.exe����ͬһ��Ŀ¼�����磬bin/Release/Config.txt


2.�����ļ�Config.txt����

1	ģ���ļ�·��	../../Data/template.txt
2	CS�ļ����·��	../../Data/
3	Excel�ļ�����·��	../../Data/Excel/
4	Txt�ļ����·��	../../Data/DataTables/
5	CS�ļ���	DataConfig.cs

3.�����ļ�GenerateCMD.exe

���һ��˳�������ڲ���2�����õ�Ŀ¼����txt�ļ���cs�������


4.Ӧ�õ���Ŀ��

4.1 �������ɵ�txt���ݵ���Ŀ��
4.2 ����DataConfig.cs(���ɵ�)��DataProvider.cs��MyDataTable.cs �����ļ�������
4.3 ʵ��DataProvider���ļ���ȡί�У�
    DataProvider.Instance.Init(delegate(string path) { return File.ReadAllText(Path.Combine("", path)); });
	����path�����·������Ҫ��txt����ʵ������Ŀ¼ƴ�ӣ�����Ӧ����ƽ̨�ļ���ȡ����