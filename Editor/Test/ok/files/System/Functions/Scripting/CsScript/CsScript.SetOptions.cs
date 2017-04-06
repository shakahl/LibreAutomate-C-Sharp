function $optionsList [flags] ;;options: debugConfig noFileCache inMemoryAsm usingNamespaces references compileFiles resourceFiles searchDirs compilerOptions language tempDir appDirs.  flags: 1 global.

 Sets options that will be used by other functions.

 flags:
   0 - set options for this variable.
   1 - set global options, ie default options used always in this process.
 optionsList - list of option=value. Options are case sensitive.
   <b>debugConfig</b> - if true, creates Debug version assembly. Also creates debug-info files (.cs and .pdb) in cache folder (Compile - .pdb file in outputFile's folder).
   <b>noFileCache</b> - if true, doesn't create cache file. Always compiles script when used first time in a process. Uses only memory cache. Sets inMemoryAsm=true.
   <b>inMemoryAsm</b> - if true (default), does not lock the assembly file while it is loaded. In most cases faster loads it. However other assemblies normally cannot have references to the loaded assembly.
   <b>usingNamespaces</b> - if true (default), automatically adds references to assemblies that match namespace names specified in 'using' directives in C# code.
     For example, if C# code contains <c 0xFF0000>using System.Windows.Forms;</c>, adds reference to System.Windows.Forms.dll assembly.
     Otherwise need to specify all references in the <b>references</b> option (see below).
   <b>references</b> - ;-separated list of references. Example: "references=System.Data;System.XML".
     In most cases don't need to specify references because they are resolved from 'using' directives in script, unless <b>usingNamespaces</b> option is false.
   <b>compileFiles</b> - ;-separated list of other C# source files (.cs) to compile to the assembly. The main script can use public members of their namespaces.
   <b>resourceFiles</b> - ;-separated list of resource files to add to the assembly.
   <b>searchDirs</b> - ;-separated list of folders where to look for various files when full path not specified. Example: "searchDirs=C:\Folder; %APPDATA%\Folder".
     Don't need to specify path for files that are in program's folder or script file's folder (if file used). Referenced assemblies is a special case, read in remarks.
   <b>compilerOptions</b> - additional command line arguments to pass to the C# compiler (csc.exe). <google>csc C# compiler options</google>.
   <b>language</b> - if VB, compiles as VB.NET.
   <b>tempDir</b> (with flag 1) - folder for temporary files and cache. Initially it is $temp qm$\CsScript.
   <b>appDirs</b> (with flag 1) - folders, relative to the program's folder, where to search for private referenced assemblies at run time (not used when compiling).

 REMARKS
 Don't need to call Init before. Especially when setting global options when QM starts, because Init then loads .NET runtime, and QM starts slowly.

 Can be specified all or some options.
 Missing options for this variable will be taken from global options.
 Missing global options will be: inMemoryAsm=true, usingNamespaces=true, all other false or empty. Missing tempDir and appDirs don't change current values.
 To clear a value, use option=.

 File paths can contain environment variables and QM special folders.

 At run time, referenced assemblies are searched in:
   Standard locations (GAC, program's folder, program's subfolder with name of the assembly, etc). More info: <google>site:microsoft.com How the Runtime Locates Assemblies</google>.
   Program's subfolders specified in appDirs.
   Folder of the main assembly file (if Load used to load it from a file) and folders specified in searchDirs. However it is slower.
 Although full path can be specified in references, at run time it is not used (used only name).
 Or you can explicitly load private assemblies with <help>CsScript.Load</help>. Also need SetOptions("inMemoryAsm=false").

 See also: <RtOptions> (.NET CLR version)

 Errors: <.>

 EXAMPLE
 str options=
  debugConfig=true
  searchDirs=C:\Folder; %temp%\folder; $qm$\Folder
  compilerOptions=/warn:1
 CsScript x.SetOptions(options)
 x.Exec(code)


if flags&1 and !__cs_sett.i ;;if engine not loaded, store global options in __cs_sett, and call SetOptions when loaded
	lock __cs_sett
	if(!__cs_sett.i) __cs_sett.s=optionsList; ret

opt noerrorshere 1
opt nowarningshere 1

Init

x.SetOptions(flags&1 optionsList)
