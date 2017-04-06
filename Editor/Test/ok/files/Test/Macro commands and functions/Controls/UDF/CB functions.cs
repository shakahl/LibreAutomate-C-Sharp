key AF12

int h=id(1003 "Options")
out control.CB_GetCount(h)
int i=control.CB_FindItem(h "Courier")
out i
str s
control.CB_GetItemText(h i s)
out s
out control.CB_SelectedItem(h s)
out s
2
control.CB_SelectItem(h 5)
2
control.CB_SelectString(h "Courier New")
