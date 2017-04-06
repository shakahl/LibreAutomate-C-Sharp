function'WTI* $txt [flags] [matchIndex] ;;flags: 0 partial, 1 full, 2 with *?, 3 regexp, 4 case insens., 8 +invisible, 0x100 capture always, 0x1000 error if not found

 Finds a text item.
 Returns address of internal variable that contains item properties.
 If not found, returns 0 or throws error (flag 0x1000).

 txt - text to find.
 flags:
   1-4 - txt match mode (see above). By default, txt can be partial and must match case. Flags 0-3 cannot be used together.
   8 - don't exclude items that are drawn but invisible, for example scrolled somewhere or covered by other child windows.
   0x100 - always call Capture(). Read more in remarks.
   0x1000 - error if not found.
 matchIndex - 1-based index of matching item. If omitted, 0 or 1, finds first item; 2 second, and so on.

 REMARKS
 If flag 0x100 not used, uses results of previous capturing, if available. Calls Capture() only if not already captured by Find(), Capture() or Wait(), and also after Init().
 The return value becomes invalid when the variable is destroyed or when a text capturing function called again.
 Init() must be called to set target window.
 Tip: If need to evaluate more properties, eg color, use a callback function. Set it with SetCallback().

 EXAMPLES
 out
 int w=id(15 win("Notepad" "Notepad")) ;;get handle of Notepad edit control
 WindowText x.Init(w)
 
  if text "findme" exists...
 if x.Find("findme")
	 out "text ''findme'' found"
 
  click text "findme"
 x.Mouse(1 x.Find("findme"))
 
  show rectangle of text "findme"
 WTI* t=x.Find("findme" 0x1000)
 RECT rv=t.rv; DpiMapWindowPoints t.hwnd 0 +&rv 2 ;;from t.hwnd client to screen
 OnScreenRect 1 &rv; 1; OnScreenRect 2 &rv


if(!m_captured or flags&0x100) Capture
if(!n) goto gr

int i ok f(flags&3) ins(flags&4!0)

if(f=3) str rx; findrx("" txt 0 ins|128 rx)

for i 0 n
	WTI& t=a[i]
	sel f
		case 0 ok=find(t.txt txt 0 ins)>=0
		case 1 ok=!StrCompare(t.txt txt ins)
		case 2 ok=matchw(t.txt txt ins)
		case 3 ok=findrx(t.txt rx 0 ins)>=0
	if ok
		if(t.flags&WTI_INVISIBLE and flags&8=0) continue
		if(m_cbFunc and call(m_cbFunc &t m_cbParam)) continue
		matchIndex-1; if(matchIndex>0) continue
		ret &t

err+ end _error
 gr
if(flags&0x1000) end ERR_OBJECT
