 /
function htb $buttons imagelist [wndstyle] [btnstyle] [btnstate] [ttTime] [tbExStyle] ;;buttons line syntax: id [imageindex] [label] [style] [state] [data]

 Initializes a toolbar control.
 Adds buttons, sets imagelist, optionally adds styles, and autosizes.

 htb - toolbar control handle.
 buttons - list of buttons.
   Line syntax: id [imageindex] [label] [style] [state] [data]
   id - button id. If 0, adds separator. To receive button click event, in dialog procedure under sel wParam insert case statement with the id. Don't use id 1, 2 and button control ids.
   imageindex - image index in imagelist. Can be -2 for no image.
   label - button text. Can be enclosed in ". Supports escape sequences. Use "" for no text.
   style - button style.
   state - button state. It will be xored with BTNS_ENABLED (4). Use 4 to disable.
 imagelist - can be an __ImageList variable. Create imagelists with the imagelist editor (menu Tools -> Imagelist Editor).
 wndstyle - toolbar styles to be added. Or you can add styles when creating the toolbar.
 btnstyle - add this style to all buttons.
 btnstate - add this state to all buttons.
 ttTime - tooltip time, s.
 tbExStyle - if not 0, sends TB_SETEXTENDEDSTYLE with this value.

 Toolbar and button styles and states are documented in the MSDN Library.


if(wndstyle) SetWinStyle htb wndstyle 1
SendMessage htb TB_SETIMAGELIST 0 imagelist
if(tbExStyle) SendMessage htb TB_SETEXTENDEDSTYLE 0 tbExStyle

ARRAY(TBBUTTON) a
str s strings; int istring
foreach s buttons
	lpstr bid img(0) label(0) style(0) state(0) data(0)
	if(tok(s &bid 6 " ''" 1|4)<1) continue
	TBBUTTON& t=a[]
	t.idCommand=val(bid)
	t.iBitmap=val(img)
	t.fsStyle=iif(t.idCommand val(style) BTNS_SEP)
	t.fsState=val(state)
	if(t.fsStyle&BTNS_SEP=0) t.fsStyle|btnstyle; t.fsState|btnstate; t.fsState^BTNS_ENABLED
	t.dwData=val(data)
	if(empty(label)) t.iString=-1
	else
		t.iString=istring; istring+1
		_s=label; _s.escape
		strings.fromn(strings strings.len _s _s.len+1)

SendMessage(htb TB_BUTTONSTRUCTSIZE sizeof(TBBUTTON) 0)
if(istring) SendMessage(htb TB_ADDSTRINGW 0 @strings)
if(a.len) SendMessage(htb TB_ADDBUTTONSW a.len &a[0])
SendMessage(htb TB_AUTOSIZE 0 0)

if ttTime
	int htt=SendMessage(htb TB_GETTOOLTIPS 0 0)
	SendMessage htt TTM_SETDELAYTIME TTDT_AUTOPOP ttTime*1000
