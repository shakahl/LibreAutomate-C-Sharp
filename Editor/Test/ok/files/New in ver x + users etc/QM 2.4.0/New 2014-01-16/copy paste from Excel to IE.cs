 1. Open file in Excel
ExcelSheet es.Init("" 8 "$documents$\Book1.xls")
 2. Get cell
str cell=es.Cell("A1")
 3,4. Open web page in Internet Explorer
int hwndIE
web "http://www..." 8|1 "" "" 0 hwndIE
 4. Password
AutoPassword "user" "password" 1|4 hwndIE 10
 5. Navigate to form
web "http://www..." 1 hwndIE
 5. Paste data
paste cell ;;before this probably will need to add some code to set focus to the form field
 6. Get/paste more cells
cell=es.Cell("A2")
paste cell ;;before this probably will need to add some code to set focus to the form field
 7. Close programs
clo hwndIE
es.Close
