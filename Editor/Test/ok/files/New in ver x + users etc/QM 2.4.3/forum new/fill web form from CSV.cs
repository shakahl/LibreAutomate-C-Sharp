 create CSV string for testing
str csv=
 A1, B1, C1
 A2, B2, C2
 A3, B3, C3

 load the CSV into a variable
ICsv x._create
x.FromString(csv) ;;replace to FromFile("c:\...\your CSV file.csv")

 open the web page
run "iexplore" "https://www.google.com/advanced_search"
mes- "please wait until Internet Explorer opens Google advanced search page. This macro uses it as an example of a web form with multiple edit boxes" "" "OCi"

 get IE window handle
int w=wait(3 WV win("" "IEFrame"))

 repeat for each row in CSV
int i n=x.RowCount
for i 0 n
	 find first edit box in web page
	Htm e=htm("INPUT" "as_q" "" w "0" 0 0x221 3) ;;to create this code, use dialog 'Find HTML element'
	 set its text from CSV first column
	e.SetText(x.Cell(i 0))
	 do the same with other edit boxes and CSV columns
	e=htm("INPUT" "as_epq" "" w "0" 1 0x221 3)
	e.SetText(x.Cell(i 1))
	e=htm("INPUT" "as_oq" "" w "0" 2 0x221 3)
	e.SetText(x.Cell(i 2))
	 find and click a Submit button
	e=htm("INPUT" "submit" "" w "0" 14 0x521 3)
	e.Click
	 now you can open another such form, and the macro will fill the edit boxes with next CSV row
	if(i<n-1) mes- "click OK when ready to repeat with next CSV row" "" "OCi"
	else mes "Done" "" "Oi"
	