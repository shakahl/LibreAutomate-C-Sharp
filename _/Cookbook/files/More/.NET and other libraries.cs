/// In scripts can be used everything from .NET libraries. Don't need to add assembly references. Need to add just <.k>using<> directives that aren't in file global.cs.

using System.Xml.Linq;

var x = XElement.Load(@"C:\Test\file.xml");

/// Many other managed libraries can be downloaded, for example from <google>NuGet</google> or <google>GitHub</google>. Currently this program does not have a NuGet package manager, therefore need to manually download and extract packages and their dependencies. NuGet packages are .zip files with file extension .nupkg. Usually need just the .dll file, and .xml if exists. Often they are in /runtimes/win/lib/netX or /lib/netX or /lib/netstandardX. Extract them to the Libraries subfolder of the editor's folder. Then in Properties add assembly reference. Then add <.k>using<> directive.

/*/ r Libraries\Humanizer.dll; /*/
using Humanizer;

var s = "one two";
s = s.Titleize();
print.it(s);

/// Often libraries have several versions, for different .NET versions. They can be either in separate packages or several .dll files in single package. Must be for .NET 6+ (look in menu Help -> About) or .NETStandard, not for .NET Framework. Often all or most of the listed dependencies are in .NET (installed), but sometimes some must be downloaded and extracted in the same way as the main library.
