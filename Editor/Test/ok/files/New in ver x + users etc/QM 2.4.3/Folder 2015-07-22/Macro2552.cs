ICsv x._create
x.FromString("one,1[]two,2")
int i=5

x.CellInt(0 1)=i ;;the same as x.Cell(0 1)=F"{i}"
out x.Cell(0 1) ;;"5"

x.CellHex(0 1)=i ;;the same as x.Cell(0 1)=F"0x{i}"
out x.Cell(0 1) ;;"0x5"

i=x.CellInt(0 1) ;;the same as i=val(x.Cell(0 1))
if(_hresult) out "not a number"; else out i
