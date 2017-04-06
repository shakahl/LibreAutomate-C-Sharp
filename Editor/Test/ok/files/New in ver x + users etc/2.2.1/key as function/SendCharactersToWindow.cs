 /
function hwnd $_string

 Sends the string to a child window using WM_CHAR messages.
 The window can be inactive.

 hwnd - child window handle.

 EXAMPLE
 int hwnd=id(15 "Notepad")
 SendCharactersToWindow hwnd "Line1[]Line2"


str s=_string
s.findreplace("[]" "[13]")
s.unicode
word* w=s
for(_i 0 s.len/2) PostMessageW hwnd WM_CHAR w[_i] 0
