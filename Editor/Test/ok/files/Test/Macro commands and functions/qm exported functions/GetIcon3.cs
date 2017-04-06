dll user32 #DrawIconEx hDC xLeft yTop hIcon cxWidth cyWidth istepIfAniCur hbrFlickerFreeDraw diFlags

str s
 s="$desktop$\test.txt"
 s="e:\graphics\MORICONS.DLL,48"
s="regedit.exe"
 s="mouse.ico"

int hi=GetIcon(s)
DrawIconEx(GetDC(id(2201 _hwndqm)) 2 2 hi 16 16 0 0 3)
