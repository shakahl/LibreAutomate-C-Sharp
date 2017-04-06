ICsv x._create
x.FromString("one,1[]two,2[]three,3")

int v=9
lpstr s
PF
rep(1000) s=x.Cell(1 1)
PN
rep(1000) _i=x.CellInt(1 1)
PN
rep(1000) x.Cell(1 1)=F"{9}"
PN
rep(1000) x.CellInt(1 1)=9
PN
rep(1000) x.CellHex(1 1)=9
PN
PO

x.Cell(1 1)="kk"

out x.Cell(1 1)
out _hresult
out x.CellInt(1 1)
out _hresult
out x.CellHex(1 1)
out _hresult

