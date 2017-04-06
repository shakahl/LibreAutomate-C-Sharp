 \
function $text timeS x y [maxTipWidth] [flags] [$title] [$titleIcon] ;;flags: 1 balloon (XP+), 2 asynchronous (don't wait).  titleIcon: "info" "warning" "error"

 Shows tooltip that is not attached to a control.

 text - text.
 timeS - time to show, s.
 x, y - position in screen.
 maxTipWidth - max width. If nonzero, text can be multiline.
 flags:
   1 - balloon. Unavailable on Windows 2000.
   2 - don't wait until disappears. The function creates other thread to show the tooltip. 
 title - title text.
 titleIcon - one of standard icons (see above). On Windows XP SP2 and later can be icon file.

 EXAMPLES
 ShowTooltip "tooltip" 2 100 100

 str s="Asynchronous balloon tooltip[]with title and icon."
 ShowTooltip s 10 xm ym 300 3 "title" "$qm$\info.ico"


if flags&2
	mac "ShowTooltip" "" text timeS x y maxTipWidth flags~2 title titleIcon
	ret

int st=TTS_NOPREFIX|TTS_ALWAYSTIP|TTS_CLOSE
if(flags&1) st|TTS_BALLOON
int hwndTT = CreateWindowEx(WS_EX_TOPMOST TOOLTIPS_CLASS 0 st 0 0 0 0 0 0 0 0)

if(maxTipWidth) SendMessage(hwndTT, TTM_SETMAXTIPWIDTH, 0, maxTipWidth)

if !empty(title)
	int ic; if(!empty(titleIcon)) ic=SelStr(0 titleIcon "info" "warning" "error"); if(!ic) __Hicon _ic=GetFileIcon(titleIcon); ic=_ic
	SendMessage(hwndTT TTM_SETTITLEW ic @title)

TOOLINFOW ti.cbSize=44
ti.uFlags = TTF_TRACK
ti.lpszText=@text
SendMessage(hwndTT, TTM_ADDTOOLW, 0, &ti)

SendMessage(hwndTT, TTM_TRACKPOSITION, 0, MakeInt(x, y))
SendMessage(hwndTT, TTM_TRACKACTIVATE, 1, &ti)

opt waitmsg 1
wait timeS -WV hwndTT; err
DestroyWindow hwndTT
