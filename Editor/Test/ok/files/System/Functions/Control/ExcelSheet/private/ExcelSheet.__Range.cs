function'Excel.Range $range [Excel.Worksheet&w]

if(!&w) &w=ws

if(empty(range)) ret w.UsedRange
sel range 1
	case ["<sel>","sel"] ret w.Application.Selection
	case "<used>" ret w.UsedRange
	case "<all>" ret w.Cells
	case "<constant>" ret w.Cells.SpecialCells(Excel.xlCellTypeConstants)
	case "<number>" ret w.Cells.SpecialCells(Excel.xlCellTypeConstants Excel.xlNumbers)
	case "<text>" ret w.Cells.SpecialCells(Excel.xlCellTypeConstants Excel.xlTextValues)
	case "<formula>" ret w.Cells.SpecialCells(Excel.xlCellTypeFormulas)
	case "<blank>" ret w.Cells.SpecialCells(Excel.xlCellTypeBlanks)
	case "<active>" ret w.Application.ActiveCell
	case else ret w.Range(range)

err+ end _error
