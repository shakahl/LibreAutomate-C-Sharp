OUT

Windows: OutputDebugString

C: printf

C++: cout

.NET: Console.WriteLine

Java:
	System.out.println
	console.writeline

Python: print

Ruby: print (no newline), puts (adds newline), ios.write (formal, called by print and puts).

Perl: print

Delphi: writeln

Swift: print(text, bool appendNewline=true), NSLog (timestamp etc)

D: write, writeln etc. Or C printf.

Groovy: print[ln]

Catkeys:
	All these are the same, users can use which they like:
	Output.Write
	Out
	Write
	Print


MSGBOX

Windows: MessageBox

C/C++: MessageBox

C#: MessageBox.Show

VB.NET:
	MsgBox(text, options, title)
	MessageBox.Show

Java:
	JOptionPane class (https://docs.oracle.com/javase/7/docs/api/javax/swing/JOptionPane.html):
		showConfirmDialog 	Asks a confirming question, like yes/no/cancel.
		showInputDialog 	Prompt for some input.
		showMessageDialog 	Tell the user about something that has happened.
		showOptionDialog 	The Grand Unification of the above three.
	or
	Alert alert = new Alert(AlertType.INFORMATION); alert.setContentText(infoMessage); ... alert.showAndWait();

Python:
	No native function.
	tkMessageBox.FunctionName(title, message [, options]), where FunctionName:
		showinfo, showwarning, showerror, askquestion, askokcancel, askyesno, askretrycancel
	ctypes.windll.user32.MessageBoxA(0, "Your text", "Your title", 1)



IDEAS

InfoBox, InfoDialog
Show.Info/InfoOkCancel/InfoYesNo/...

switch(Show.MessageDialog("text", "title", MD.YesNo|MD.Warning, wnd, x, y, timeout)){
	case UserSaid.Yes: break;
	case UserSaid.No: break;
	case UserSaid.Timeout: break;
	//or
	case DialogResult.Yes: break;
	case DialogResult.No: break;
	case DialogResult.None: break;
}

MessageDialog(string text, string title=null, IB options=0, IWin32Window owner=null, int x=0, int y=0, int timeoutS=0)
MessageDialog(string text, string title, string options, IWin32Window owner=null, int x=0, int y=0, int timeoutS=0)

Or let MessageDialog be just a simple wrapper for MessageBox. If need x y timeout etc, use TaskDialog class.

MessageDialog(string text, string title=null, IB options=0, IWin32Window owner=null)
MessageDialog(string text, string title, string options, IWin32Window owner=null)

var t=new TaskDialog();
t.Title=""; t.BigText=""; t.SmallText=""; t.Owner=w; t.Flags=TDF.Flag; t.Icon=ic; t.Buttons=TDB.OK;
t.CustomButtons=new TDButton[]{{11, "one"},{12, "two"}}; t.DefaultButton=2;
t.RadioButtons=new TDButton[]{{11, "one"},{12, "two"}}; t.DefaultRadioButton=2;
t.OnLinkClick= linkIndex => Out(linkIndex);
t.X=111; t.X=222; t.TimeoutS=30;
...
int button=t.Show(); //or t.Show(ref checked)
Out(t.ResultRadioButton);

also static:
int button=Show.TaskDialog.Simple(w, title, bigText, smallText, buttons, icon);


QM2:
	int mes[-]($text [$caption] [$style])
	int mes[-]($text $caption MES'm)   ;;Message box with more features.
	 style: "[O|OC|YN|YNC|ARI|RC|CTE] [1|2|3] [?|!|x|i|q|v] [s] [a|n] [t]"
	MES ~style x y timeout default hwndowner
