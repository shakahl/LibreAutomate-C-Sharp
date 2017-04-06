 1. Open file in Excel
run "C:\Users\jgarnett\Documents\VBA Test.xls"
int hwndExcel=wait(30 WA "VBA Test")
1
ExcelSheet es.Init
 2. Get cell
str cell=es.Cell("A1")
 3,4. Open web page in Internet Explorer
int hwndIE
web "http://www..." 1 "" "" 0 hwndIE
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
clo hwndExcel
