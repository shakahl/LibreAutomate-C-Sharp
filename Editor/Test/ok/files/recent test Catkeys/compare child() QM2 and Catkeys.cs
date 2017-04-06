int w=win("Quick Macros - ok - [Macro2759]" "QM_Editor")
int c=id(2053 w) ;;tool bar
int w1=win("Catkeys - Microsoft Visual Studio" "HwndWrapper[DefaultDomain;*")
int c1=id(1 w1) ;;push button 'Find All'
int w2=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
int c2=id(1 w2) ;;push button 'Find Next'
int w3=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
int c3=child("Solution Explorer" "SysTreeView32" w3) ;;outline
int w4=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
int c4=child("Menu Bar" "MsoCommandBar" w4) ;;menu bar 'Menu Bar'
int w5=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
int c5=child("" "ComboBox" w5 0x0 "id=102") ;;combo box

Wnd c6=w5.Control("", "ComboBox", new Wnd.ConProp(id: 102));
Wnd c6=w5.Control("", "ComboBox", 102);

Wnd c6=w5.Child("", "ComboBox", 102);
Wnd c6=w5.Child(102);
Wnd c6=w5.Child("", "ComboBox", 102, false, new Wnd.ChildProp(wfName: "myControl"));
Wnd c6=w5.Child("", "ComboBox", 102, false, new Wnd.ChildProp() { wfName = "myControl" });
Wnd c6=w5.Child("", "ComboBox", 102, false, "<prop wfName='myControl'/>" });
Wnd c6=w5.Child("", "ComboBox", false, "<prop id='102' wfName='myControl'/>" });
Wnd c6=w5.Child("", "ComboBox", false, "id='102' wfName='myControl'" });
Wnd c6=w5.Child("", "ComboBox", 102, false, null, (c, e) => { Out(c); }, 2);
Wnd c6=w5.Child("", "ComboBox", 102, f: (c, e) => { Out(c); }, matchIndex: 2);
Wnd c6=w5.Child("", "ComboBox", 102, f: e => { Out(e.w); }, matchIndex: 2);

