function [hGrid] [str&gridVar]

ICsv c=__CopyCsv(0)

if(&gridVar) c.ToString(gridVar)
if(hGrid) DlgGrid g.Init(hGrid); g.FromICsv(c)

err+
