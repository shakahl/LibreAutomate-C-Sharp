/exe
 \Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("IL_Dialog" &IL_Dialog 0)) ret

 BEGIN DIALOG
 0 "" 0x90CF0A44 0x100 0 0 222 142 "Form"
 5 SysListView32 0x54010000 0x200 76 24 146 118 ""
 4 SysTreeView32 0x54010000 0x200 0 24 74 118 ""
 3 ToolbarWindow32 0x54010000 0x0 0 0 222 17 ""
 END DIALOG
 DIALOG EDITOR: "" 0x2030003 "*" "" ""

 messages
sel message
	case WM_INITDIALOG
	 load imagelist. We'll use single imagelist for 3 controls.
	__ImageList- t_imagelist.Load(":5941 $qm$\il_qm.bmp")

	int h
	
	 init toolbar
	lpstr btns=
	 1001 0 "One"
	 1002 1 "Two"
	 1003 2 "Three"
	TO_TbInit id(3 hDlg) btns t_imagelist TBSTYLE_FLAT|TBSTYLE_TOOLTIPS
	
	 init treeview
	h=id(4 hDlg)
	SetWinStyle h TVS_HASBUTTONS|TVS_HASLINES|TVS_LINESATROOT|TVS_SHOWSELALWAYS 1
	SendMessage h TVM_SETIMAGELIST 0 t_imagelist
	int hifolder hifirst
	hifirst=TvAdd(h 0 "One" 2001 3)
	hifolder=TvAdd(h 0 "Folder" 0 4)
	TvAdd h hifolder "Two" 2002 5
	TvAdd h 0 "Three" 2003 6
	SendMessage h TVM_SELECTITEM TVGN_CARET hifirst
	
	 init listview
	h=id(5 hDlg)
	SetWinStyle h LVS_REPORT|LVS_SHAREIMAGELISTS|LVS_SINGLESEL 1 ;;note: LVS_SHAREIMAGELISTS is important
	SendMessage h LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_INFOTIP|LVS_EX_FULLROWSELECT|LVS_EX_ONECLICKACTIVATE -1
	SendMessage h LVM_SETIMAGELIST LVSIL_SMALL t_imagelist
	TO_LvAddCol h 0 "Item" 70
	TO_LvAddCol h 1 "Subitem 1" 70
	TO_LvAddCol h 2 "Subitem 2" 70
	TO_LvAdd h 0 0 7 "One" "01" "02"
	TO_LvAdd h 1 0 8 "Two" "11" "12"
	TO_LvAdd h 2 0 9 "Three" "21" "22"
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case [1001,1002,1003] ;;toolbar button clicked
	out wParam
ret 1
 messages3
NMHDR* nh=+lParam
sel nh.idFrom
	case 4 ;;tree
	sel nh.code
		case TVN_SELCHANGED ;;treeview item selected
		NMTREEVIEW* ntv=+nh
		int k=ntv.itemNew.lParam
		if(k) out k ;;else folder
	
	case 5 ;;list
	sel nh.code
		case NM_CLICK
		NMITEMACTIVATE* na=+nh
		out "%i %i" na.iItem na.iSubItem

 BEGIN PROJECT
 main_function  IL_Dialog
 exe_file  $my qm$\IL_Dialog.exe
 icon  
 manifest  $qm$\default.exe.manifest
 res  
 on_before  
 on_after  
 on_run  
 flags  22
 end_hotkey  0
 guid  {15AE1131-235F-4AA0-B95D-D718EB46EF94}
 END PROJECT
