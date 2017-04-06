 STATIC

var w=Hwnd.Find("name", "class", "program", flags, "prop", matchIndex);
//or
var w=Wnd.Find("name", "class", "program", flags, "prop", matchIndex);

var w=Wnd.Control(hwndParent, "text", "class", flags, "prop", matchIndex);
var w=Wnd.Control(hwndParent, id); //searches at first direct child controls, then all
var w=Wnd.Control(hwndParent, id, bDirect); //searches only direct child controls
var w=Wnd.FocusedControl(hwndParent);

var w=Wnd.FromPosAny(x, y);
var w=Wnd.FromMouseAny();
var w=Wnd.FromPosToplevel(x, y);
var w=Wnd.FromMouseToplevel();
var w=Wnd.FromPosControl(x, y);
var w=Wnd.FromMouseControl();
var w=Wnd.FromAcc(a);

var w=Wnd.Wait.Exists(timeS, "name", "class", "program", flags, "prop", matchIndex); //or var w=Wait.WindowExists(timeS, "name", "class", "program", flags, "prop", matchIndex);
var w=Wnd.Wait.Visible(timeS, "name", "class", "program", flags, "prop", matchIndex);
var w=Wnd.Wait.Active(timeS, "name", "class", "program", flags, "prop", matchIndex);
//var w=Wnd.WaitControl.Exists //probably don't need here
//Same could be used for Acc: var a=Acc.Wait(timeMS, ...);

var w=Wnd.Get.Active(); //could be property, but better method
var w=Wnd.Get.Top();
var w=Wnd.Get.Next(ww);
var w=Wnd.Get.Previous(ww);
var w=Wnd.Get.First(ww);
var w=Wnd.Get.Last(ww);
var w=Wnd.Get.FirstChild(ww);
var w=Wnd.Get.TopMain();
var w=Wnd.Get.NextMain(ww);
var w=Wnd.Get.Shell;
var w=Wnd.Get.Desktop;
var w=Wnd.Get.Qm;
var w=Wnd.Get.QmEditControl;
//var w=Wnd.Get.Trigger; //don't need here
 var w=Wnd.Owner(ww); //maybe let be only instance members. Or both.
 var w=Wnd.DirectParent(ww);
 var w=Wnd.ToplevelParent(ww);

var a=Wnd.All(name=null, class=null, ...);
var a=Wnd.AllMain(flags=0);
var a=Wnd.AllControls(hwndParent, name=null, class=null, ...);
var a=Wnd.AllOfThisThread();

//bool methods
Wnd.Exists("name", "class", ...); //then can be: if(Wnd.Exists("name", "class")) instead of if(0!=Wnd.Find("name", "class"))
Wnd.ActiveIs("name", "class", ...); //can be used instead of: var w=Win.Active(); if(w.Match("name", "class", ...))
Wnd.MatchClass(hwnd, "class"); //QM2 WinTest. Let these 3 be non-static if Wnd is struct (classVar.Method() would be exception if classVar is null, but no exception if structVar is 0).
Wnd.Match(hwnd, "name", "class", "program", etc); //QM2 wintest; not error if hwnd is 0.
Wnd.MatchControl(hwnd, "text", "class", etc); //QM2 childtest; not error if hwnd is 0.

Wnd.Desktop.ShowDesktop(bOn);
Wnd.Desktop.MinimizeWindows(bOn);
Wnd.Desktop.CascadeWindows();
Wnd.Desktop.TileWindows(bVertically);

Wnd.CloseAllOf("name", ...);

Wnd.RegisterWindowMessage, Wnd.AllowActivateWindows
string s=Wnd.DecodeMessage (QM2 OutWinMsg)
...

 INSTANCE

w.Activate();
w.Close(); //posts WM_CLOSE
w.CloseLikeUser(); //posts WM_SYSCOMMAND
w.Close(double waitS); //error on timeout (unless waitS is 0). Also applies autodelay.
w.CloseLikeUser(double waitS); //error on timeout (unless waitS is 0). Also applies autodelay.
w.Maximize();
w.Minimize();
w.Restore();
 w.RestoreIfMinimized(); //maybe don't need. Instead can use Activate(); or if(w.IsMinimized()) w.Restore();
w.Move(x, y, flags=0);
w.Resize(width, height);
w.MoveResize(x, y, width, height, flags=0);
w.Show(true);
w.Enable(true);
w.Zorder(...);
w.SetWindowPos(); //works like API SetWindowPos
w.SetTransparent(opacity, color);
w.SetStyle(style, flags=0); //cannot use property-put because need flags
w.SetExStyle(style, flags=0);
w.CenterInScreen();
w.EnsureInScreen();
string s=w.PropertiesString(); //same as w.ToString/Out(w) but with formatting options and option to get all properties like in "Explore windows" dialog
w.SendMessage/SendNotifyMessage/SendMessageTimeout/PostMessage
...

//bool properties
w.IsActive;
w.IsFocusedControl;
w.IsVisible;
w.IsEnabled;
w.IsMaximized;
w.IsMinimized;
w.IsNotMinMax;
w.IsHung;
w.Is64Bit;
w.IsCloaked;
w.IsControl;
w.IsValid;

//other properties
w.Name; //get-set
w.Class;
w.ProgramName;
w.ProgramPath;
w.WindowRect;
w.ClientRect;
w.MonitorIndex;
w.MonitorHandle;
w.Style;
w.ExStyle;
w.Id;
 w.Owner; //for these 3 maybe use static functions for consistency. Or both.
 w.DirectParent;
 w.ToplevelParent;
w.ThreadId;
w.ProcessId;
w.ThreadProcessId(ref pid);

//Maybe also add these for of but().
//These removed because not used in any of my macros: but* (toggle).
//But better add these to another class.
w.ButtonClick(); //w is button handle
w.ButtonClick(id); //w is parent window handle. Can be used instead of: var b=Win.Control(id); b.ButtonClick();
w.ButtonCheck(bCheck, bClick); //bClick - click if unchecked
w.ButtonCheck(id, bCheck, bClick);
w.ButtonIsChecked();
w.ButtonIsChecked(id);


 RELATED

Out(Wnd); //shows handle, class, name
