 get data from file mm.xls into array
ARRAY(str) a ;;array variable for sheet data
ExcelSheet es.Init("" 8 "$documents$\mm.xls") ;;open file in hidden EXCEL process
es.GetCells(a)

 activate the order window
int w1=win("Basket Order")
act w1
 also may need to set focus to the first control (field in the window). To insert the code, use 'Window' dialog from the floating toolbar, or press Ctrl+Shift+Alt+W...


 for each row
int r ;;variable for row index
for r 0 a.len(2)
	 The following code must populate controls from a single row.
	 It depends on how you will do it.
	 To insert the code you can use dialogs from the floating toolbar.
	
	 for text fields can be used keyboard, assuming that the field is focused
	key Ca (a[0 r]) T ;;press Ctrl+A to select existing text (to be erased), type first column's text, and press Tab to set focus to the next control
	
	 for combo boxes use code like this
	CB_SelectString child a[1 r]; key T
	 however this will not work if it is not a standard combo box control. Then try keyboard or accessible objects.
	
	 and so on
	
	 Finally submit and set focus to the first field, to be ready for the next row.
	 For this probably can be used keyboard.
	 If need human interaction, display message box, which will pause the macro.
	
