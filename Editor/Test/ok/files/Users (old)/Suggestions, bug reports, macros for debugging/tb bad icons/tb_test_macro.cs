function [qmthread]

 A QM toolbar has a child window of class ToolbarWindow32.
 It is standard toolbar control, used in many applications.
 This macro runs toolbar tb_test, deletes the original
 child window, creates new child window, and adds one button.
 To do this, it uses the same methods as it does QM when
 creating the original child window.

 At first, change favorites.ico to some icon that is incorrectly
 displayed. Run this macro. If the icon is displayed incorrectly,
 try to change the following values:

 Change ILC_COLOR16 to ILC_COLORDDB, ILC_COLOR32, ILC_COLOR24, ILC_COLOR8 or ILC_COLOR;
 Change cx and cy to 30.
 Delete SWP_NOCOPYBITS;
 Delete WS_VISIBLE, and add SWP_SHOWWINDOW later;
 Delete DestroyIcon(ic);
 Delete tbb._delete;
 And experiment with different icons (icons created in QM, other icons).


str iconfile="favorites.ico" ;;change this
 str iconfile="$qm$" ;;also try this
 ________________________________

str toolbarname="TB_TEST"
if(!qmthread)
	ClearOutput
	 if the toolbar is open, close it
	int h=win(toolbarname "QM_Toolbar"); if(h) clo h
	0.1
	 run it again
	mac toolbarname
	0.5
	 start this macro in qm main thread
	SendMessage _hwndqm WM_SETTEXT 3 "M ''tb_test_macro'' A 1"
	ret

 find the toolbar
h=win(toolbarname "QM_Toolbar")
 close the child window
clo id(9999 h)
 hide (to match original conditions)
hid h
 ________________________________

 The following code is a simplified version of the code
 that QM uses to create toolbar child window and add buttons.

#compile tb_test_def

 create new child window
int st=WS_VISIBLE|WS_CHILD|WS_CLIPSIBLINGS|CCS_ADJUSTABLE|CCS_NODIVIDER|CCS_NORESIZE|TBSTYLE_TOOLTIPS|TBSTYLE_FLAT|TBSTYLE_WRAPABLE
int cx=0
int cy=0
int htb=CreateWindowEx(WS_EX_TOOLWINDOW, "ToolbarWindow32", 0, st, 0, 0, cx, cy, h, 9999, _hinst, 0)

 create imagelist
int il=ImageList_Create(16, 16, ILC_COLOR16|ILC_MASK, 0, 1)
ImageList_SetBkColor(il, GetSysColor(COLOR_BTNFACE))

 attach the imagelist to the child window
SendMessage(htb, TB_SETIMAGELIST, 0, il)

 get icon handle
int ic=GetIcon(iconfile)

 fill TBBUTTON structure
TBBUTTON* tbb._new(1)
tbb[0].iString=-1
tbb[0].fsState=TBSTATE_ENABLED
tbb[0].iBitmap=ImageList_ReplaceIcon(il, -1, ic) ;;add the icon to the imagelist
DestroyIcon(ic)

 add button
SendMessage(htb, TB_BUTTONSTRUCTSIZE, sizeof(TBBUTTON), 0)
SendMessage(htb, TB_ADDBUTTONS, 1, tbb)
tbb._delete

 resize the child window
SetWindowPos(htb, 0, 0, 0, 30 30, SWP_NOCOPYBITS|SWP_NOZORDER|SWP_NOOWNERZORDER) ;;add SWP_SCOWWINDOW here
 __________________________________

 unhide
SetWindowPos(h, 0, 0, 0, 0, 0, SWP_SHOWWINDOW|SWP_NOSIZE|SWP_NOMOVE|SWP_NOACTIVATE|SWP_NOZORDER|SWP_NOOWNERZORDER)
