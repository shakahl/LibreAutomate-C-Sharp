function# $baseClassName $className wndProc [cbWndExtra]

 Registers window class that extends an existing class.
 Returns class atom.

 baseClassName - class name of an existing class that will be base of the new class.
 className - new class name.
 wndProc - address of window procedure (user-defined function that will be called to process messages).
   Template: menu File -> New -> Templates -> WndProc -> WndProc_Superclass.
 cbWndExtra - number of extra window bytes that can be accessed with SetWindowLong/GetWindowLong.
   Adds it to the number of extra bytes of the base class, which will be baseClassCbWndExtra member of this variable.

 REMARKS
 It is called <google "site:microsoft.com About Window Procedures, superclassing">superclassing</google>.
 Used to extend functionality of system control classes (Edit, SysListView32...) or global QM classes (eg Scintilla).
 The class is Unicode. The window procedure must call CallWindowProcW and handle Unicode messages and Unicode UTF-16 strings in their parameters.
 Should be used single global variable for a class. With CallWindowProcW use member baseClassWndProc of the variable.
 Does nothing if Superclass or Register already successfully called with this variable.
 Fails if the class does not exist in this process or is a local class. Error if fails.
 The new class is an app-local class (without CS_GLOBALCLASS style). With CreateWindowEx use _hinst for the hInstance paramater.


lock
if(atom) ret atom

WNDCLASSEXW w.cbSize=sizeof(w)
if(!GetClassInfoExW(0 @baseClassName &w)) end F"GetClassInfoEx failed. {_s.dllerror}"

int wp(w.lpfnWndProc) we(w.cbWndExtra)

w.hInstance=_hinst
w.style~CS_GLOBALCLASS
w.lpszClassName=@className
w.lpfnWndProc=wndProc
w.cbWndExtra+cbWndExtra

atom=RegisterClassExW(&w)
if(!atom) end F"RegisterClassEx failed. {_s.dllerror}"

baseClassWndProc=wp
baseClassCbWndExtra=we
ret atom
