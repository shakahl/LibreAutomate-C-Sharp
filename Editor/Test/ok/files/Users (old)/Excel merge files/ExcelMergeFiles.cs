typelib Excel {00020813-0000-0000-C000-000000000046} 1.2 0 1

Excel.Application a._create ;;create new Excel instance
Excel.Worksheet ws=a.Workbooks.Add.Worksheets.Item(1) ;;create new workbook and get first worksheet
Excel.Range r_dest=ws.Range("A:C") ;;select first 3 columns. You need to change this.
int row_counter=1

 add all xls files that are on the desktop, in random order
Dir d; str sPath
foreach(d "$Desktop$\*.xls" FE_Dir)
	sPath=d.FileName(1)
	 out sPath
	ExcelStripAndAdd sPath r_dest row_counter

  or, use list of files
 str sPath
 lpstr files=
  file1
  file2
  ...
 foreach sPath files
	 ExcelStripAndAdd sPath r_dest row_counter

 show Excel. You will need to save (in some other folder).
a.Visible=TRUE
