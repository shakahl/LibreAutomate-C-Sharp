 /DEX_Main3
 \Dialog_Editor
function# hDlg message wParam lParam

 BEGIN DIALOG
 0 "" 0x90CF0A44 0x100 0 0 222 142 "Form"
 5 SysListView32 0x54010000 0x200 84 18 68 64 ""
 4 SysTreeView32 0x54010000 0x200 4 18 68 64 ""
 3 ToolbarWindow32 0x54010000 0x0 0 0 222 17 ""
 6 msctls_statusbar32 0x54000100 0x0 0 128 222 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020002 "DEX_Main3" ""

 messages
sel message
	case WM_INITDIALOG
	DEX_InitToolbar3 hDlg id(3 hDlg)
	DEX_InitStatusBar3 hDlg id(6 hDlg)
	DEX_InitTreeView3 hDlg id(4 hDlg)
	DEX_InitListView3 hDlg id(5 hDlg) 0
	DEX_AutoSize2 hDlg
	
	case WM_DESTROY

	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	case WM_SIZE DEX_AutoSize2 hDlg
ret
 messages2
sel wParam
	case IDOK
	case IDCANCEL
	case [1001,1002,1003] DEX_SB2 hDlg 0 wParam ;;toolbar button clicked
	 case x RedrawWindow(id(4 hDlg) 0 0 RDW_INVALIDATE) ;;this can be used to redraw a control
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 3 ;;toolbar
	sel nh.code
		case NM_RCLICK
		NMMOUSE* mo=+nh
		if(mo.dwItemSpec=-1) ret ;;not on button
		out "right click on button %i" mo.dwItemSpec
		
		case TBN_GETINFOTIP
		NMTBGETINFOTIP* tt=+lParam
		str-- stt
		stt.format("tooltip text of button %i" tt.iItem)
		tt.pszText=stt
		
		 case NM_CUSTOMDRAW ret DT_Ret(hDlg DEX_TbCustomDraw2(+nh))
		
	case 4 ;;tree
	sel nh.code
		case TVN_SELCHANGED ;;treeview item selected
		NMTREEVIEW* ntv=+nh
		int i=ntv.itemNew.lParam ;;was set by DEX_TvAdd2
		if(i) DEX_SB2 hDlg 1 i ;;else folder
		
		case NM_CUSTOMDRAW ret DT_Ret(hDlg DEX_TvCustomDraw2(+nh))
	
	case 5 ;;list
	sel nh.code
		case LVN_ITEMCHANGED
		NMITEMACTIVATE* na=+nh
		if(na.uNewState&LVIS_SELECTED and na.uOldState&LVIS_SELECTED=0) ;;listview item selected
			SetFocus nh.hwndFrom
			DEX_SB2 hDlg 2 na.lParam ;;was set by DEX_LvAdd2
		
		case NM_CUSTOMDRAW ret DT_Ret(hDlg DEX_LvCustomDraw2(+nh))
		
		
	
