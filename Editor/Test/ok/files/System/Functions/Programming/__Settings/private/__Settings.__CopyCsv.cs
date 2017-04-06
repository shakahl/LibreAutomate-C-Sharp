function'ICsv removeColumn ;;removeColumn: if>=0, removes this column

ICsv c._create
int i nc=m_c.ColumnCount
for(i 0 m_c.RowCount) c.AddRowMS(i nc m_c.Cell(i 0))

if(removeColumn>=0) c.RemoveColumn(removeColumn)

ret c
