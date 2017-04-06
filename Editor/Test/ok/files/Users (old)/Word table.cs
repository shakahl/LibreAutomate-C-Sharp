typelib Word {00020905-0000-0000-C000-000000000046} 8.0 0x409 ;;Microsoft Word 8.0 Object Library, ver 8.0
Word.Application a._getactive
Word.Table t=a.ActiveDocument.Tables.Item(1) ;;1 is table index
str s
Word.Cell c
foreach c t.Columns.Item(2).Cells
	s=c.Range.Text
	s.fix(s.len-2)
	out s

 int i nr=t.Rows.Count
 str s
 for i 1 nr+1
	 s=t.Cell(i 2).Range.Text
	 s.fix(s.len-2)
	 out s
