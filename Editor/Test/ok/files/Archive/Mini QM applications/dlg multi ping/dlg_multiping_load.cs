 /
function! hDlg [flags] ;;flags: 1 launch

int hlv=id(3 hDlg)
SendMessage hlv LVM_DELETEALLITEMS 0 0

str spl.getfile("$my qm$\ping list.txt"); err ret

ARRAY(str) a=spl
int i
for(i a.len-1 -1 -1) if(!a[i].len or a[i][0]=32 or a[i][0]=';') a.remove(i)
if(!a.len) ret

int per=val(_s.getwintext(id(7 hDlg))); if(per<1) per=1

for i 0 a.len
	TO_LvAdd hlv i 0 0 a[i]
	if(flags&1) mac "dlg_multiping_thread" "" hDlg i a[i] per

ret 1
