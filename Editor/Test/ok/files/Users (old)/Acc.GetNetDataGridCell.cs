function# nColumns [row] [col] [Acc&ac]

 Retrieves accessible object for specified cell in NET data grid.
 This object must be data grid. You can call this function multiple times with it.
 Returns the number of used rows.
 nColumns is the number of columns in the grid.
 row and col are 1-based row and column indexes.

 EXAMPLE
 Acc aGrid=acc("DataGrid" "TABLE" win("Customer Details" "WindowsForms10.Window.8.app4") "WindowsForms10.Window.8.app4" "" 0x1001)
 Acc aCell
 int i
 for i 0 aGrid.GetNetDataGridCell(11)
	 aGrid.GetNetDataGridCell(11 i+1 1 aCell)
	 out aCell.Value
	  aCell.SetValue("new value")


int n=a.ChildCount-nColumns-2
if(&ac)
	if(row>n) end "row too big"
	str s.format("child%i child%i" row+nColumns+1 col)
	Navigate(s ac)
	
ret n
err+ end _error
