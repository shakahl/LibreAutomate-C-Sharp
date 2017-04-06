function [$range] [what] ;;range examples: "A1" (cell), "3:3" (row), "C:C" (column), "A1:C3" (range).  what (flags): 0 everything, 1 contents, 2 comments, 4 formats

 Clears contents and/or other attributes of a range.

 range - cells to clear. Default: "" - whole used range. <help>Excel range strings</help>.

 Added in: QM 2.3.3.
 Errors: Excel errors

 EXAMPLES
  /exe 1
 ExcelSheet es.Init
 es.Clear("3:3" 1|2) ;;clears contents and comments of row 3
 
 es.Clear("<number>" 1); err ;;clear all numbers


WS

Excel.Range r=__Range(range)

if what
	if(what&1) r.ClearContents
	if(what&2) r.ClearComments
	if(what&4) r.ClearFormats
else r.Clear

err+ E
