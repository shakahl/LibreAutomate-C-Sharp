out
int w1=win(" - Mozilla Firefox" "Mozilla*WindowClass" "" 0x800)
 act w1
Q &q
 Acc a=acc("Esteban Ruiz Spain Trade" "TEXT" w1 "" "" 0x1801 0x40 0x20000040)
 Acc a=AccPath2(w1 "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT/div/div/div/div/div/div/div/div/TEXT" "Esteban Ruiz Spain Trade" 0x0801)
 Acc a; AccPath2(w1 "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT/div/div/div/div/div/div/div/div/TEXT" "Esteban Ruiz Spain Trade" 0x0801 a)
 Acc a=acc("" "APPLICATION" w1); a.Navigate(
Acc a=AccPath2(w1 "APPLICATION/GROUPING/PROPERTYPAGE/browser/DOCUMENT" "" 0x0881)
Q &qq
outq
if(a.a)
	out a.Name
else out "<not found>"

 acc