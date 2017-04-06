out

 Chrome, http://www.quickmacros.com/help/Functions/IDP_ACC.html, searching for text "acc".

int w=win("Google Chrome" "Chrome_WidgetWin_1")
act w

 at first we need the rectangle, in screen coordinates
RECT r
int capture=1 ;;change this: set to 0 or 1
if capture ;;capture rectangle at run time
	int w2 ;;may be a child window, not w
	if(2!CaptureWindowAndRect(w2 r)) ret
	MapWindowPoints w2 0 +&r 2 ;;client to screen coord
else ;;specify rectangle in macro
	SetRect &r 10 500 600 600
	MapWindowPoints w 0 +&r 2 ;;client to screen coord

 show rectangle; remove this later
OnScreenRect 0 r; 1; OnScreenRect 2

 search for an object; use callback function
Acc a.Find(w "STATICTEXT" "acc" "" 0x3011 0 0 0 &sub.Callback_Acc_Find &r)

 results
a.Mouse


#sub Callback_Acc_Find
function# Acc&a level RECT&r

 Callback function for Acc.Find or acc.
 Read more about <help #IDP_ENUMWIN>callback functions</help>.

 a - the found object.
 cbParam - cbParam passed to Acc.Find.

 Return:
 0 - stop. Let a will be the found object.
 1 - continue.
 2 - continue, but skip children of a.


 out a.Name
int x y
a.Location(x y)
if(PtInRect(&r x y)) ret

ret 1
