/*/ role exeProgram; outputPath %AFolders.Workspace%\bin\RenameMe; runMode blue; ifRunning warn_restart; /*/ //.
using Au; using Au.Types; using System; using System.Collections.Generic; using System.IO; using System.Linq;
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

partial class Script : AScript { [STAThread] static void Main(string[] a) => new Script(a); Script(string[] args) { //;;;

/*
To create .exe program from a script: in Properties select role exeProgram. This script is an example.
When you compile or run the script, .exe and other required files are created in the output directory (outputPath).
To run the .exe program, you can click the Run button. Or run it like any other program.
If antivirus blocks new .exe files or makes compiling slow, add the output directory to its list of exclusions.
Before deploying the files, in Properties set optimize true and compile. It also creates 64-bit and 32-bit versions.
The program can run on computers with installed .NET Core Runtime 3.1. Download: https://dotnet.microsoft.com/download
To add version info, use attributes like the examples above.
*/










}
}
