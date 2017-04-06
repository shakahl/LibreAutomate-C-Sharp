function [`sheet] [`workbook]

 Deletes this or other sheet.

 sheet - sheet to delete.
   Default: "" - deletes this worksheet and clears this variable.
   Can be name, 1-based sheet index, or object.
   Can be worksheet, chart or sheet of other type.
 workbook - parent workbook.
   Default: "" - workbook of this worksheet.
   Can be name, 1-based index, or object.

 Added in: QM 2.3.3.
 Errors: <ExcelSheet._Sheet>


WS

IDispatch x=this._Sheet(sheet workbook)

__ExcelState _.Init(this 1)
__CallWsOrChart(x 2)

if(x=ws) ws=0

err+ E
