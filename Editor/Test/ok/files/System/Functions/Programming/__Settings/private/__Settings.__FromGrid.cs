function [hGrid] [str&gridVar]

ICsv c._create

if(&gridVar) c.FromString(gridVar)
else if(hGrid) DlgGrid g.Init(hGrid); g.ToICsv(c)
else ret

int i nc=m_c.ColumnCount-2
for(i 0 m_c.RowCount) m_c.ReplaceRowMS(i nc c.Cell(i 1) 2)

err+
