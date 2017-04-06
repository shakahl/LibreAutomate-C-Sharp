 This macro gets table cells to 2-dim array.
 Works in IE and FF.

int ff
 ff=1 ;;enable this for firefox

ClearOutput

 Create empty 2-dim array.
ARRAY(str) ar.create(5 0)
 Get first cell.
Acc a
if(ff) a=acc("Order #" "TEXT" win("Mozilla Firefox" "MozillaUIWindowClass") "MozillaContentWindowClass" "" 0x1801 0x40 0x20000040 "parent next7 first")
else a=acc("Order #" "TEXT" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1801 0x40 0x20000040 "parent next7 first")
 Walk through the table.
int c r
for r 0 1000000 ;;for each row
	ar.redim(-1) ;;add empty elements for 1 row
	
	for c 0 5 ;;for each cell in the row
		ar[c r]=a.Name
		if(c!=4) a.Navigate("parent next first")
		
	a.Navigate(iif(ff "parent next3 first" "parent next6 first"))
	err break

 Show results.
for r 0 ar.len(2)
	out "--- row %i ---" r+1
	for c 0 ar.len(1)
		str s=ar[c r]
		out s
		