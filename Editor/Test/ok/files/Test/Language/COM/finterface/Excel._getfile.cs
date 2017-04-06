typelib Excel {00020813-0000-0000-C000-000000000046} 1.2 ;;Microsoft Excel 8.0 Object Library, ver 1.2
 #opt dispatch 1

Excel.Workbook x._getfile("$desktop$\Book5.xls")

Excel.Application a=x.Application
a.Visible=-1; err

Excel.Windows ws=a.Windows
Excel.Window w=ws.Item(1)
w.Visible=-1

2
