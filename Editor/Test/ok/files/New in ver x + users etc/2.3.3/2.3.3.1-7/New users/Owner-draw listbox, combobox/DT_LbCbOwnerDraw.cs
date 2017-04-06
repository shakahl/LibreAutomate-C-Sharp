 /
function! hDlg message wParam lParam [ctlId] [flags] [ARRAY(int)&aColorText] [ARRAY(int)&aColorBack] [selColorText] [selColorBack] [ARRAY(int)&aFont] ;;flags: 1 wrap lines, 2 draw separators

 Draws items in owner-draw listbox or combobox control.
 Items can have multiple lines.
 Call this function from a dialog procedure, like in dlg_listbox_multiline.
 Returns 1 if draws, 0 if not.

 hDlg message wParam lParam - hDlg message wParam lParam.
 ctrlId - control id. If 0, draws all owner-draw listbox and combobox controls.
 aColorText, aColorBack - arrays containing colors of item text and background. The variables should have thread scope. If omitted or 0, uses default colors.
 selColorText, selColorBack - colors of selected item text and background. If omitted or 0, uses default selection colors.
 aFont - array containing font handles of item text. The variable should have thread scope. Can be set only some elements, others can be 0 (default font).


sel(message) case [WM_MEASUREITEM,WM_DRAWITEM] case else ret

int ct cid msg1 msg2

sel message
	case WM_MEASUREITEM
	MEASUREITEMSTRUCT& mi=+lParam
	ct=mi.CtlType
	cid=mi.CtlID
	
	case WM_DRAWITEM
	DRAWITEMSTRUCT& di=+lParam
	ct=di.CtlType
	cid=di.CtlID

if(ctlId and cid!ctlId) ret
sel ct
	case ODT_LISTBOX msg1=LB_GETTEXT; msg2=LB_GETTEXTLEN
	case ODT_COMBOBOX msg1=CB_GETLBTEXT; msg2=CB_GETLBTEXTLEN
	case else ret

int hwnd i hdc fl
RECT rt
hwnd=id(cid hDlg)

sel message
	case WM_MEASUREITEM
	i=mi.itemID
	hdc=GetDC(hwnd)
	RECT rc; GetClientRect hwnd &rc; rt.right=rc.right
	fl=DT_CALCRECT
	
	case WM_DRAWITEM
	i=di.itemID
	if(i<0) ret
	hdc=di.hDC
	rt=di.rcItem
	
	int colB colT colB_used colT_used
	if(di.itemState&ODS_SELECTED)
		colB=COLOR_HIGHLIGHT; colT=COLOR_HIGHLIGHTTEXT
		if(selColorBack) colB=selColorBack; colB_used=1
		if(selColorText) colT=selColorText; colT_used=1
	else
		colB=COLOR_WINDOW; colT=COLOR_WINDOWTEXT
		if(&aColorBack && i<aColorBack.len) colB=aColorBack[i]; colB_used=1
		if(&aColorText && i<aColorText.len) colT=aColorText[i]; colT_used=1
	if(!colB_used) colB=GetSysColorBrush(colB); else colB=CreateSolidBrush(colB)
	if(!colT_used) colT=GetSysColor(colT)
	FillRect hdc &rt colB; if(colB_used) DeleteObject colB
	SetTextColor hdc colT
	SetBkMode hdc TRANSPARENT

int tl=SendMessageW(hwnd msg2 i 0)
if tl>0
	BSTR s.alloc(tl)
	tl=SendMessageW(hwnd msg1 i s.pstr)
	if tl>0
		if(&di and di.itemState&ODS_COMBOBOXEDIT) fl|DT_SINGLELINE
		else if(flags&1) fl|DT_WORDBREAK
		
		int font
		if(&aFont && i<aFont.len) font=aFont[i]
		if(!font) font=SendMessageW(hwnd WM_GETFONT 0 0)
		int oldfont=SelectObject(hdc font)
		DrawTextW(hdc s tl &rt fl|DT_EXPANDTABS|DT_NOPREFIX)
		SelectObject hdc oldfont

sel message
	case WM_MEASUREITEM
	ReleaseDC hwnd hdc
	mi.itemHeight=rt.bottom
	
	case WM_DRAWITEM
	if(di.itemState&ODS_FOCUS) DrawFocusRect hdc &rt
	else if flags&2
		int pen=CreatePen(PS_SOLID 1 0xc0c0c0)
		int oldpen=SelectObject(hdc pen)
		MoveToEx hdc 0 rt.bottom-1 0; LineTo hdc rt.right rt.bottom-1
		DeleteObject SelectObject(hdc oldpen)

ret DT_Ret(hDlg 1)
