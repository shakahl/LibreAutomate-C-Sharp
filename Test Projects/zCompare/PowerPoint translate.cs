/// Translates multiple PowerPoint files. Uses words from the first table in a Word file.
/// Saves translated files in another folder. Does not modify source files.

#region begin_script
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
//using System.Reflection;
//using System.Windows.Forms;

using Catkeys.Automation;
using static Catkeys.Automation.NoClass;
using Catkeys.Winapi;

//.reference ...PowerPoint...

using PowerPoint = Microsoft.Office.Interop.PowerPoint;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
//note: need to download and install Office 2003 PIA (interop assemblies). VS fails to auto-generate PIAs from Office COM typelibs (maybe need to run VS as admin?). Maybe tlbimp.exe could generate?

public static class ThisScript {
static void Main(string[] args) {
#endregion

// change these if need. Here $documents$ is the Documents folder.
string WordFile=@"$documents$\dictionary.doc"; //Word file containing the table of words
string PowerPointFiles=@"$documents$\*.ppt"; //source files (all .ppt files in Documents folder). The files will not be modified.
string folderForTranslatedFiles=@"$documents$\translated"; //folder where to save translated files. The macro will create it if does not exist.

Output.Clear();
//____________________________________

// PowerPoint must not be running, it creates problems for this macro
if(Processes.NameToId("POWERPNT")!=0)
	MsgBoxEnd("Before running this macro, close PowerPoint application. Also close hidden PowerPoint processes in Task Manager.", "", "x");

// get table from Word file
List<S2> a=_GetTable(WordFile);
foreach(S2 t in a) Out($"{t.s1} = {t.s2}"); //debug-output words

// run hidden PowerPoint process
var ap=new PowerPoint.Application();

// create folder for translated files, if does not exist
folderForTranslatedFiles=folderForTranslatedFiles.ExpandPath(); //folderForTranslatedFiles=Files.ExpandPath(folderForTranslatedFiles);
Directory.CreateDirectory(folderForTranslatedFiles);

// repeat for each PowerPoint file
foreach(string path in Directory.EnumerateFiles(PowerPointFiles, "*.ppt")) {
	_TranslateFile(ap, path, a, folderForTranslatedFiles);
	}

//foreach(Files.FileProp d in Files.EnumFiles(PowerPointFiles)) {
//	string path=d.FullPath();
//	_TranslateFile(ap, path, a, folderForTranslatedFiles);
//	}
}

static void _TranslateFile(PowerPoint.Application ap, string path, List<S2> a, string folderForTranslatedFiles)
{

Out($"<><Z 0xff8080>{path}</Z>"); //debug-output PowerPoint files

// open PowerPoint file
PowerPoint.Presentation p=ap.Presentations.Open(path);

int nReplaced=0;

// repeat for each slide
foreach(PowerPoint.Slide k in p.Slides) {
	Out($"<><Z 0x80ffff>{k.Name}</Z>"); //debug-output slide name
	
	// repeat for each text box
	foreach(PowerPoint.Shape h in k.Shapes) {
		if(h.HasTextFrame==0) continue;
		if(h.TextFrame.HasText==0) continue;
		string s=h.TextFrame.TextRange.Text;
		//Out(s); //debug-output slide text
		
		// replace words in this text box
		foreach(S2 u in a) {
			s=s.FindReplace(u.s1, u.s2, 1|2|64);
			}
		h.TextFrame.TextRange.Text=s;
		}
	}

// save translated file
string newFilePath=Path.Combine(folderForTranslatedFiles, Path.GetFileName(folderForTranslatedFiles));
p.SaveAs(newFilePath);
p.Close();

Out(nReplaced);
}

struct S2 { public string s1, s2; };

static List<S2> _GetTable(string WordFile)
{
var a=new List<S2>();

// open Word file
var wa=new Word.Application();
Word.Document wd=wa.Documents.Open(WordFile.ExpandPath());

// repeat for each row in the first table
string s1, s2;
foreach(Word.Row r in wd.Tables[1].Rows) {
	// get cells
	s1=r.Cells[1].Range.Text; s1.Remove(s1.Length-2);
	s2=r.Cells[2].Range.Text; s2.Remove(s2.Length-2);
	if(s1.Length==0) continue;
	// add to array a
	a.Add(new S2() {s1="one", s2="two"});
//	int i=a.redim(-1); a[0 i]=s1; a[1 i]=s2
}
// quit; without this would remain hidden Word processes
wa.Quit();
return a;
}














	//static void TestExcel()
	//{
	//	Excel.Range r;
	//	r.Value;
	//}

}
