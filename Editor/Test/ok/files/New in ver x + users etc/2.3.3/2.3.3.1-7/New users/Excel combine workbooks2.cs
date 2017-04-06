 This macro creates new Excel workbook on desktop, and copies all sheets from multiple Excel workbook files to it. From all xls files in a folder.

 HOW TO USE
 1. Change folder path in this macro. It is where are your Excel workbook files (xls).
 2. Change new file path/name in this macro.
 3. Run this macro and wait. At the end it will open the new file.

out
str folder="$Documents$\Excel" ;;change this
str newFileName.timeformat("$desktop$\{yyyy}-{MM}-{dd} {HH}-{mm}-{ss}.xls") ;;change this

 ---------------------------

 create and save master workbook
ExcelSheet esm.Init(0 8) ;;I used ExcelSheet because its dtor calls Application.Quit
Excel.Application xlApp=esm.ws.Application
Excel.Workbook wbm=xlApp.ActiveWorkbook
wbm.SaveAs(_s.expandpath(newFileName) Excel.xlNormal @ @ @ @ 1) ;;if fails, possibly filename contains illegal characters, or the file exists

 copy sheets from other workbooks
Dir d
foreach(d F"{folder.expandpath}\*.xls" FE_Dir) ;;for each file
	str sPath=d.FileName(1)
	out sPath
	str filename=d.FileName; filename.fix(filename.len-4)
	Excel.Workbook wb=xlApp.Workbooks.Open(sPath)
	Excel.Worksheet ws
	foreach ws wb.Sheets ;;for each sheet
		ws.Copy(@ wbm.Sheets.Item(wbm.Sheets.Count))
		str name.from(filename " " ws.Name)
		out name
		wbm.ActiveSheet.Name=name ;;if fails, probaby name is too long. Somehow make shorter. Also fails if a sheet with this name already exists.
	wb.Close

 delete first 3 empty sheets, select first sheet, save
rep(3) wbm.Sheets.Item(1).Delete
wbm.Sheets.Item(1).Activate
wbm.Save
wbm.Close ;;thank you TheVig

 open in Excel to see
run newFileName
