/// To create code to find a UI element, use tool "Find UI element"; it's in the Code menu, hotkey Ctrl+Shift+E. It also can create action code, for example to click the element.

{ //click button "Properties" in a folder window; then wait 1 s
var w = wnd.find(1, cn: "CabinetWClass").Activate();
var e = w.Elm["SPLITBUTTON", "Properties", "class=NetUIHWND"].Find(1);
e.Invoke();
//e.MouseClick(); //use this when Invoke does not work
//e.MouseClickD(); //or this (double click)
//e.MouseClickR(); //or this (right click)
//e.PostClick(); //or this
//e.WebInvoke(); //use this with web page links when need to wait for new page
//e.JavaInvoke(); //use this with Java windows when Invoke does not work well
} 1.s();
{ //check checkbox "Read-only"; then wait 1 s
var w = wnd.find(1, "* Properties", "#32770");
var e = w.Elm["CHECKBOX", "Read-only"].Find(1);
e.Check(true);
} 1.s();
{ //select tab "Details"
var w = wnd.find(1, "* Properties", "#32770").Activate();
var e = w.Elm["PAGETAB", "Details"].Find(1);
e.Focus(true);
}

{ //expand folder "System32" in a folder window. At first expands its ancestors.
var w = wnd.find(1, cn: "CabinetWClass").Activate();
var e = w.Elm["TREEITEM", "This PC", "id=100"].Find(1);
e.Expand("*C:*|Windows|System32");
//wait 2 s and collapse 3 levels
2.s();
keys.send("Left*6");
}

{ //select combo box item "Baltic"
var w = wnd.find(1, "Font", "#32770").Activate();
var e = w.Elm["COMBOBOX", "Script:"].Find(1);
e.ComboSelect("Baltic");
}

/// To select a menu item, need to find and click each intermediate menu item. However usually it's better to use hotkeys and Alt+keys.

//hotkey and Alt+keys
wnd.find(0, "*- Notepad", "Notepad").Activate();
keys.send("Ctrl+V");
keys.send("Alt+E P");

//the same with elm functions
var wNotepad = wnd.find(0, "*- Notepad", "Notepad").Activate();
var eEdit = wNotepad.Elm["MENUITEM", "Edit"].Find(0);
eEdit.Invoke();
var wMenu = wnd.find(1, "", "#32768", wNotepad);
var ePaste = wMenu.Elm["MENUITEM", "Paste\tCtrl+V"].Find(1);
ePaste.Invoke();

/// Use <b>elm</b> functions to get menu item state (checked, disabled). This code checks menu Format -> Word Wrap. 

var wNotepad2 = wnd.find(0, "*- Notepad", "Notepad").Activate();
keys.send("Alt+O"); //Format
var wMenu2 = wnd.find(3, "", "#32768", wNotepad2);
var eWW = wMenu2.Elm["MENUITEM", "Word Wrap"].Find(1);
keys.send(eWW.IsChecked ? "Esc*2" : "W");

/// Wait until button "Apply" isn't disabled.
{
var w = wnd.find(1, "* Properties", "#32770");
var e = w.Elm["BUTTON", "Apply"].Find(1);
e.WaitFor(0, e => !e.IsDisabled);
print.it("enabled");
}

/// To find child/descendant elements, use <see cref="elm.Elm"/>.

{
var w = wnd.find(1, cn: "CabinetWClass").Activate();
var e = w.Elm["LIST", "Items View", "class=DirectUIHWND"].Find(1);
var e2 = e.Elm["LISTITEM", "c"].Find();
print.it(e2);
print.it("---");
var a1 = e.Elm["LISTITEM"].FindAll();
print.it(a1);
print.it("---");
var a2 = e.Elm.FindAll();
print.it(a2);
}

/// You can find more <b>elm</b> functions in the popup list that appears when you type . (dot) after a variable name or elm.

var em = elm.fromMouse(); //popup list when typed "elm."
string role = em.Role; //popup list when typed "em."

/// In some cases to find UI elements it's better to use <see cref="elmFinder"/>. A single instance can be used to find elements in multiple windows etc.

/// Find window that contains button "Apply" (UI element), and get the UI element too.

var f1 = new elmFinder("BUTTON", "Apply"); //or var f1 = elm.path["BUTTON", "Apply"];
var w1 = wnd.find(cn: "#32770", also: t => f1.In(t).Exists()); //or t => t.HasElm(f1)
print.it(w1);
print.it(f1.Result);
