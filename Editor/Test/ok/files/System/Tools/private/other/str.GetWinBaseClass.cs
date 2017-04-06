function$ hwnd

 Calls RealGetWindowClass.
 If the class is based on Button, Edit, Static, ComboBox, ListBox or ScrollBar, should get the base class name, but for most controls it does not work.


BSTR b.alloc(300)
int n=RealGetWindowClassW(hwnd b 300)
ansi(b -1 n)
ret this
