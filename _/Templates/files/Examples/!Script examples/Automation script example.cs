/*/ runSingle true; /*/ //.
using Au;
;scriptt.setup(trayIcon: true); //;

//Click the ► button on the toolbar to run the script.

print.it("The programming language is C#.");
if (dialog.showYesNo("Run Notepad?", "The script will add some text and close Notepad after 2 s.")) {
	run.it(folders.System + @"Notepad.exe");
	var w = wnd.wait(5, active: true, "*- Notepad"); //to create this code can be used the Code menu
	50.ms();
	keys.sendt("some text");
	2.s();
	keys.send("Ctrl+Z"); //Undo
	w.Close();
}

string linkText = "Read about code editor features",
	linkUrl = "https://www.quickmacros.com/au/help/editor/Code editor.html";
print.it($"<><link \"{linkUrl}\">{linkText}</link>");
