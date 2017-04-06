\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

if(!ShowDialog("" &dlg_QM_Grid_images_easy 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 399 175 "QM_Grid"
 3 QM_Grid 0x56031041 0x0 0 0 400 152 ""
 1 Button 0x54030001 0x4 98 158 48 14 "OK"
 2 Button 0x54030000 0x4 148 158 48 14 "Cancel"
 4 Button 0x54032000 0x0 236 158 48 14 "Button"
 END DIALOG
 DIALOG EDITOR: "" 0x2030202 "" "" ""

ret
 messages
int i
DlgGrid g.Init(hDlg 3)
sel message
	case WM_INITDIALOG
	lpstr columns="0x27[]noedit,150[]edit,50[]edit+button,80,16[]edit multiline,80,8[]combo,80,1[]check,50,2[]read-only,80,7"
	g.ColumnsAdd(columns 2)
	lpstr si=
	 <//2>q,z,c,d,e,f,g
	 <7//3/1>w,x,c,d,e,f,g
	 <//4/2>e,c,c,d,e,f,g
	 <//5/0/1/1>r,v,c,d,e,f,g
	 <//6/0/2/2>t,b,c,d,e,f,g
	 <//7/3>y,n,c,d,e,f,g
	
	g.FromCsv(si ",")
	
	 foreach(_s si) g.RowAddSetMS(i _s 1); i+1
	
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	il.SetOverlayImages("0 1")
	g.SetImagelist(il)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	g.ToCsv(_s ",")
	ShowText "" _s
	
	case 4
	 out g.RowAddSetMS(4 "aaaa[0]bbbb[0]ccc" 3)
	 out g.RowAddSetMS(4 "aaaa[0]bbbb[0]ccc" 3 0 1)
	out g.RowAddSetMS(1 "<//8/2/2/1>aaaa[0]bbbb[0]ccc" 3 0 3)
	 out g.RowAddSetMS(15 "<//8/2/2/1>aaaa[0]bbbb[0]ccc" 3 0 3)
	 g.CellSet(1 1 "new")
	 ret
	 
	for i 0 g.Send(LVM_GETITEMCOUNT)
		LVITEMW li.mask=LVIF_IMAGE|LVIF_PARAM|LVIF_STATE|LVIF_INDENT
		li.iItem=i
		li.stateMask=-1
		if(g.Send(LVM_GETITEM 0 &li)) out "%i %i 0x%X %i" li.iImage li.lParam li.state li.iIndent
		 outx g.Send(LVM_GETITEMSTATE i -1)
	
	 g.Send(LVM_SORTITEMSEX g &lvsortproc)
	 g.Send(LVM_SORTITEMS g &lvsortproc)
	
ret 1
 messages3
NMHDR* nh=+lParam
 if(nh.code!=NM_CUSTOMDRAW) OutWinMsg message wParam lParam
 if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))
