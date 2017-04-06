function $range [$comment]  ;;range examples: "A1", ExcelRange(2 5)

 Adds, changes or deletes a comment.

 range - cell. <help>Excel range strings</help>.
 comment - comment text. If omitted or "", deletes comment from the cell.

 Added in: QM 2.3.3.
 Errors: Excel errors


WS

Excel.Range r=__Range(range)

Excel.Comment c=r.Comment
if empty(comment)
	if(c) c.Delete
else
	if !c
		c=r.AddComment
		c.Shape.TextFrame.Characters.Font.Bold=0; err ;;why it is bold initially??
	c.Visible=0
	_s=comment; _s.findreplace("[]" "[10]") ;;Excel does not use \r in comments; Excel 97 would display rectangles
	c.Text(_s)

err+ E
