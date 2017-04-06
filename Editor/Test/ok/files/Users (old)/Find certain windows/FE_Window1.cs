 /
function# &hwnd $text [$cls] [$exe] [flags]

 Use with foreach to enumerate windows that match specified properties.
 All these properties are the same as with the win function.
 Retrieved is window handle. Window handle can be used with all window
 commands instead of window name.

 EXAMPLES
  enumerate windows that contains "Find" in window name
 int h
 foreach h "Find" FE_Window1
	 act h

  enumerate visible windows with class name "#32770" (dialogs, messages)
 int h
 foreach h "" FE_Window1 "#32770" "" 0x400
	 out _s.getwintext(h)


ARRAY(int) a
int i

if(!i)
	win text cls exe flags|0x8000 &FEW1_EnumProc &a

if(i=a.len) ret
hwnd=a[i]
i+1
ret 1
