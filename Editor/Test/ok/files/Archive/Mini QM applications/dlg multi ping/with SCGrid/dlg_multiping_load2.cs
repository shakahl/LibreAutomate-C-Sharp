 /
function! hDlg [flags] ;;flags: 1 launch

prjSCGrid.SCGrid gridping._getcontrol(id(3 hDlg))
gridping.Rows=1

str spl.getfile("$my qm$\ping list.txt"); err ret

ARRAY(str) a=spl
int i
for(i a.len-1 -1 -1) if(!a[i].len or a[i][0]=32 or a[i][0]=';') a.remove(i)
if(!a.len) ret

int per=val(_s.getwintext(id(7 hDlg))); if(per<1) per=1

gridping.Rows=2; gridping.RowBackColor(1)=0xffffff; gridping.RowStyle(1)=0 ;;other rows inherit style of current last row
gridping.Rows=a.len+1
for i 0 a.len
	gridping.Text(i+1 0)=a[i]
	if(flags&1) mac "dlg_multiping_thread" "" hDlg i+1 a[i] per

ret 1
