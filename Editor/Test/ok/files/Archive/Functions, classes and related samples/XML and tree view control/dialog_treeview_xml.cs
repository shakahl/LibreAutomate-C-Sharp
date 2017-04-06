\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

 create xml file if does not exist
iff- "$temp$/qm xml tv.xml"
	_s=
 <x>
 <x t="a1"/>
 <x t="a2">
 	<x t="b1"/>
 	<x t="b2">
 		<x t="c1"/>
 		<x t="c2"/>
 	</x>
 	<x t="b3"/>
 </x>
 <x t="a3"/>
 </x>
	_s.setfile("$temp$/qm xml tv.xml")

str controls = "5"
str e5
if(!ShowDialog("dialog_treeview_xml" &dialog_treeview_xml &controls)) ret

 BEGIN DIALOG
 0 "" 0x90C80AC8 0x0 0 0 223 135 "Dialog"
 3 SysTreeView32 0x54010027 0x0 0 0 96 116 ""
 5 Edit 0x54030080 0x200 102 6 118 14 ""
 4 Button 0x54032000 0x0 102 24 118 14 "Add child of selected"
 6 Button 0x54032000 0x0 154 44 66 14 "Delete selected"
 1 Button 0x54030001 0x4 120 116 48 14 "OK"
 2 Button 0x54030000 0x4 170 116 48 14 "Cancel"
 END DIALOG
 DIALOG EDITOR: "" 0x2030300 "" "" ""

ret
 messages
sel message
	case WM_INITDIALOG
	 load xml file and show in tree view control
	_s.getfile("$temp$/qm xml tv.xml"); err
	if(_s.len) XmlToTreeView _s id(3 hDlg)
	
	 StringFromTreeView _s id(3 hDlg)
	 out; out _s; clo hDlg
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
ret
 messages2
sel wParam
	case IDOK
	 save tree view control data to xml file
	XmlFromTreeView _s id(3 hDlg)
	_s.setfile("$temp$/qm xml tv.xml")
	 out _s
	
	case 4 ;;add child item of selected item
	_s.getwintext(id(5 hDlg))
	TvAdd id(3 hDlg) SendMessage(id(3 hDlg) TVM_GETNEXTITEM TVGN_CARET 0) _s
	InvalidateRect id(3 hDlg) 0 1 ;;redraw
	
	case 6 ;;delete selected item
	_i=SendMessage(id(3 hDlg) TVM_GETNEXTITEM TVGN_CARET 0)
	if(_i) SendMessage(id(3 hDlg) TVM_DELETEITEM 0 _i) ;;else deletes all
	
	case IDCANCEL
ret 1
