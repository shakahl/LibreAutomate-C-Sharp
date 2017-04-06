function $csv

 Gets settings from string, and replaces default values added by Init.
 You can use this function instead of FromReg, for example if you store settings in a file.

 csv - list of settings in CSV format: "name,value[]name,value[]..."


ICsv c._create
c.FromString(csv)

int i y nc

nc=c.ColumnCount; if(nc<2) ret
i=m_c.ColumnCount-1
nc=iif(nc<i nc i)-1

for(y 0 c.RowCount) if(m_m.IntGet(c.Cell(y 0) i)) m_c.ReplaceRowMS(i nc c.Cell(y 1) 2)

err+
