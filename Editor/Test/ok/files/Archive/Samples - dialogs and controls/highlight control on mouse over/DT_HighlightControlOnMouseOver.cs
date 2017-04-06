 /
function# hDlg message wParam lParam hctrl backcolor textcolor

 Changes control colors when mouse pointer is over.
 The control must be Static or read-only Edit.
 Call this function from dialog procedure, between ' messages' and 'sel message' lines.
 If this function returns a nonzero value, let the dialog procedure return that value.
 Call this function for each control you want to change colors.

 hDlg message wParam lParam - hDlg message wParam lParam.
 hctrl - control handle.
 backcolor - control color. Use -1 for default color.
 textcolor - control text color. Use -1 for default color.

 See also: <ColorFromRGB>

 EXAMPLE
  ...
  messages
 int z=DT_HighlightControlOnMouseOver(hDlg message wParam lParam id(3 hDlg) 0xff0000 0x00ffff)
 if(z) ret z
 sel message
 	 ...

int brush=GetProp(hDlg "qm_hcoobrush")
sel message
	case WM_SETCURSOR
	if(!brush and child(mouse)=hctrl)
		 out "1 %i" hctrl
		if(backcolor=-1) backcolor=GetSysColor(COLOR_BTNFACE)
		SetProp(hDlg "qm_hcoobrush" CreateSolidBrush(backcolor))
		SetProp(hDlg "qm_hcoohwnd" hctrl)
		RedrawWindow hctrl 0 0 RDW_INVALIDATE
		SetTimer hDlg 33345 50 0
	case WM_TIMER
	if(wParam=33345 and hctrl=GetProp(hDlg "qm_hcoohwnd") and child(mouse)!=hctrl)
		 out "2 %i" hctrl
		KillTimer hDlg wParam
		DeleteObject RemoveProp(hDlg "qm_hcoobrush")
		RemoveProp(hDlg "qm_hcoohwnd")
		RedrawWindow hctrl 0 0 RDW_INVALIDATE
	case WM_CTLCOLORSTATIC
	if(brush and lParam=hctrl)
		SetBkMode wParam TRANSPARENT
		if(textcolor!=-1) SetTextColor wParam textcolor
		ret brush
	case WM_DESTROY
	if(RemoveProp(hDlg "qm_hcoohwnd")) DeleteObject RemoveProp(hDlg "qm_hcoobrush")
