\Dialog_Editor
function# hDlg message wParam lParam
if(hDlg) goto messages

ref GRID

if(!ShowDialog("" &dlg_QM_Grid_LVITEM 0 _hwndqm)) ret

 BEGIN DIALOG
 0 "" 0x90C80A44 0x100 0 0 399 175 "QM_Grid"
 3 QM_Grid 0x56031041 0x0 0 0 400 154 "0[]noedit,150[]edit,50[]edit+button,80,16[]edit multiline,80,8[]combo,80,1[]check,50,2[]read-only,80,7"
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
	 g.Send(LVM_SETEXTENDEDLISTVIEWSTYLE LVS_EX_CHECKBOXES LVS_EX_CHECKBOXES)
	
	lpstr si=
	 <//2>q,z,c,d,e,f,g
	 <7//3 1>w,x,c,d,e,f,g
	 <//4 2>e,c,c,d,e,f,g
	 <//5 0 1/1>r,v,c,d,e,f,g
	 <//6 0 2/2>t,b,c,d,e,f,g
	 <//7/3>y,n,c,d,e,f,g
	
	foreach(_s si)
		LVITEM la.mask=LVIF_IMAGE|LVIF_PARAM|LVIF_STATE|LVIF_INDENT
		la.iImage=i+3
		la.lParam=i*2
		la.iIndent=i/2
		la.state=0x7100; la.stateMask=0xff00
		g.RowAddSetMS(i _s 1 0 0 &la)
		i+1
	
	__ImageList- il.Load("$qm$\il_dlg.bmp")
	il.SetOverlayImages("0 1")
	g.SetImagelist(il il)
	
	case WM_DESTROY
	case WM_COMMAND goto messages2
	case WM_NOTIFY goto messages3
ret
 messages2
sel wParam
	case IDOK
	 get and show all cells
	g.ToCsv(_s ",")
	ShowText "" _s
	
	case 4
	 out g.RowAddSetMS(5 "aaaa[0]bbbb[0]ccc" 3)
	 out g.RowAddSetMS(1 "<//8/2>aaaa[0]bbbb[0]ccc" 3)
	 ret
	
	 for i 0 5
		 opt waitmsg 1
		 la.mask=LVIF_IMAGE|LVIF_PARAM|LVIF_STATE|LVIF_INDENT
		 la.iImage=i+5
		 la.lParam=5
		 la.iIndent=i/2
		 la.state=(i<<12)|0x200; la.stateMask=0xff00
		  la.state=2; la.stateMask=0xff
		  out g.RowAddSetMS(1 "repl" 1 0 1 &la)
		 out g.RowAddSetMS(1 "" 0 0 1 &la)
		  la.iItem=1; g.Send(LVM_SETITEM 0 &la)
		 1
	 
	for i 0 g.Send(LVM_GETITEMCOUNT)
		LVITEM li.mask=LVIF_IMAGE|LVIF_PARAM|LVIF_STATE|LVIF_INDENT
		li.iItem=i
		li.stateMask=-1
		 if(g.Send(LVM_GETITEM 0 &li)) out "%i %i 0x%X %i" li.iImage li.lParam li.state li.iIndent
		 out g.RowGetMS(i 1 0 0 0 li)
		if(g.RowGetMS(i 0 0 0 0 li)>0) out "%i %i 0x%X %i" li.iImage li.lParam li.state li.iIndent
		 ARRAY(str) a
		 out g.RowGetSA(a i 1 0 0 li)
		 if(g.RowGetSA(0 i 0 0 0 li))
			 out a[0]
			 out "%i %i 0x%X %i" li.iImage li.lParam li.state li.iIndent
		 outx g.Send(LVM_GETITEMSTATE i -1)
	
	 g.Send(LVM_SORTITEMSEX g &lvsortproc)
	 g.Send(LVM_SORTITEMS g &lvsortproc)
	
ret 1
 messages3
NMHDR* nh=+lParam
 if(nh.code!=NM_CUSTOMDRAW) OutWinMsg message wParam lParam
if(nh.idFrom=3) ret DT_Ret(hDlg gridNotify(nh))
