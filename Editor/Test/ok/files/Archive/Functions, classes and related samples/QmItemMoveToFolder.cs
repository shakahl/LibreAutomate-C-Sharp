 \
function $folder

 Moves current macro to the folder.
 EXAMPLE
 QmItemMoveToFolder "\New\Users new"


QMITEM q qc
int idf=qmitem(folder 0 q); if(!idf or q.itype!=5) mes- "folder not found"
int idcurr=qmitem("" 0 qc)

str s.format("Click the item within 10 s" folder)
OnScreenDisplay s 1
wait 10 ML ;err ret

Acc a=acc(mouse)
if(a.Role!=ROLE_SYSTEM_OUTLINEITEM) ret
 s=a.Name
 int iid=qmitem(s

int tv=id(2202 _hwndqm)
key Cx
SendMessage tv TVM_SELECTITEM TVGN_CARET q.htvi
key Cv
int h=wait(1 WA win("Quick Macros" "#32770" "" 0x401)); err ret
but 6 h
SendMessage tv TVM_SELECTITEM TVGN_CARET qc.htvi
SendMessage tv TVM_SELECTITEM TVGN_CARET q.htvi
SendMessage tv TVM_SELECTITEM TVGN_CARET qc.htvi
