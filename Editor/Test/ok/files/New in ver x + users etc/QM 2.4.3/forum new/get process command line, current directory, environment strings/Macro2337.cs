out
int w=win("Notepad" "Notepad")
 int w=win("Save As" "#32770")
 int w=win("app - Microsoft Visual Studio" "wndclass_desked_gsk")
 int w=win("C:\Windows\System32\java.exe" "ConsoleWindowClass")
 int w=win("PHPWebDriver" "CabinetWClass")
str s
GetProcessString w 1 s
 out s.len
 s.findreplace("" "[]" 32)
out s.len
out s
