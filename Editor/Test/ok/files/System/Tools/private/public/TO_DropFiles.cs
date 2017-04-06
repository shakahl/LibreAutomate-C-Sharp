 /
function! hDlg QMDRAGDROPINFO&di [$sid] [idLnkTarget] [idLnkParam]

 sid - list of edit control ids, like "3 4 5". "id-" will not unexpand

int i j f nosf h=child(mouse); if(!IsWindowEnabled(h)) ret
str s

ARRAY(str) a
if(tok(sid a -1 " "))
	for(i 0 a.len)
		if(GetDlgCtrlID(h)=val(a[i] 0 j))
			nosf=a[i][j]='-'
			break
	if(i=a.len) ret

if(!WinTest(h "Edit")) ret

s.getl(di.files)

f=sub_to.File_GetPathFlags(hDlg)
if(f&1=0) sub_to.File_LinkTarget s 0 hDlg iif((idLnkParam and GetDlgCtrlID(h)=idLnkTarget) idLnkParam 0)
if(!nosf and f&2=0) s.expandpath(s 2)

TO_SetText s 0 h 1
ret 1
