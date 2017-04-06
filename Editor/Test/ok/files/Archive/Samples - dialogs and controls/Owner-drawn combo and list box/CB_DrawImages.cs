 /
function# hDlg message wParam lParam ctrlId $icons [flags] [imgWidth] [imgHeight] [dtFlags] ;;flags: 1 listbox (default combobox), 2 item data is image index, 4 icons is imagelist, 16 not icons (bmp, jpg, gif)

 Adds icons or bmp/jpg/gif images to a combo box or list box.

 This is what you have to do to create a combo or list box with icons or images:
 1. The dialog must be a smart dialog, ie with dialog procedure.
 2. In the dialog editor, add a combo or list box.
 3. In the Styles dialog:
    For combo box, select CBS_OWNERDRAWFIXED and CBS_HASSTRINGS styles. Make sure that CBS_SORT is not selected (sorted combo boxes are not supported).
    For list box, select LBS_OWNERDRAWFIXED and LBS_HASSTRINGS styles. Make sure that LBS_SORT is not selected (sorted list boxes are not supported).
 4. In the dialog procedure, insert CB_DrawImages before 'sel messages' line.

 hDlg, message, wParam, lParam - hDlg, message, wParam, lParam.
 ctrlId - combo/list box id.
 icons - list of icon files. If flag 16 used, list of image files of type bmp, jpg or gif. If flag 4 used, imagelist file.
 flags:
    1 the control is a listbox.
    2 item image index is stored in item data (default - same as item index).
    4 icons is .bmp file created with QM imagelist editor.
    16 use bitmap (default - use icon). Don't use with flag 4.
 imgWidth, imgHeight - width and height of images. Default: 16 pixels.
 dtFlags - DrawText flags. Documented in the MSDN Library on the Internet. Xored with DT_NOPREFIX.


int i n il hi hb hcb; str s
hcb=id(ctrlId hDlg)
if(!imgWidth) imgWidth=16
if(!imgHeight) imgHeight=16

sel message
	case WM_CREATE
	SendMessage hcb iif(flags&1 LB_SETITEMHEIGHT CB_SETITEMHEIGHT) 0 imgHeight+2
	
	case WM_INITDIALOG
	if(flags&4)
		il=__ImageListLoad(icons)
	else
		n=numlines(icons)
		il=ImageList_Create(imgWidth imgHeight ILC_MASK|ILC_COLOR32 0 n)
		for(i 0 n)
			s.getl(icons -i)
			if(flags&16)
				hb=LoadPictureFile(s 0); if(!hb) goto g1
				ImageList_Add(il hb 0)
				if(hb) DeleteObject hb
			else
				hi=GetFileIcon(s 0 (imgWidth>=24 or imgHeight>=24))
				 g1
				ImageList_ReplaceIcon(il -1 iif(hi hi _dialogicon))
				if(hi) DestroyIcon hi
	SetProp hcb "qm_il" il
	
	case WM_DESTROY
	ImageList_Destroy RemoveProp(hcb "qm_il")
	
	case WM_DRAWITEM
	if(wParam=ctrlId)
		DRAWITEMSTRUCT* ds=+lParam
		RECT r=ds.rcItem
		 background
		int selected=ds.itemState&(ODS_SELECTED|ODS_COMBOBOXEDIT)=ODS_SELECTED
		FillRect ds.hDC &ds.rcItem iif(selected COLOR_HIGHLIGHT COLOR_WINDOW)+1
		 icon
		il=GetProp(hcb "qm_il")
		int ii=iif(flags&2 ds.itemData ds.itemID)
		n=ImageList_GetImageCount(il); if(ii>=n) ii=n-1
		ImageList_Draw il ii ds.hDC r.left+1 r.top+1 0
		 text
		SetBkMode ds.hDC TRANSPARENT
		SetTextColor ds.hDC GetSysColor(iif(selected COLOR_HIGHLIGHTTEXT COLOR_WINDOWTEXT))
		GetItemText ds.hWndItem ds.itemID s flags&1
		r.left+imgWidth+4; r.top+2
		DrawTextW ds.hDC @s -1 &r dtFlags^DT_NOPREFIX
