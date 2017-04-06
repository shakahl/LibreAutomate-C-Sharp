function# $controlName [$text] [$classname] [flags]

 Finds control in .NET (Windows Forms) window.
 Returns its handle. If does not find, returns 0.

 controlName - control name (eg "Button1").
 other arguments - the same as with function child().


if(!m_hwnd) end ES_INIT
ARRAY(int) a
if(!child(text classname m_hwnd flags 0 0 a)) ret

int i h
str s
for(i 0 a.len)
	h=a[i]
	if(GetControlName(h s) and s~controlName) ret h

err+
