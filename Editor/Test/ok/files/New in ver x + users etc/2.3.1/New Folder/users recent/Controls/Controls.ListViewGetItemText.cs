function! hlv item column str&text

 Gets text of a listview control item.
 Returns 1 if successful, 0 if not.
 List view control class name is "SysListView32" or similar.
 Fails if the process belongs to another user.
 On Vista, fails if QM is running as User and the process has higher integrity level.

 hlv - list view control handle.
 item - zero-based item index.
 column - zero-based column index.
 text - str variable that receives text.


if(!hlv) end ES_WINDOW
text.len=0

if(!InitPm(hlv)) ret

LVITEMW* lip2=m_pm.address
LVITEMW lip
lip.pszText=+(lip2+sizeof(lip))
lip.cchTextMax=260
lip.iSubItem=column
lip.mask=LVIF_TEXT
m_pm.Write(&lip sizeof(lip))
int tl=SendMessageW(hlv LVM_GETITEMTEXTW item lip2)
if(tl)
	m_pm.ReadStr(text tl*2 sizeof(lip) 1)
	ret 1

err+
