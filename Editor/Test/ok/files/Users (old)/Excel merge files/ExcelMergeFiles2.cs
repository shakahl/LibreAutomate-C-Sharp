typelib Excel {00020813-0000-0000-C000-000000000046} 1.2 0 1

Excel.Application a._create ;;create new Excel instance
Excel.Worksheet ws=a.Workbooks.Add.Worksheets.Item(1) ;;create new workbook and get first worksheet
Excel.Range r_dest=ws.Range("A:C") ;;select first 3 columns. You need to change this.
int row_counter=1

 add xls files copied to the clipboard
ARRAY(str) arr
int i
for i 0 GetClipboardFiles(arr)
	ExcelStripAndAdd arr[i] r_dest row_counter

 show Excel. You will need to save (in some other folder).
a.Visible=TRUE
