lpstr tabName="Macro1261"

int w1=win("QM outex" "QM_outex")
Acc a=acc(tabName "PAGETAB" w1 "SysTabControl32" "" 0x1001); err ret
int idEdit=a.elem+99
 out idEdit
str s.getwintext(id(idEdit w1))
out s
