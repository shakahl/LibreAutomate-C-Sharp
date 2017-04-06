function# ~name [`sheet] [`book]

 Renames this or other sheet.

 name - new name.
 sheet - sheet to rename.
   Default: "" - this.
   Can be name, 1-based sheet index, or object.
   Can be worksheet, chart or sheet of other type.
 book - parent workbook.
   Default: "" - workbook of this worksheet.
   Can be name, 1-based index, or object.

 REMARKS
 Excel sheet name rules:
   Must not contain : \ / ? * [ ]
   Must not be longer than 31. With this function - 28.
   Must not be empty.
   Must not be the same as of another sheet (worksheet, chart etc).
 If name does not mach these rules, this function corrects it and returns 1. If match, returns 0.

 Added in: QM 2.3.3.
 Errors: <>, <ExcelSheet._Sheet>


WS

IDispatch x=this._Sheet(sheet book)

int r=name.replacerx("[:\\/\?\*\[\]]" "_")
r|name.LimitLen(28) ;;31=28+("1" to "999")

int i n=name.len
for i 1 1000
	__CallWsOrChart(x 1 name)
	err
		name.fix(n); name+i
		r=1
		continue
	break
if(i=1000) end ERR_FAILED

ret r!0

err+ E
