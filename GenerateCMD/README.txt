=======表格自动生成工具=========

功能：从Excel表格生成txt和对应的读表代码

作者：limotao@qq.com   2015年11月27日, PM 03:48:24

用法：


1.配置文件Config.txt位置

该文件必须和运行文件GenerateCMD.exe放在同一级目录，例如，bin/Release/Config.txt


2.配置文件Config.txt内容

1	模版文件路径	../../Data/template.txt
2	CS文件输出路径	../../Data/
3	Excel文件输入路径	../../Data/Excel/
4	Txt文件输出路径	../../Data/DataTables/
5	CS文件名	DataConfig.cs

3.运行文件GenerateCMD.exe

如果一切顺利，会在步骤2中配置的目录生成txt文件和cs读表代码


4.应用到项目中

4.1 拷贝生成的txt数据到项目中
4.2 引用DataConfig.cs(生成的)，DataProvider.cs，MyDataTable.cs 三个文件到工程
4.3 实现DataProvider的文件读取委托，
    DataProvider.Instance.Init(delegate(string path) { return File.ReadAllText(Path.Combine("", path)); });
	参数path是相对路径，需要和txt数据实际所在目录拼接，以适应各个平台文件读取需求