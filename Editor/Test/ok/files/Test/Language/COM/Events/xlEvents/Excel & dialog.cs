ClearOutput
Excel.Application a._create
a._setevents("a_Events")
#opt dispatch 1
out "-------"
a.Visible=1
Excel.Workbook xlBook=a.Workbooks.Add(Excel.xlWBATWorksheet)
ShowDialog("Dialog200" 0)
out "-------"
a.Quit

 click Excel cells and look at QM output
