//#define STANDARD_SCRIPT

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Reflection;
using Microsoft.Win32;
using System.Runtime.ExceptionServices;
//using System.Linq;
//using System.Xml.Linq;

using Au;
using Au.Types;
using static Au.NoClass;

namespace Au.Compiler
{
	/// <summary>
	/// Extracts C# file/project settings, references, etc from comments in C# code.
	/// </summary>
	/// <remarks>
	/// To compile C# code, often need various settings, more files, etc. In Visual Studio you can set it in project Properties and Solution Explorer. In Au you can set it in C# code as meta comments.
	/// Meta comments is a block of multiline comments that starts with <c>/* meta</c> and ends with <c>*/</c>. Must be at the very start of C# code. Example:
	/// <code><![CDATA[
	/// /* meta
	/// option1 value1
	/// option2 value2
	/// option2 value3
	/// //comments
	/// */
	/// ]]></code>
	/// Options can be separated with newlines or |. Option and values can be separated with spaces or =. Example: /* meta option1=value1 | option2=value2 */
	/// Options and values must match case, except filenames/paths. No "", no escaping.
	/// The same option can be several times with different values, for example to specify several references.
	/// All available options are in the examples below. The //comments in option lines are used only for documentation here; not allowed in real meta comments.
	/// When compiling multiple files (project, or using option 'c'), only the main file can contain all options. Other files can contain only 'r', 'c', 'resource'.
	/// </remarks>
	/// <example>
	/// <h3>References</h3>
	/// <code><![CDATA[
	/// r System.Assembly //.NET/GAC assembly reference. Without ".dll".
	/// r Other.Assembly.dll //other assembly reference. With ".dll" or ".exe". The file must be in the main Au folder or its subfolder Libraries.
	/// r assembly path.dll //other assembly reference using full path or path relative to the main Au folder. May be problem locating them at run time.
	/// r Alias=X.dll //assembly reference that can be used with C# keyword 'extern alias'.
	/// ]]></code>
	/// Don't need to specify these references: mscorlib, System, System.Core, System.Windows.Forms, System.Drawing, Au.dll.
	/// 
	/// <h3>Other C# files to compile together</h3>
	/// <code><![CDATA[
	/// c file.cs //file in this C# file's folder. It must be regular C# file (.cs), not C# script. Should not contain Main function.
	/// c folder\file.cs //path relative to this C# file's folder
	/// c .\folder\file.cs //the same as above
	/// c ..\folder\file.cs //path relative to the parent folder
	/// c \folder\file.cs //path relative to the collection folder
	/// ]]></code>
	/// The file must be in this collection. Or it can be a link (in collection) to an external file. The same is true with most other options.
	/// 
	/// <h3>References to libraries created in this collection</h3>
	/// <code><![CDATA[
	/// library folder\file.cs
	/// ]]></code>
	/// Compiles the .cs file or its project and uses the output dll file like with option r. It is like a "project reference" in Visual Studio.
	/// The .cs file must produce a dll file; see options outputType and outputPath below. The .cs file can be in a library project or not.
	/// 
	/// <h3>Files to add to managed resources</h3>
	/// <code><![CDATA[
	/// resource file.png //can be filename or relative path, like with 'c'
	/// resource file.txt /string //text file. Must be single space before /.
	/// resource file.csv /strings //CSV file containing multiple strings as 2-column CSV (name, value)
	/// ]]></code>
	/// Resource type depends on file extension and suffix:
	/// Suffix /string or /strings - string. Extension .png, .bmp, .jpg, .jpeg, .gif, .tif or .tiff - Bitmap. Extension .ico - Icon. Other - byte[].
	/// Examples of loading resources at run time:
	/// In meta comments: <c>resource \images\file.png</c>. Code: <c>var bitmap = Au.Util.Resources_.GetAppResource("file.png") as Bitmap;</c>
	/// In meta comments: <c>resource file.ico</c>. Code: <c>var icon = new Icon(Au.Util.Resources_.GetAppResource("qm.ico") as Icon, 16, 16);</c>
	/// In meta comments: <c>resource file.cur</c>. Code: <c>var cursor = Au.Util.Cursor_.LoadCursorFromMemory(Au.Util.Resources_.GetAppResource("file.cur") as byte[]);</c>
	/// 
	/// <h3>Settings used when compiling</h3>
	/// <code><![CDATA[
	/// debug true|false //if true (default), don't optimize code; also define preprocessor symbol DEBUG. It usually makes startup faster (faster JIT compiling), but low-level code slower.
	/// warningLevel 1 //compiler warning level, 0 (none) to 4 (all). Default: 4.
	/// disableWarnings 3009,162 //don't show these compiler warnings
	/// define SYMBOL1,SYMBOL2 //define preprocessor symbols that can be used with #if etc. These symbols are added implicitly: TRACE - always; DEBUG - if there is no option 'debug' false; EXE - if used options 'outputPath', which means that the assembly is normal exe file that runs not in a host process.
	/// preBuild file /arguments //run this script/app before compiling. More info below.
	/// postBuild file /arguments //run this script/app after compiling successfully. More info below.
	/// ]]></code>
	/// About options 'preBuild' and 'postBuild':
	/// The script/app runs in compiler's thread. Compiler waits and does not respond during that time. To stop compilation, let the script throw an exception.
	/// The script/app has variable string[] args. If there is no /arguments, args[0] is the output assembly file, full path. Else args contains the specified arguments, parsed like a command line. In arguments you can use these variables:
	/// $(outputFile) -  the output assembly file, full path; $(sourceFile) - the C# file, full path; $(source) - path of the C# file in collection, eg "\folder\file.cs"; $(outputPath) - meta option 'outputPath', default ""; $(debug) - meta option 'debug', default "true".
	/// 
	/// <h3>Settings used to run the compiled script or app. Here a|b|c means a or b or c.</h3>
	/// <code><![CDATA[
	/// runMode supervised|unattendedSingle|unattendedMulti|editorThread - whether the task can be easily ended, multiple instances, etc. Default: supervised. More info below.
	/// ifRunning cancel|wait|restart|restartOrWait //whether/how to start new task if a task is running. Default: cancel. More info below.
	/// uac same|user|admin //UAC integrity level of the script process. Default: same. More info below.
	/// prefer32bit true|false //if true, the task process is 32-bit even on 64-bit OS. It can use 32-bit dlls and AnyCPU dlls, but not 64-bit dlls. Default: false.
	/// ]]></code>
	/// Here word "task" is used for "script or app that is running or should start".
	/// Options 'runMode', 'ifRunning' and 'uac' are applied only when the task is started from Au editor, not when it runs as independent exe program.
	/// 
	/// About runMode:
	/// supervised (default) - the task has these properties: 1. Cannot run simultaneously with other supervised tasks (see also option 'ifRunning'); 2. Can be ended with the "End task" hotkey; 3. Changes the tray icon.
	/// unattendedSingle - unattended single-instance task. More info below. See also option 'ifRunning'.
	/// unattendedMulti - unattended multi-instance task. Multiple task instances can run simultaneously. Option 'ifRunning' not used.
	/// editorThread - run the task in the main UI thread of the editor process. This is an advanced option, and rarely used. Can be used to create editor extensions. The user cannot see and end the task. Creates memory leaks when executing recompiled assemblies (eg after editing the script/app), because old assembly versions cannot be unloaded until process exits.
	/// 
	/// Unattended tasks don't have the 1-3 properties of supervised tasks. Such tasks are used: to run simultaneously with any tasks (supervised, unattended, other instances of self); to protect the task from the "End task" hotkey; as background tasks that wait for some event or watch for some condition; as collections of global or application-specific methods that can be executed with method triggers, toolbars and autotexts.
	/// 
	/// About ifRunning:
	/// Defines whether/how to start new task if a task is running. What is "another task": if runMode supervised (default), it is "a supervised task"; if runMode unattendedSingle, it is "another instance of this task"; with other run modes this option cannot be used.
	/// unspecified (default) - don't run. Print a warning.
	/// cancel - don't run. Don't print a warning.
	/// wait - run later, when that task ends.
	/// restart - if that task is of this script/app, end it and run. Else like 'unspecified' (when both tasks are supervised).
	/// restartOrWait - if that task is of this script/app, end it and run. Else like 'wait'. Cannot be used with unattended tasks.
	/// 
	/// About uac:
	/// same (default) - the task process has the same UAC integrity level as of the editor process, probably Medium.
	/// user - the task process has UAC integrity level Medium, like most applications. Such tasks cannot automate windows of processes that run as administrator, cannot modify some directories and registry keys, etc.
	/// admin - the task process has UAC integrity level High, also known as "runs as administrator" or "elevated". Normally it starts without a UAC consent screen/dialog. Such tasks don't have the above limitations, but can have some others, for example cannot automate some apps through COM.
	/// 
	/// <h3>Other</h3>
	/// <code><![CDATA[
	/// config app.config //use this configuration file when the output assembly file is executed. Can be filename or relative path, like with 'c'. The file is copied to the output directory unmodified but renamed to match the assembly name. This option cannot be used with dll. If not specified, will be used host program's config file.
	/// ]]></code>
	/// 
	/// <h3>To create .exe file, at least option 'outputPath' must be specified</h3>
	/// <code><![CDATA[
	/// outputPath path //create output files (.exe, .dll, etc) in this directory. Can be full path or relative path like with 'c'. If not specified, and 'outputType' is dll, uses %Folders.ThisApp%\Libraries.
	/// outputType app|console|dll //type of the output assembly file. Default: app (Windows application) for scripts, dll for .cs files. Scripts cannot be dll.
	/// icon file.ico //icon of the .exe/.dll file. Can be filename or relative path, like with 'c'.
	/// manifest file.manifest //manifest file of the .exe file. Can be filename or relative path, like with 'c'.
	/// resFile file.res //file containing native resources to add to the .exe/.dll file. Can be filename or relative path, like with 'c'.
	/// sign file.snk //sign the output .exe/.dll file with a strong name using this .snk file. Can be filename or relative path, like with 'c'. 
	/// xmlDoc file.xml //create XML documentation file from XML comments. The file argument is optional. If it is not full path, creates in the 'outputPath' directory.
	/// ]]></code>
	/// If used option 'outputPath', or if 'outputType' is dll, creates .exe or .dll file, named like this C# file. Else creates a temporary file in subfolder ".temp" of the collection folder.
	/// 
	/// Full path can be used with 'r', 'outputPath' and 'xmlDoc'. It can start with and environment variable or special folder name in %%, like <c>%Folders.ThisAppDocuments%\file.exe</c>.
	/// Files used with other options ('c', 'resource' etc) must be in this collection, as local files or links to external files.
	/// 
	/// About native resources:
	/// If option 'resFile' specified, adds resources from the file, and cannot add other resources; error if also specified 'icon' or 'manifest'.
	/// If 'manifest' and 'resFile' not specified when creating .exe file, adds manifest from file "default.exe.manifest" in the main Au folder, if exists.
	/// If 'resFile' not specified when creating .exe or .dll file, adds version resource, with values collected from attributes such as [assembly: AssemblyVersion("...")]; see how it is in Visual Studio projects, in file Properties\AssemblyInfo.cs.
	/// 
	/// About thread COM apartment type:
	/// For scripts it is STA, and cannot be changed.
	/// For apps it is STA if the Main function has [STAThread] attribute; or if using meta option runMode editorThread. Else it is MTA.
	/// </example>
	class MetaComments
	{
		/// <summary>
		/// Name of the main C# file, without ".cs".
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// True if the main C# file is C# script, not regular C# file.
		/// </summary>
		public bool IsScript { get; private set; }

		/// <summary>
		/// Meta option 'debug'.
		/// Default: <see cref="DefaultIsDebug"/> (default true).
		/// </summary>
		public bool IsDebug { get; private set; }

		/// <summary>
		/// Gets or sets default meta option 'debug' value. Initially true.
		/// </summary>
		public static bool DefaultIsDebug { get; set; } = true;

		/// <summary>
		/// Meta option 'define'.
		/// Default: <see cref="DefaultDefines"/> (default { "TRACE" }).
		/// </summary>
		public List<string> Defines { get; private set; }

		/// <summary>
		/// Gets or sets default meta option 'define' value. Initially { "TRACE" }.
		/// </summary>
		public static List<string> DefaultDefines { get; set; } = new List<string> { "TRACE" };

		/// <summary>
		/// Meta option 'warningLevel'.
		/// Default: <see cref="DefaultWarningLevel"/> (default 4).
		/// </summary>
		public int WarningLevel { get; private set; }

		/// <summary>
		/// Gets or sets default meta option 'warningLevel' value. Initially 4.
		/// </summary>
		public static int DefaultWarningLevel { get; set; } = 4;

		/// <summary>
		/// Meta option 'disableWarnings'.
		/// Default: <see cref="DefaultDisableWarnings"/> (default null).
		/// </summary>
		public List<string> DisableWarnings { get; private set; }

		/// <summary>
		/// Gets or sets default meta option 'disableWarnings' value. Initially null.
		/// </summary>
		public static List<string> DefaultDisableWarnings { get; set; }

		/// <summary>
		/// Meta option 'config'.
		/// </summary>
		public IWorkspaceFile ConfigFile { get; private set; }

		/// <summary>
		/// All meta errors of all files. Includes meta syntax errors, file 'not found' errors, exceptions.
		/// </summary>
		public ErrBuilder Errors { get; private set; }

		/// <summary>
		/// Default references and unique references added through meta option 'r' in all C# files of this compilation.
		/// Use References.<see cref="MetaReferences.Refs"/>.
		/// </summary>
		public MetaReferences References { get; private set; }

		/// <summary>
		/// All C# files of this compilation.
		/// The main C# file, then other files of its project, and at the end all unique C# files added through meta option 'c' (see <see cref="CountC"/>).
		/// </summary>
		public List<MetaCSharpFile> Files { get; private set; }

		List<IWorkspaceFile> _filesC; //files added through meta option 'c'. Finally parsed and added to Files.

		/// <summary>
		/// Count of files added through meta option 'c'. They are at the end of <see cref="Files"/>.
		/// </summary>
		public int CountC => _filesC?.Count ?? 0;

		/// <summary>
		/// Unique resource files added through meta option 'resource' in all C# files of this compilation.
		/// null if none.
		/// </summary>
		public List<MetaFileAndString> Resources { get; private set; }

		/// <summary>
		/// Meta option 'library'.
		/// null if none.
		/// </summary>
		public List<IWorkspaceFile> Libraries { get; private set; }

		/// <summary>
		/// Meta option 'preBuild'.
		/// </summary>
		public MetaFileAndString PreBuild { get; private set; }

		/// <summary>
		/// Meta option 'postBuild'.
		/// </summary>
		public MetaFileAndString PostBuild { get; private set; }

		/// <summary>
		/// Meta option 'runMode'.
		/// Default: supervised.
		/// </summary>
		public ERunMode RunMode { get; private set; }

		/// <summary>
		/// Meta option 'ifRunning'.
		/// Default: 'unspecified' ('cancel' + warning).
		/// </summary>
		public EIfRunning IfRunning { get; private set; }

		/// <summary>
		/// Meta option 'uac'.
		/// Default: same.
		/// </summary>
		public EUac Uac { get; private set; }

		/// <summary>
		/// Meta option 'prefer32bit'.
		/// Default: false.
		/// </summary>
		public bool Prefer32Bit { get; private set; }

		/// <summary>
		/// Meta option 'icon'.
		/// </summary>
		public IWorkspaceFile IconFile { get; private set; }

		/// <summary>
		/// Meta option 'manifest'.
		/// </summary>
		public IWorkspaceFile ManifestFile { get; private set; }

		/// <summary>
		/// Meta option 'res'.
		/// </summary>
		public IWorkspaceFile ResFile { get; private set; }

		/// <summary>
		/// Meta option 'outputPath'.
		/// Default: null.
		/// </summary>
		public string OutputPath { get; private set; }

		/// <summary>
		/// Meta option 'outputType'.
		/// Default: exe.
		/// </summary>
		public EOutputType OutputType { get; private set; }

		/// <summary>
		/// Gets default meta option 'outputType' value. It is app if isScript, else dll.
		/// </summary>
		public static EOutputType DefaultOutputType(bool isScript) => isScript ? EOutputType.app : EOutputType.dll;

		/// <summary>
		/// Meta option 'sign'.
		/// </summary>
		public IWorkspaceFile SignFile { get; private set; }

		/// <summary>
		/// Meta 'xmlDoc'.
		/// Default: null.
		/// </summary>
		public string XmlDocFile { get; private set; }

		/// <summary>
		/// If there is meta, gets character position after */. Else 0.
		/// </summary>
		public int EndOfMeta { get; private set; }

		EMPFlags _flags;

		/// <summary>
		/// Extracts meta comments from all C# files of this compilation, including project files and files added through meta option 'c'.
		/// Returns false if there are errors. Then use <see cref="Errors"/>.
		/// </summary>
		/// <param name="f">Main C# file. If projFolder not null, must be the main file of the project.</param>
		/// <param name="projFolder">Project folder of the main file, or null if it is not in a project.</param>
		/// <param name="flags"></param>
		public bool Parse(IWorkspaceFile f, IWorkspaceFile projFolder, EMPFlags flags)
		{
			Debug.Assert(Errors == null); //cannot be called multiple times
			Errors = new ErrBuilder();
			_flags = flags;

			_ParseFile(f, true);

			if(projFolder != null) {
				foreach(var ff in projFolder.IcfEnumProjectFiles(f)) _ParseFile(ff, false);
			}

			if(_filesC != null) {
				foreach(var ff in _filesC) {
					if(Files.Exists(o => o.f == ff)) continue;
					_ParseFile(ff, false);
				}
			}

			_FinalCheckOptions();

			if(!Errors.IsEmpty) {
				if(flags.Has_(EMPFlags.PrintErrors)) Errors.PrintAll();
				return false;
			}

			if(IsDebug && !Defines.Contains("DEBUG")) Defines.Add("DEBUG");
			if(OutputPath != null && !Defines.Contains("EXE")) Defines.Add("EXE");

			return true;
		}

		/// <summary>
		/// Extracts meta comments from a single C# file.
		/// </summary>
		/// <param name="f"></param>
		/// <param name="isMain">If false, it is a file added through meta option 'c'.</param>
		public void _ParseFile(IWorkspaceFile f, bool isMain)
		{
			string code = File_.LoadText(f.FilePath);
			bool isScript = f.IcfIsScript;

			if(_isMain = isMain) {
				string name = f.Name;
				if(!isScript) name = name.Remove(name.Length - 3);
				Name = name;
				IsScript = isScript;

				IsDebug = DefaultIsDebug;
				WarningLevel = DefaultWarningLevel;
				DisableWarnings = DefaultDisableWarnings != null ? new List<string>(DefaultDisableWarnings) : new List<string>();
				Defines = DefaultDefines != null ? new List<string>(DefaultDefines) : new List<string>();
				OutputType = DefaultOutputType(isScript);

				Files = new List<MetaCSharpFile>();
				References = new MetaReferences();
			}

			Files.Add(new MetaCSharpFile(f, code));

			_fn = f;
			_code = code;

			if(code.Length < 10 || !code.StartsWith_("/* meta") || code[7] > ' ') return;
			int i = 8, iTo = code.IndexOf_("*/", i); if(iTo < 0) return;
			if(isMain) EndOfMeta = iTo + 2;

			foreach(var seg in code.Segments_(i, iTo - i, "\r\n|", SegFlags.NoEmpty)) {
				seg.Trim();
				if(seg.Length != 0 && !seg.StartsWith("//")) _ParseOption(code, seg);
			}
		}

		bool _isMain;
		IWorkspaceFile _fn;
		string _code;

		void _ParseOption(string code, in StringSegment seg)
		{
			//simply split to get key and value
			bool _IsSep(char c1) => c1 <= ' ' || c1 == '=';
			int i = seg.Offset, iKey = i, iEnd = seg.EndOffset;
			while(i < iEnd && !_IsSep(code[i])) i++;
			string key = code.Substring(iKey, i - iKey);
			while(i < iEnd && _IsSep(code[i])) i++;
			int iValue = i;
			string value = code.Substring(iValue, iEnd - iValue);
			//Print($"'{key}'  '{value}'");

			if(value.Length == 0) {
				switch(key) {
				case "outputPath": case "xmlDoc": break;
				default: _Error(iValue, "value cannot be empty"); return;
				}
			}
			g1:
			switch(key) {
			case "r":
				try {
					if(!References.Resolve(value)) {
						_Error(iValue, "reference assembly not found: " + value); //FUTURE: need more info, or link to Help
					}
				}
				catch(Exception e) {
					_Error(iValue, "exception: " + e.Message); //unlikely. If bad format, will be error later, without position info.
				}
				return;
			case "c":
				if(!value.EndsWithI_(".cs")) { _Error(iValue, "must be .cs file"); return; }
				var ff = _GetFile(value, iValue); if(ff == null) return;
				if(_filesC == null) _filesC = new List<IWorkspaceFile>();
				else if(_filesC.Contains(ff)) return;
				_filesC.Add(ff);
				return;
			case "resource":
				var fs1 = _GetFileAndString(value, iValue); if(fs1.f == null) return;
				if(Resources == null) Resources = new List<MetaFileAndString>();
				else if(Resources.Exists(o => o.f == fs1.f && o.s == fs1.s)) return;
				Resources.Add(fs1);
				return;
				//FUTURE: support wildcard:
				// resource *.png //add to managed resources all matching files in this C# file's folder.
				// resource Resources\*.png //add to managed resources all matching files in a subfolder of this C# file's folder.
				//Support folder (add all in folder).
			}
			if(!_isMain) {
				_Error(iKey, $"in this file only these options can be used: 'r', 'c', 'resource'. Others only in the main file of the compilation - {Files[0].f.Name}.");
				return;
			}

			switch(key) {
			case "library":
				if(_Library(ref value, iValue)) { key = "r"; goto g1; }
				break;
			case "debug":
				if(_TrueFalse(out bool isDebug, value, iValue)) IsDebug = isDebug;
				break;
			case "warningLevel":
				int wl = value.ToInt_();
				if(wl >= 0 && wl <= 4) WarningLevel = wl;
				else _Error(iValue, "must be 0 - 4");
				break;
			case "disableWarnings":
				DisableWarnings.AddRange(value.Split_(", ", SegFlags.NoEmpty));
				break;
			case "define":
				Defines.AddRange(value.Split_(", ", SegFlags.NoEmpty));
				break;
			case "preBuild":
				PreBuild = _GetFileAndString(value, iValue);
				break;
			case "postBuild":
				PostBuild = _GetFileAndString(value, iValue);
				break;
			case "outputType":
				_cantRunInEditor = true;
				if(_Enum(out EOutputType ot, value, iValue)) {
					OutputType = ot;
					if(ot == EOutputType.dll) {
						if(IsScript) _Error(iValue, "cannot create dll from script");
						else if(OutputPath == null) OutputPath = Folders.ThisAppBS + "Libraries";
					}
				}
				break;
			case "outputPath":
				_cantRunInEditor = true;
#if STANDARD_SCRIPT
				//scripts can be .exe, but we don't allow it, because there is no way to set args, [STAThread], triggers.
				if(IsScript) _Error(iKey, "scripts cannot have option outputPath. If want to create .exe, use App or App/Script project (menu -> File -> New)."); else
#endif
				OutputPath = _GetOutPath(value, iValue); //and creates directory if need
				break;
			case "runMode":
				_cantBeDll = true;
				if(_Enum(out ERunMode rm, value, iValue)) RunMode = rm;
				break;
			case "ifRunning":
				_cantBeDll = _cantRunInEditor = _cantRunMulti = true;
				if(_Enum(out EIfRunning ifR, value, iValue)) IfRunning = ifR;
				break;
			case "uac":
				_cantBeDll = _cantRunInEditor = true;
				if(_Enum(out EUac uac, value, iValue)) Uac = uac;
				break;
			case "prefer32bit":
				_cantBeDll = _cantRunInEditor = true;
				if(_TrueFalse(out bool is32, value, iValue)) Prefer32Bit = is32;
				break;
			case "config":
				_cantBeDll = _cantRunInEditor = true;
				ConfigFile = _GetFile(value, iValue);
				break;
			case "manifest":
				_cantBeDll = _cantRunInEditor = true;
				if(ManifestFile != null) _Error(iKey, "cannot add multiple manifests");
				else ManifestFile = _GetFile(value, iValue);
				break;
			case "icon":
				if(IconFile != null) _Error(iKey, "cannot add multiple icons");
				else IconFile = _GetFile(value, iValue);
				break;
			case "resFile":
				if(ResFile != null) _Error(iKey, "cannot add multiple res files");
				else ResFile = _GetFile(value, iValue);
				break;
			case "sign":
				SignFile = _GetFile(value, iValue);
				break;
			case "xmlDoc":
				XmlDocFile = value.Length != 0 ? value : (Name + ".xml");
				break;
			//case "targetFramework": //need to use references of that framework? Where to get them at run time? Or just add [assembly: TargetFramework(...)]?
			//	break;
			//case "version": //will be auto-created from [assembly: AssemblyVersion] etc
			//	break;
			//case "include": //FUTURE
			//	// include file //include options from this file. Can be filename or relative path, like with 'c'.
			//	//CONSIDER: also add "using", it can be useful with "include"; or add regular usings from the included file; also [assembly: ] and [module: ].
			//	//CONSIDER: allow to have one .cs file to be compiled with all. Eg it can contain [module: DefaultCharSet(CharSet.Unicode)].
			//	break;
			default:
				_Error(iKey, "unknown meta option");
				break;
			}

			//rejected:
			//pdb true|false //if true, creates .pdb file. Note: if false, can be difficult to find unhandled exception place in code. Default: true.
			//skip  //don't compile this file when compiling multiple files ('c'). Must be the first line.
			//c  //also compile all other .cs files in the same folder and subfolders. This option can be only in the main file, not in files compiled because of 'c'. Script files are not included.
			//using Namespace; //\\r //.NET/GAC assembly reference, using assembly name = Namespace
			//using Namespace; //\\r .dll //other assembly reference, using file name = Namespace.dll. The file must be in the main Au folder or its subfolder Libraries.
		}

		bool _Error(int pos, string s)
		{
			Errors.AddError(_fn, _code, pos, "error in meta: " + s);
			return false;
		}

		bool _TrueFalse(out bool b, string s, int errPos)
		{
			b = false;
			switch(s) {
			case "true": b = true; break;
			case "false": break;
			default: return _Error(errPos, "must be true or false");
			}
			return true;
		}

		bool _Enum<T>(out T result, string s, int errPos) where T : Enum
		{
			result = default;
			var ty = typeof(T); var a = ty.GetFields();
			foreach(var v in a) if(v.Name == s) { result = (T)v.GetRawConstantValue(); return true; }
			_Error(errPos, "must be one of: " + string.Join(", ", Enum.GetNames(ty)));
			return false;
		}

		IWorkspaceFile _GetFile(string s, int errPos)
		{
			var f = _fn.IcfFindRelative(s, false);
			if(f == null) { _Error(errPos, $"file '{s}' does not exist in this collection"); return null; }
			if(!Au.File_.ExistsAsFile(s = f.FilePath, true)) { _Error(errPos, "file does not exist: " + s); return null; }
			return f;
		}

		MetaFileAndString _GetFileAndString(string s, int errPos)
		{
			string s2 = null;
			int i = s.IndexOf_(" /");
			if(i > 0) {
				s2 = s.Substring(i + 2);
				s = s.Remove(i);
			}
			return new MetaFileAndString(_GetFile(s, errPos), s2);
		}

		string _GetOutPath(string s, int errPos)
		{
			s = s.TrimEnd('\\');
			if(!Path_.IsFullPathExpandEnvVar(ref s)) {
				if(s.StartsWith_('\\')) s = _fn.IcfWorkspace.IcfFilesDirectory + s;
				else s = Path_.GetDirectoryPath(_fn.FilePath, true) + s;
			}
			s = Path_.LibNormalize(s, noExpandEV: true);

			Au.File_.CreateDirectory(s);
			return s;
		}

		bool _Library(ref string value, int iValue)
		{
			var f = _GetFile(value, iValue); if(f == null) return false;
			if(f.IcfFindProject(out var projFolder, out var projMain)) f = projMain;
			if(!Compiler.Compile(false, out var r, f, projFolder)) return _Error(iValue, "failed to compile library");
			if(r.file == null) return _Error(iValue, "the main library code file must have meta outputPath or outputType dll");
			//Print(r.outputType, r.file);
			if(r.outputType != EOutputType.dll) return false; //not error, just compile that script/app
			value = r.file;
			(Libraries ?? (Libraries = new List<IWorkspaceFile>())).Add(f); //for XCompiled
			return true;
		}

		bool _FinalCheckOptions()
		{
			if(ResFile != null) {
				if(IconFile != null) return _Error(0, "cannot add both res file and icon");
				if(ManifestFile != null) return _Error(0, "cannot add both res file and manifest");
			}

			if(OutputType == EOutputType.dll && _cantBeDll)
				return _Error(0, "with outputType dll cannot use runMode, ifRunning, uac, prefer32bit, config, manifest");

			switch(RunMode) {
			case ERunMode.unattendedMulti when _cantRunMulti:
				return _Error(0, "with runMode unattendedMulti cannot use ifRunning");
			case ERunMode.unattendedSingle when IfRunning == EIfRunning.restartOrWait:
				return _Error(0, "with runMode unattendedSingle cannot use ifRunning restartOrWait. Use ifRunning restart.");
			case ERunMode.editorThread when _cantRunInEditor:
				return _Error(0, "with runMode editorThread cannot use outputType, outputPath, ifRunning, uac, prefer32bit, config, manifest");
			}

			if(RunMode == ERunMode.editorThread && _cantRunInEditor)
				return _Error(0, "with runMode editorThread cannot use outputType, outputPath, ifRunning, uac, prefer32bit, config, manifest");

			//if(OutputPath == null && OutputType!= EOutputType.dll) {
			//	//FUTURE: show warning if used non-.NET/GAC/Au refs
			//}

			return true;
		}

		bool _cantBeDll, _cantRunInEditor, _cantRunMulti;
	}

	struct MetaCSharpFile
	{
		public IWorkspaceFile f;
		public string code;

		public MetaCSharpFile(IWorkspaceFile f, string code) { this.f = f; this.code = code; }
	}

	struct MetaFileAndString
	{
		public IWorkspaceFile f;
		public string s;

		public MetaFileAndString(IWorkspaceFile f, string s) { this.f = f; this.s = s; }
	}

	public enum EOutputType { app, console, dll }

	public enum EUac { same, user, admin }

	public enum ERunMode { supervised, unattendedSingle, unattendedMulti, editorThread }

	public enum EIfRunning { unspecified, cancel, wait, restart, restartOrWait }

	/// <summary>
	/// Flags for <see cref="MetaComments.Parse"/>
	/// </summary>
	[Flags]
	public enum EMPFlags
	{
		/// <summary>
		/// Call <see cref="ErrBuilder.PrintAll"/>.
		/// </summary>
		PrintErrors = 1,
	}
}
