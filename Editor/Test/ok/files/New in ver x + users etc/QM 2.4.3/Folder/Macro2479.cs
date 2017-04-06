 Example.
 At first open both files in Excel.
 Edit file names in this macro to match what is displayed in Excel title bar for each file.
 Then run this macro.

ExcelSheet A B ;;variables
A.Init("" 0 "Book1.xls") ;;connect to the first open file
B.Init("" 0 "Book2.xls") ;;connect to other open file

str s=A.Cell("A1") ;;get cell
if s.len ;;if not empty
	out s ;;display in QM
	B.SetCell(s "C10") ;;set cell in other file
