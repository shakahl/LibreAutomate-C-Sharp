function# $name [hwnd]

Close
if(_winver>=0x501) handle=OpenThemeData(hwnd @name)
ret handle
