key AF12; key R
 act "Options"

int h=id(1285 "Options")
lef 23 8 h
0.5
out control.LB_GetCount(h)
int i=control.LB_FindItem(h "Window")
out i
str s
control.LB_GetItemText(h i s)
out s
out control.LB_SelectedItem(h s)
out s
2
control.LB_SelectString(h "Window")
1
control.LB_SelectItem(h 1 3)
