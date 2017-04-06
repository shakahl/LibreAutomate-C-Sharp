 /DEX_Main2
 \Dialog_Editor
function# hDlg message wParam lParam

 BEGIN DIALOG
 0 "" 0x90CF0A44 0x100 0 0 222 142 "Form"
 5 SysListView32 0x54010000 0x200 84 18 68 64 ""
 4 SysTreeView32 0x54010000 0x200 4 18 68 64 ""
 3 ToolbarWindow32 0x54010000 0x0 0 0 222 17 ""
 6 msctls_statusbar32 0x54000900 0x0 0 128 222 14 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2020002 "DEX_Main" ""

 messages
sel message
	case WM_INITDIALOG
	DEX_InitToolbar2 hDlg id(3 hDlg)
	DEX_InitStatusBar2 hDlg id(6 hDlg)
	DEX_InitTreeView2 hDlg id(4 hDlg)
	 SendMessage(id(4 hDlg) TBM_SETUNICODEFORMAT 0 0)
	DEX_InitListView2 hDlg id(5 hDlg) 0
	DEX_AutoSize hDlg
	
	case WM_DESTROY

	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
	case WM_SIZE DEX_AutoSize hDlg
ret
 messages2
sel wParam
	case [1001,1002,1003] DEX_SB hDlg 0 wParam ;;toolbar button clicked
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 4 ;;tree
	sel nh.code
		case TVN_SELCHANGED ;;treeview item selected
		NMTREEVIEW* ntv=+nh
		int i=ntv.itemNew.lParam ;;was set by TvAdd
		if(i) DEX_SB hDlg 1 i ;;else folder
		
		 case NM_CUSTOMDRAW ret DT_Ret(hDlg DEX_TvCustomDraw(+nh))
	
	case 5 ;;list
	sel nh.code
		case LVN_ITEMCHANGED
		NMITEMACTIVATE* na=+nh
		if(na.uNewState&LVIS_SELECTED and na.uOldState&LVIS_SELECTED=0) ;;listview item selected
			SetFocus nh.hwndFrom
			DEX_SB hDlg 2 na.lParam ;;was set by DEX_LvAdd
		
		 case NM_CUSTOMDRAW ret DT_Ret(hDlg DEX_LvCustomDraw(+nh))
		
		
	
