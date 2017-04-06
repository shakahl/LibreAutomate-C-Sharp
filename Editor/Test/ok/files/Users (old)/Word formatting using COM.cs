typelib Word {00020905-0000-0000-C000-000000000046} 8.3
Word.Application app._getactive
Word.Document doc=app.ActiveDocument
Word.Table table

foreach table doc.Tables
	if(table.range.ComputeStatistics(Word.wdStatisticCharacters)=0)
		table.Delete
	else
		table.AutoFitBehavior(Word.wdAutoFitWindow)
