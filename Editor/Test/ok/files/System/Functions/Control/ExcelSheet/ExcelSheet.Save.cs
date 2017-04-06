function [$saveas] [flags] ;;flags: 1 save all workbooks, 2 no alerts, 4 copy

 Saves workbook of this worksheet, or all workbooks.

 saveas - save to this file.
   If omitted or "", executes "Save" command.
   Else if flag 4 used, saves a copy of this file (read more below).
   Else executes "Save As" command.
 flags:
   1 - save all workbooks that are opened in this Excel instance. If used, saveas must be "".
   2 (QM 2.3.3) - don't show Excel message boxes. Excel will choose default response. For example, error if "Save As" to an existing file.
   4 (QM 2.3.3) - save a copy of current file. Don't change/save current file. If the file (saveas) already exists, overwrite. This for example can be used to create a backup copy.

 REMARKS
 Does not change file format, regardless of saveas file extension. See example 2.

 EXAMPLES
  /exe 1
 ExcelSheet es.Init
  ...
 es.Save
 
  save as csv, and close
 Excel.Workbook wb=es._Book ;;get workbook of this worksheet
 __ExcelState _es.Init(es 1) ;;temporarily disables Excel alerts (message boxes)
 wb.SaveAs(_s.expandpath("$documents$\test ExcelSheet.csv") Excel.xlCSV @ @ @ @ 1)
 _es.AlertsRestore
 wb.Saved=-1
 wb.Close


WS

if((flags&1 and !empty(saveas)) or (flags&4 and empty(saveas))) end ERR_BADARG

__ExcelState _.Init(this flags&2!0)
Excel.Workbook wb

if flags&1
	foreach(wb ws.Application.Workbooks) wb.Save
else
	wb=ws.Parent
	if empty(saveas)
		wb.Save
	else
		_s.expandpath(saveas)
		if(flags&4) wb.SaveCopyAs(_s)
		else wb.SaveAs(_s @ @ @ @ @ 1)

err+ end ERR_SAVE
