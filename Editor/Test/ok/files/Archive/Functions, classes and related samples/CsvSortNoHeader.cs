 /
function ICsv&c flags [col] ;;flags: 0 simple, 1 insens, 2 ling, 3 ling/insens, 4 number/ling/insens, 0x100 descending

 Sorts skipping first row.
 See <help "::/User/IDP_ICSV.html">ICsv.Sort</help>.


if(c.RowCount<3) ret

c.GetRowMS(0 _s); c.RemoveRow(0)
c.Sort(flags col)
c.AddRowMS(0 c.ColumnCount _s)

err+ end _error
