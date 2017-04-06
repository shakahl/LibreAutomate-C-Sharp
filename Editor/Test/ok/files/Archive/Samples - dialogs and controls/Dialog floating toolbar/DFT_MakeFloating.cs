 /DFT_Main
function hDlg floating ;;floating: 1 floating, 0 docked.  hDlg must be parent of toolbar control

int htb=id(3 hDlg)

if floating
	 create floating parent window of the toolbar control and move the control there
	int hf=ShowDialog("DFT_FloatingDlg" &DFT_FloatingDlg 0 hDlg 1)
	SetParent htb hf
	
	 resize the new parent window to show whole toolbar control
	RECT r
	SendMessage htb TB_GETITEMRECT SendMessage(htb TB_BUTTONCOUNT 0 0)-1 &r ;;get rect of last button
	r.left=0; r.top=0
	AdjustWindowRectEx &r GetWinStyle(hf) 0 GetWinStyle(hf 1) ;;client -> window
	siz r.right-r.left r.bottom-r.top hf
else
	 move the control to the main window and destroy the floating window
	hf=hDlg
	hDlg=GetWindow(hDlg GW_OWNER)
	SetParent htb hDlg
	DestroyWindow hf

 move/resize controls that are below the toolbar
DFT_ResizeControls hDlg
