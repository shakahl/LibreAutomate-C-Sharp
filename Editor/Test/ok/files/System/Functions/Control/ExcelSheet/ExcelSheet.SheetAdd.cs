function'IDispatch [$name] [`book] [`templ] ;;templ: use "copy" to copy this worksheet

 Adds sheet. Can add empty, from template, or copy.
 Returns object of the new sheet, by default Excel.Worksheet.

 name - sheet name. Optional.
 book - parent workbook.
   Default: "" - workbook of this worksheet.
   Can be name, 1-based index, or object.
 templ - sheet type, or template (.xlt file), or "copy" (copy this worksheet).
   Type can be: Excel.xlWorksheet (default), xlChart, xlExcel4MacroSheet, or xlExcel4IntlMacroSheet.
   Template example: "$pf$\Microsoft Office\Templates\1033\timecard.xlt".

 REMARKS
 You can assign the return value to an ExcelSheet variable (this or other). Or to an Excel.Worksheet or other (depends on sheet type).

 Added in: QM 2.3.3.
 Errors: <>, Excel errors

 EXAMPLE
  /exe 1
 ExcelSheet es.Init
 ExcelSheet es2=es.SheetAdd("test")
 es2.SetCell("z" 1 1)


WS

Excel.Workbook b=this._Book(book); err end F"{ERR_FILE} (book)"
Excel.Sheets c=b.Sheets
IDispatch x after=c.Item(c.Count)

int copy
if(!__VtReal(templ)) templ=Excel.xlWorksheet
else if(templ.vt=VT_BSTR)
	if(!templ.cmp("copy")) copy=1
	else _s=templ; templ=_s.expandpath

__ExcelState _.Init(this 2)

if copy
	ws.Copy(@ after)
	x=b.ActiveSheet
else
	x=c.Add(@ after @ templ)

if(!empty(name)) SheetRename(name x)

ret x

err+ E
