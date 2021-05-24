/*/ runSingle true; /*/ //.
using Au;
;ATask.Setup(trayIcon: true); //;

//Click the ► button on the toolbar to run the script.

AOutput.Write("The programming language is C#.");
if (ADialog.ShowYesNo("Run Notepad?", "The script will add some text and close Notepad after 2 s.")) {
	AFile.Run(AFolders.System + @"Notepad.exe");
	var w = AWnd.Wait(5, active: true, "*- Notepad"); //to create this code can be used the Code menu
	50.ms();
	AKeys.Text("some text");
	2.s();
	AKeys.Key("Ctrl+Z"); //Undo
	w.Close();
}

string linkText = "Read about code editor features",
	linkUrl = "https://www.quickmacros.com/au/help/editor/Code editor.html";
AOutput.Write($"<><link \"{linkUrl}\">{linkText}</link>");
