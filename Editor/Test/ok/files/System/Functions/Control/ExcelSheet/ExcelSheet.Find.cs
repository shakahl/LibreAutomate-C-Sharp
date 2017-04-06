function! `what ARRAY(Excel.Range)&aFound [flags] [$range] ;;flags: 1 match case, 2 match entire cell, 4 find all, 8 within workbook, 16 by columns, 0x100,0x200 look in (0 formulas, 0x100 values, 0x200 comments)

 Finds cells.
 Returns: 1 found, 0 not found.

 what - find what. Can be of any type supported by Excel (text, number, DATE, VARIANT).
 aFound - variable for results.
 flags - find options. The same as in Excel Find dialog. Some flags:
   4 - find all matching cells. If not set, finds the first matching cell, and aFound will have 1 element.
   8 - search in all worksheets of this workbook. If not set - in this worksheet only.
 range - part of worksheet where to search. Default: "" - all. <help>Excel range strings</help>.

 Added in: QM 2.3.3.

 EXAMPLES
  find single cell containing "c"
 ExcelSheet es.Init
 ARRAY(Excel.Range) a
 if(!es.Find("c" a)) out "not found"; ret
  show results
 out a[0].Value; out a[0].Column; out a[0].Row
 es.ws.Application.Goto(a[0])

  find all cells containing "c"; search in all worksheets
 ExcelSheet es.Init("" 16)
 ARRAY(Excel.Range) a; int i
 es.Find("c" a 4|8)
  show results
 for i 0 a.len
	 Excel.Range r=a[i]
	 Excel.Worksheet w=r.Parent
	 str sWS(w.Name) sAddr(r.Address(0 0 1))
	 out "%s: %s (R%iC%i)" sWS sAddr r.Row r.Column
	 w.Activate; r.Activate
	 1


WS

aFound=0

if(flags&8=0) ret __Find(ws range what aFound flags)

Excel.Workbook b=ws.Parent
Excel.Worksheet w
foreach w b.Worksheets
	if(__Find(w range what aFound flags) and flags&4=0) break

ret aFound.len!0

err+ E
