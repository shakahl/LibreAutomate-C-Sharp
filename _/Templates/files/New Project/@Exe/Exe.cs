/*/ role exeProgram; outputPath %folders.Workspace%\bin\RenameMe; /*/ //.
using System.Reflection;

[assembly: AssemblyVersion("1.0.0.0")]

// attributes for native version resource, all optional
//[assembly: AssemblyFileVersion("1.0.0.0")] //if missing, uses AssemblyVersion
//[assembly: AssemblyTitle("File description")]
//[assembly: AssemblyDescription("Comments")]
//[assembly: AssemblyCompany("Company name")]
//[assembly: AssemblyProduct("Product name")]
//[assembly: AssemblyInformationalVersion("1.0.0.0")] //product version
//[assembly: AssemblyCopyright("Copyright © 2020")]
//[assembly: AssemblyTrademark("Legal trademarks")]

class Program { static void Main(string[] a) => new Program(a); Program(string[] args) { //;;
script.setup(trayIcon: true);
//;

/*
To create .exe program from a script: in Properties select role exeProgram. This script is an example.
When you compile or run the script, .exe and other required files are created in the output directory (outputPath).
To run the .exe program, you can click the Run button. Or run it like any other program.
If antivirus blocks new .exe files or makes compiling slow, add the output directory to its list of exclusions.
Before deploying the files, in Properties set optimize true and compile.
If need 32-bit version too, in Properties check bit32, compile, uncheck again.
The program can run on computers with installed .NET 5 Runtime. Download: https://dotnet.microsoft.com/download
To add version info, use attributes like the examples above.
*/










}
}
