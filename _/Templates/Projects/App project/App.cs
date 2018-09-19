/* meta
outputType app
//outputPath %Folders.ThisAppDocuments%\App1
//isolation process
//debug false
*/
//#include usings.txt

[assembly: System.Reflection.AssemblyVersion("1.0.0.0")]
//[assembly: System.Runtime.Versioning.TargetFramework(".NETFramework,Version=v4.7.2")]

class App :AuAppBase {
[STAThread] static void Main(string[] args) { new App()._Main(args); }

void _Main(string[] args) {
	Print("test");
	
	
	
}


}
