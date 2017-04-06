typelib Excel {00020813-0000-0000-C000-000000000046} 1.2
ClearOutput
 Deb
Excel.Application a._create
a._setevents("a_Events")
#opt dispatch 1
opt waitmsg 1
out "-------"
a.Visible=1
 Excel.Worksheet xlSheet=+a.Workbooks.Add(xlWBATWorksheet).ActiveSheet
Excel.Workbook xlBook=a.Workbooks.Add(Excel.xlWBATWorksheet)
a.UserControl=1
wait 0 -WV " Excel"
out "-------"
a.Quit

 click Excel cells and look at QM output
