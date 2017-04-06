\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

out
str controls = "4"
str e4
if(!ShowDialog("Dialog103" &Dialog103 &controls)) ret

 BEGIN DIALOG
 0 "" 0x90CC0AC8 0x0 0 0 205 129 "Dialog"
 3 Button 0x54032000 0x0 0 115 48 14 "Button"
 4 Edit 0x54030080 0x200 216 72 96 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030400 "*" "" ""

ret
 messages
__GdiHandle hrgn
sel message
	case WM_INITDIALOG
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_ERASEBKGND
	out "bk %i" wParam
	case WM_NCPAINT
	out "nc"
	
	case WM_PAINT
	out "paint begin"
	
	 out GetUpdateRect(hDlg 0 0)
	
	PAINTSTRUCT ps
	int hdc=BeginPaint(hDlg &ps)
	
	  RECT r; out GetClipBox(hdc &r); zRECT r
	 
	 hrgn=CreateRectRgn(0 0 0 0)
	 
	  out GetRandomRgn(hdc hrgn 4)
	 
	  SelectClipRgn(hdc hrgn)
	 
	  __Hdc m.CreateMemoryDC()
	 __Hdc m.CreateMemoryDC(hdc)
	 
	  SelectClipRgn(m.dc hrgn)
	 
	 out GetClipRgn(hdc hrgn)
	 out GetClipRgn(m.dc hrgn)
	
	EndPaint hDlg &ps
	out "paint end"
ret
 messages2
sel wParam
	case 3
	out
	 act _hwndqm
	 InvalidateRect hDlg 0 1
	
	 __Hdc dc.FromWindowDC(hDlg)
	__Hdc dc.FromWindowDC(id(4 hDlg))
	hrgn=CreateRectRgn(0 0 10 10)
	 out GetUpdateRgn(hDlg hrgn 0)
	 out GetClipRgn(dc.dc hrgn)
	 out GetRandomRgn(dc.dc hrgn 1)
	out GetRandomRgn(dc.dc hrgn 4)
	RECT rb; out GetRgnBox(hrgn &rb); zRECT rb
	
ret 1
