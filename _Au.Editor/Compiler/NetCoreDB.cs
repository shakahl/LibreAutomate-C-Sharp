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
using System.Windows.Forms;
//using System.Drawing;
using System.Linq;

using Au;
using Au.Types;
using static Au.AStatic;

static class NetCoreDB
{
	/// <summary>
	/// Creates SQLite databases containing design-time assemblies and XML documentation files of all .NET Core runtimes of installed .NET Core SDKs.
	/// Creates with names ref.version.db (eg ref.3.0.0.db) and doc.version.db.
	/// We ship and at run time load databases of single version, named ref.db and doc.db. In the future should allow to download and use multiple versions.
	/// Also this function allows users to create databases from SDKs installed on their PC, but currently this feature is not exposed. Would need to add UI and exception handling.
	/// ref.db contains dlls from 'dotnet\packs' folder. They contain only metadata of public API, not all code like dlls in the 'dotnet\shared' folder.
	///		Why need it when we can load PortableExecutableReference from 'dotnet\shared' folder? Because:
	///			1. They are big and may add 100 MB of process memory. We need to load all, because cannot know which are actually used in various stages of compilation.
	///			2. When loading from dll files, Windows Defender makes it as slow as 2.5 s or more, unless the files already are in OS file buffers.
	///			3. Better compatibility. See https://github.com/dotnet/standard/blob/master/docs/history/evolution-of-design-time-assemblies.md
	///	doc.db contains XML documentation files of .NET Core assemblies. From the same 'dotnet\packs' folder.
	///		Why need it:
	///			1. Else users would have to download whole .NET Core SDK. Now need only runtimes.
	///			2. Parsed XML files can use eg 200 MB of process memory. Now we get doc of a single type/method/etc from database only when need; all other data is not in memory.
	/// </summary>
	public static void Create()
	{
		Cursor.Current = Cursors.WaitCursor;
		if(0 == AFolders.NetRuntimeBS.RegexReplace(@"(?i)\\shared\\(Microsoft\.NETCore\.App)\\.+", @"\packs\$1.Ref", out var refDir, 1)) throw new AuException();
		//Print(refDir);
		foreach(var f in AFile.EnumDirectory(refDir, FEFlags.UseRawPath)) { //for each version
			if(!f.IsDirectory) continue;
			_Create(refDir, f.Name);
		}
		Print("Done.");
	}

	static void _Create(string refDir, string version)
	{
		var dir1 = refDir + @"\" + version + @"\ref\netcoreapp" + version.RegexReplace(@"^\d+\.\d+\K.+", @"\", 1);
		//Print(dir1, AFile.ExistsAsDirectory(dir1));
		if(!AFile.ExistsAsDirectory(dir1, true)) throw new DirectoryNotFoundException("Not found: " + dir1);
		if(0 == dir1.RegexReplace(@"(?i)\\Microsoft\.\KNETCore(?=\.)", "WindowsDesktop", out string dir2, 1)) throw new AuException();
		//Print(dir2, AFile.ExistsAsDirectory(dir2));
		if(!AFile.ExistsAsDirectory(dir2, true)) throw new DirectoryNotFoundException("Not found: " + dir2);

		_CreateRef(version, dir1, dir2);
		_CreateDoc(version, dir1, dir2);
	}

	static void _CreateRef(string version, string dir1, string dir2)
	{
		var dbPath = AFolders.ThisAppBS + "ref." + version + ".db";
		AFile.Delete(dbPath);
		using var d = new ASqlite(dbPath);
		using var trans = d.Transaction();
		d.Execute("CREATE TABLE ref (name TEXT PRIMARY KEY, data BLOB)");
		using var statInsert = d.Statement("INSERT OR REPLACE INTO ref VALUES (?, ?)");

		_AddDir(dir1, "WindowsBase", "System.Drawing");
		_AddDir(dir2);

		trans.Commit();
		d.Execute("VACUUM");

		Print("Created " + dbPath);

		void _AddDir(string dir, params string[] skip)
		{
			foreach(var f in AFile.EnumDirectory(dir)) {
				if(f.IsDirectory) continue;
				if(!f.Name.Ends(".dll", true)) continue;
				var asmName = f.Name.RemoveSuffix(4);
				if(skip.Contains(asmName)) continue;
				_AddFile(asmName, f.FullPath);
				//break;
			}
		}

		void _AddFile(string asmName, string asmPath)
		{
			//Print(asmName);
			statInsert.Bind(1, asmName);
			statInsert.Bind(2, File.ReadAllBytes(asmPath));
			statInsert.Step();
			statInsert.Reset();
		}
	}

	static void _CreateDoc(string version, string dir1, string dir2)
	{
		var dbPath = AFolders.ThisAppBS + "doc." + version + ".db";
		AFile.Delete(dbPath);
		using var d = new ASqlite(dbPath, sql: "PRAGMA page_size = 8192;"); //8192 makes file smaller by 2-3 MB.
		using var trans = d.Transaction();
		d.Execute("CREATE TABLE doc (name TEXT PRIMARY KEY, xml TEXT)");
		using var statInsert = d.Statement("INSERT INTO doc VALUES (?, ?)");
		using var statDupl = d.Statement("SELECT xml FROM doc WHERE name=?");
		var haveRefs = new List<string>();
		var uniq = new Dictionary<string, string>(); //name -> asmName

		_AddDir(dir1, "WindowsBase");
		_AddDir(dir2);

		statInsert.BindAll(".", string.Join("\n", haveRefs)).Step();

		trans.Commit();
		d.Execute("VACUUM");

		Print("Created " + dbPath);

		void _AddDir(string dir, params string[] skip)
		{
			foreach(var f in AFile.EnumDirectory(dir)) {
				if(f.IsDirectory) continue;
				if(!f.Name.Ends(".xml", true)) continue;
				var asmName = f.Name.RemoveSuffix(4);
				if(skip.Contains(asmName)) continue;
				if(!AFile.ExistsAsFile(dir + asmName + ".dll")) {
					Print("<><c 0x808080>" + f.Name + "</c>");
					continue;
				}
				_AddFile(asmName, f.FullPath);
				//break;
			}
		}

		void _AddFile(string asmName, string xmlPath)
		{
			//Print(asmName);
			haveRefs.Add(asmName);
			var xr = AExtXml.LoadElem(xmlPath);
			foreach(var e in xr.Descendants("member")) {
				var name = e.Attr("name");

				//remove <remarks> and <example>. Does not save much space, because .NET xmls don't have it.
				foreach(var v in e.Descendants("remarks").ToArray()) v.Remove();
				foreach(var v in e.Descendants("example").ToArray()) v.Remove();

				using var reader = e.CreateReader();
				reader.MoveToContent();
				var xml = reader.ReadInnerXml();
				//Print(name, xml);

				if(uniq.TryGetValue(name, out var prevRef)) {
					if(!statDupl.Bind(1, name).Step()) throw new AuException();
					var prev = statDupl.GetText(0);
					if(xml != prev && asmName != "System.Linq") Print($"<>\t{name} already defined in {prevRef}\r\n<c 0xc000>{prev}</c>\r\n<c 0xff0000>{xml}</c>");
					statDupl.Reset();
				} else {
					statInsert.BindAll(name, xml).Step();
					uniq.Add(name, asmName);
				}
				statInsert.Reset();
			}
		}
	}

	//public static void UpdateIfNeed()
	//{
	//}
}
