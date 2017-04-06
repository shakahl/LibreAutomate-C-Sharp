 This macro works in IE.

ClearOutput
 Find first cell (containing useful information) in the table.
 Since its contents is undefined, capture some object above
 the table and navigate to the cell. In the example is used
 navigation string "parent next7 first". You can discover the
 string, as well as other navigation strings, by looking at
 the object tree in the 'Find accessible object' dialog and
 experimenting with the Navigate field.
Acc a=acc("Order #" "TEXT" win(" Internet Explorer" "IEFrame") "Internet Explorer_Server" "" 0x1801 0x40 0x20000040 "parent next7 first")
 Now walk through the table using accessible object navigation
int i r
 For each row
rep ;;I don't know how many rows there are, so use rep and later break on error
	r+1; out "--- row %i ---" r
	 For each cell
	for i 0 5 ;;I know the number of columns. It is 5 (Order #, Customer, Date, Total, Sales Rep)
		str s=a.Name
		out s
		if(i!=4) a.Navigate("parent next first") ;;get next cell
	 get first cell in next row
	a.Navigate("parent next6 first")
	err break ;;if error, there are no more rows
	