 /Dialog95
function# hDlg message wParam lParam ctrlId callbackFunc [param]

 Draws combo or list box items with custom background and text colors.

 This is how to create a combo or list box and set colors:
 1. The dialog must be a smart dialog, ie with dialog procedure.
 2. In the dialog editor, add a combo or list box.
 3. In the Styles dialog:
    For combo box, select CBS_OWNERDRAWVARIABLE and CBS_HASSTRINGS styles.
    For list box, select LBS_OWNERDRAWVARIABLE and LBS_HASSTRINGS styles.
 4. Create callback function similar to SampleCbItemColorProc. The function sets colors.
 5. In the dialog procedure, call CB_ItemColor before 'sel messages' line. Pass address of the callback function. Call it for each such control.

 hDlg, message, wParam, lParam - hDlg, message, wParam, lParam.
 ctrlId - combo/list box id.
 callbackFunc - address of a callback function. See SampleCbItemColorProc.
 param - a user-defined value that will be passed to the callback function.


type CBITEMCOLOR str'text bkColor textColor hwnd item itemData !selected !isLB dtFlags param

sel message
	case WM_MEASUREITEM
	if(wParam=ctrlId)
		MEASUREITEMSTRUCT& mi=+lParam
		int hlb=id(ctrlId hDlg 1)
		int ih=GetProp(hlb "lbItemHeight")
		if !ih
			int dc=GetDC(hlb); int of=SelectObject(dc SendMessage(hlb WM_GETFONT 0 0))
			RECT _r; DrawText dc "J" 1 &_r DT_CALCRECT; ih=_r.bottom
			SetProp hlb "lbItemHeight" ih
			SelectObject dc of; ReleaseDC hlb dc
		mi.itemHeight=ih+4
	
	case WM_APP+140
	RemoveProp id(ctrlId hDlg 1) "lbItemHeight"
	
	case WM_DRAWITEM
	if(wParam=ctrlId)
		DRAWITEMSTRUCT* ds=+lParam
		if(ds.itemID<0) ret
		RECT r=ds.rcItem
		
		CBITEMCOLOR c
		c.hwnd=ds.hWndItem
		c.item=ds.itemID
		c.itemData=ds.itemData
		c.isLB=ds.CtlType=ODT_LISTBOX
		c.param=param
		c.dtFlags=DT_NOPREFIX
		c.selected=ds.itemState&(ODS_SELECTED|ODS_COMBOBOXEDIT)=ODS_SELECTED
		c.bkColor=GetSysColor(iif(c.selected COLOR_HIGHLIGHT COLOR_WINDOW))
		c.textColor=GetSysColor(iif(c.selected COLOR_HIGHLIGHTTEXT COLOR_WINDOWTEXT))
		GetItemText c.hwnd c.item c.text c.isLB
		
		call callbackFunc &c
		
		SetBkMode ds.hDC TRANSPARENT
		__GdiHandle brush=CreateSolidBrush(c.bkColor)
		FillRect ds.hDC &r brush
		SetTextColor ds.hDC c.textColor
		
		r.left+4; r.top+2
		DrawTextW ds.hDC @c.text -1 &r c.dtFlags
