 Gets data from Excel (must be at least 3 columns) and
 populates 3 fields in Google advanced search page.
 Repeats for each row.

ExcelSheet es.Init
ARRAY(str) a
es.CellsToArray(a "A:C")

int w=win("Google Advanced Search - Mozilla Firefox" "MozillaWindowClass")
 int w=win("Google Advanced Search - Google Chrome" "Chrome_WidgetWin_1")
 int w=win("Google Advanced Search - Internet Explorer" "IEFrame")
act w

int row
for row 0 a.len ;;for each row
	 select the first field
	Acc k.Find(w "TEXT" "all these words:" "" 0x3091 3)
	k.Select(1)
	 if difficult to create this code for your web page, delete this code and add code to select with keyboard or mouse
	
	key CaX (a[0 row]) T
	key CaX (a[1 row]) T
	key CaX (a[2 row])
	
	 here you can add code to select files. Will need to automate the file selection dialog, probably with key too.
	
