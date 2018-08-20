/* meta
outputType app
//outputPath %Folders.ThisAppDocuments%\App1
//isolation process
*/
//#include usings.txt
//#include attributes.txt

partial class App
{
	[STAThread]
	static void Main(string[] args) { new App()._Main(args); }
	void _Main(string[] args)
	{
		Print("test");



	}
}
