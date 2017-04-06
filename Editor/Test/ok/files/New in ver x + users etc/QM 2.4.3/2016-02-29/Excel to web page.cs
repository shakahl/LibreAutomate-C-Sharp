 get all used cells from Excel row 1:1 to array variable a
ARRAY(str) a
ExcelSheet e.Init
e.CellsToArray(a "1:1")

 activate web browser and get its window handle
int w=act(win("Firefox" "MozillaWindowClass"))

 for each cell
int i
for i 0 a.len(1)
	 find HTML edit box accessible object. To create this example code I used dialog 'Find accessible object'.
	Acc a1.FindFF(w "INPUT" "" "name=keywords" 0x1004 3)
	 set keyboard focus
	a1.Select(1)
	 input cell text
	key (a[i 0])
	
	 here add code that creates the resulting information
	
	 then find accessible object that contains the resulting information
	 Acc a2.Find...
	
	 get the resulting information into an array variable (here I use the same variable)
	 a[i 0]=a2.Value

 copy cells from array a to Excel row 2:2
e.CellsFromArray(a "2:2")
