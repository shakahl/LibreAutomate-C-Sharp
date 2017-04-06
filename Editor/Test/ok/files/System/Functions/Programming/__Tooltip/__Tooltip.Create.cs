function hwnd [flags] [timeToShow] ;;flags: 1 show when window inactive, 2 balloon, 4 don't subclass

 Creates tooltip control and adds tooltips.

 hwnd - handle of parent window of controls. It will be owner window of the tooltip control.
 timeToShow - number of seconds to show tooltip. Max 32.
 flags:
   1 - show tooltip regardless whether the parent window is active.
   2 - show balloon tooltip.
   4 - don't subclass controls. Parent window/dialog procedure must relay mouse messages to the tooltip control, for example use <help>__Tooltip.OnWmSetcursor</help>. Else tooltips are not displayed. It also fixes Windows XP bug: no tooltip after a click.

 REMARKS
 To specify controls (child windows of hwnd for which you want to add tooltips), call AddControl (for each control) or AddControls, before or after calling this function.
 If tooltip control already created, this function at first destroys it.
 The variable must exist while the parent window exists. For example it can be a thread variable (__Tooltip-).
 Usually you call Create on WM_CREATE or WM_INITDIALOG, and Destroy on WM_DESTROY.
 You don't have to use this class to add tooltips to a dialog. You can specify tooltips in Dialog Editor or in dialog template.
   ShowDialog uses an internal __Tooltip variable for dialog tooltips. You can call DT_GetTooltip from dialog procedure to modify dialog tooltips.


if(htt) Destroy

int tts=TTS_NOPREFIX
if(flags&1) tts|TTS_ALWAYSTIP
if(flags&2) tts|TTS_BALLOON
htt=CreateWindowExW(WS_EX_TRANSPARENT L"tooltips_class32" 0 tts 0 0 0 0 hwnd 0 _hinst 0)
SendMessage htt TTM_ACTIVATE 1 0
SendMessage htt TTM_SETMAXTIPWIDTH 0 600
if(timeToShow) SendMessage htt TTM_SETDELAYTIME TTDT_AUTOPOP iif(timeToShow>32 32 timeToShow)*1000

__hwnd=hwnd
m_flags=flags

for(_i 0 __a.len) AddControl(__a[_i].i __a[_i].s)
__a=0

 ;;note: without ex transparent sometimes blinks
