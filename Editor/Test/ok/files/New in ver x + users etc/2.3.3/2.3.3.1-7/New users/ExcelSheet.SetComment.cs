function $cell $text

 Adds or changes cell comment.

 cell - cell, such as "A1". Use "sel" for current selection.
 text - comment text.

 EXAMPLE
 ExcelSheet es.Init
 es.SetComment("A2" "bbbbbb")


Excel.Range r
if(!StrCompareN(cell "sel" 3 1)) r=ws.Application.Selection
else r=ws.Range(cell)

r.AddComment(text)
err ;;error if comment already exists
	Excel.Comment c=r.Comment
	c.Text(text)
