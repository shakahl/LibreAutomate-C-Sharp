 Gets data from Excel (must be at least 3 columns) and
 populates 3 fields in Google advanced search page.
 Repeats for each row.

ExcelSheet es.Init
ARRAY(str) a
es.CellsToArray(a "A:C")

int w=win(" Internet Explorer")
act w

Htm el
int row
for row 0 a.len ;;for each row
	el=htm("INPUT" "as_q" "" w 0 0 0x221 5)
	el.SetText(a[0 row])
	el=htm("INPUT" "as_epq" "" w 0 3 0x221)
	el.SetText(a[1 row])
	el=htm("INPUT" "as_eq" "" w 0 5 0x221)
	el.SetText(a[2 row])
	1
	el=htm("INPUT" "submit" "" w 0 12 0x521)
	 el.Click
	 15 I
	 3
	 web "Back"
	