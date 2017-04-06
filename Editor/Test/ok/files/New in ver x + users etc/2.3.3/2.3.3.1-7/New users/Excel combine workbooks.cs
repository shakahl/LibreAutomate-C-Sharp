 This macro copies all sheets from multiple Excel workbook files to the current workbook. From all xls files in a folder.

 HOW TO USE
 1. Change folder path in this macro. It is where are your Excel workbook files (xls).
 2. In Excel create new empty workbook. Save somewhere in other folder. You must save it before running this macro.
 3. Run this macro and wait.
 4. Delete the first 3 empty sheets, because this macro does not delete existing sheets.

 NOTES
 This macro temporarily opens each file in the same Excel instance.

out
str folder="$Documents$\Excel" ;;change this

 ---------------------------

ExcelSheet es esm.Init
Excel.Workbook wb=esm.ws.Parent

Dir d
foreach(d F"{folder}\*.xls" FE_Dir) ;;for each file
	str sPath=d.FileName(1)
	out sPath
	str filename=d.FileName; filename.fix(filename.len-4)
	es.Init(1 4 sPath)
	Excel.Workbook wb2=es.ws.Parent
	rep ;;for each sheet
		str name.from(filename " " es.ws.Name)
		out name
		es.ws.Copy(@ wb.Sheets.Item(wb.Sheets.Count))
		wb.ActiveSheet.Name=name
		es.ws=es.ws.Next; if(!es.ws) break
	wb2.Close
