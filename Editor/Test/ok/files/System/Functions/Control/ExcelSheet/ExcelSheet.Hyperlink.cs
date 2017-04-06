function $range [$address] [$linkText] [$tipText] ;;range examples: "A1" (cell), "B1:D1" (3 cells)

 Adds, changes or deletes a hyperlink.

 range - cell or cells where to add/modify hyperlink. <help>Excel range strings</help>.
 address - URL to a web page or file, or bookmark to a sheet/cell.
   Like with Excel menu Insert -> Hyperlink.
   If omitted or "", deletes hyperlink. Does not delete text.
 linkText - text to display.
 tipText - screentip text. Not used with oldest Excel versions.

 EXAMPLE
  /exe
 ExcelSheet es.Init
 es.Hyperlink("A15" "#Sheet1!C1" "link")

 Added in: QM 2.3.3.
 Errors: Excel errors


WS

Excel.Range r=__Range(range)
if empty(address)
	r.Hyperlinks.Delete; err
else
	if(!empty(linkText)) r.Value=linkText
	IDispatch d=ws.Hyperlinks
	d=d.Add(r address) ;;info: in old Excel 3 params
	if(!empty(tipText)) d.ScreenTip=tipText; err

err+ E
